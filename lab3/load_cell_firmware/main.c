#include <msp430.h>
#include <stdint.h>

/*
 * Load-cell acquisition firmware for MSP430FR5739.
 *
 * - Samples the load-cell amplifier output connected to A0 (P1.0) with the
 *   ADC12_B in 10-bit mode referenced to AVCC (0‑3.6 V).
 * - Streams framed samples over UART (USCI_A0) at 115200 bps.
 * - Frame format: 0xFF marker, 5 MSBs, 5 LSBs (three-byte packet).
 *
 * Wiring:
 *   • Load-cell amplifier output → P1.0/A0.
 *   • MSP430 UART TX (P2.0) → host RX.
 *   • Ensure the amplified signal remains below ~2.5 V for headroom.
 */

#define FRAME_MARKER 0xFF

static void init_clock(void);
static void init_gpio(void);
static void init_uart(void);
static void init_adc(void);
static void init_timer_trigger(void);
static void send_frame(uint8_t ms5, uint8_t ls5);

#if defined(__GNUC__)
static void timer0_b0_isr(void) __attribute__((interrupt(TIMER0_B0_VECTOR)));
static void adc12_isr(void) __attribute__((interrupt(ADC12_VECTOR)));
#endif

int main(void)
{
    WDTCTL = WDTPW | WDTHOLD; /* Stop watchdog */

    init_clock();
    init_gpio();
    init_uart();
    init_adc();
    init_timer_trigger();

    __bis_SR_register(GIE); /* Enable global interrupts */

    /* Enable conversions; Timer ISR triggers ADC12SC when ready */
    ADC12CTL0 |= ADC12ENC;

    while (1)
    {
        __bis_SR_register(LPM0_bits | GIE);
        __no_operation();
    }
}

static void init_clock(void)
{
    /* Unlock clock system registers */
    CSCTL0_H = CSKEY_H;

    /* DCO at ~8 MHz (factory trim) and source all clocks from it */
    CSCTL1 = DCOFSEL_3;
    CSCTL2 = SELA__VLOCLK | SELS__DCOCLK | SELM__DCOCLK;
    CSCTL3 = DIVA__1 | DIVS__1 | DIVM__1;

    /* Lock clock system registers */
    CSCTL0_H = 0;
}

static void init_gpio(void)
{
    /* Default all pins low output to reduce leakage */
    P1OUT = 0;
    P1DIR = 0xFF;
    P2OUT = 0;
    P2DIR = 0xFF;
    P3OUT = 0;
    P3DIR = 0xFF;
    PJOUT = 0;
    PJDIR = 0xFFFF;

    /* P1.0 as analog input (A0) */
    P1DIR &= ~BIT0;
    P1SEL0 |= BIT0;
    P1SEL1 |= BIT0;

    /* UART pins: P2.0 = UCA0TXD, P2.1 = UCA0RXD */
    P2SEL0 |= BIT0 | BIT1;
    P2SEL1 &= ~(BIT0 | BIT1);
}

static void init_uart(void)
{
    /* SMCLK at 8 MHz, target 115200 baud */
    UCA0CTLW0 = UCSWRST;        /* Hold eUSCI in reset */
    UCA0CTLW0 |= UCSSEL__SMCLK; /* SMCLK clock source */

    UCA0BRW = 4; /* Integer divider */
    UCA0MCTLW = UCOS16 | UCBRF_5 | 0x5500; /* Oversampling mode fine tuning */

    UCA0CTLW0 &= ~UCSWRST; /* Release for operation */
}

static void init_adc(void)
{
    ADC12CTL0 = ADC12SHT0_2 | ADC12ON; /* 32-cycle sample time, enable ADC */
    ADC12CTL1 = ADC12SHP;              /* Sample timer, single-channel mode */
    ADC12CTL2 = ADC12RES_1;            /* 10-bit resolution */
    ADC12MCTL0 = ADC12INCH_0 | ADC12VRSEL_0; /* Channel A0, AVCC reference */
    ADC12IER0 = ADC12IE0;              /* Enable interrupt for MEM0 */
}

static void init_timer_trigger(void)
{
    /*
     * Timer_B0 drives conversions at ~100 Hz by requesting ADC12SC whenever
     * CCR0 expires. 8 MHz / 80000 ≈ 100 Hz.
     */
    TB0CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB0CCR0 = 80000 - 1;
    TB0CCTL0 = CCIE;
}

static void send_frame(uint8_t ms5, uint8_t ls5)
{
    while (!(UCA0IFG & UCTXIFG))
        ;
    UCA0TXBUF = FRAME_MARKER;

    while (!(UCA0IFG & UCTXIFG))
        ;
    UCA0TXBUF = (uint8_t)(ms5 & 0x1F);

    while (!(UCA0IFG & UCTXIFG))
        ;
    UCA0TXBUF = (uint8_t)(ls5 & 0x1F);
}

#if defined(__TI_COMPILER_VERSION__)
#pragma vector = TIMER0_B0_VECTOR
static __interrupt void timer0_b0_isr(void)
#else
static void timer0_b0_isr(void)
#endif
{
    TB0CCTL0 &= ~CCIFG;

    if (!(ADC12CTL1 & ADC12BUSY))
    {
        ADC12CTL0 |= ADC12SC;
    }
}

#if defined(__TI_COMPILER_VERSION__)
#pragma vector = ADC12_VECTOR
static __interrupt void adc12_isr(void)
#else
static void adc12_isr(void)
#endif
{
    switch (__even_in_range(ADC12IV, ADC12IV_ADC12RDYIFG))
    {
        case ADC12IV_NONE:
            break;

        case ADC12IV_ADC12IFG0:
        {
            uint16_t raw = ADC12MEM0 & 0x03FFU; /* Only 10 bits are valid */
            uint8_t ms5 = (uint8_t)((raw >> 5) & 0x1F);
            uint8_t ls5 = (uint8_t)(raw & 0x1F);
            send_frame(ms5, ls5);
            __bic_SR_register_on_exit(LPM0_bits);
            break;
        }

        default:
            break;
    }
}
