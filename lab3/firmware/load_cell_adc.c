#include <msp430.h>
#include <stdint.h>

/*
 * Simple load-cell acquisition firmware for the MSP430FR5739.
 *
 * - Samples the analog signal connected to A0 (P1.0) with the ADC12_B in
 *   10-bit mode using AVCC (3.6 V max) as the reference.
 * - Streams framed measurements over UART (USCI_A0) at 115200 bps.
 * - Frame format: 0xFF marker, 5 MSBs, 5 LSBs.
 *
 * Connect the load-cell amplifier output (0-3.6 V) to pin P1.0/A0.
 * Connect UART TX (P2.0) to the host's RX input.
 */

#define FRAME_MARKER 0xFF

static void init_clock(void);
static void init_gpio(void);
static void init_uart(void);
static void init_adc(void);
static void init_timer_trigger(void);
static void send_frame(uint8_t ms5, uint8_t ls5);

int main(void)
{
    WDTCTL = WDTPW | WDTHOLD; /* Stop watchdog */

    init_clock();
    init_gpio();
    init_uart();
    init_adc();
    init_timer_trigger();

    __bis_SR_register(GIE); /* Enable global interrupts */

    /* Enable conversions; Timer will toggle ADC12SC */
    ADC12CTL0 |= ADC12ENC;

    while (1)
    {
        __bis_SR_register(LPM0_bits | GIE);
        __no_operation();
    }
}

static void init_clock(void)
{
    /* Unlock CS registers */
    CSCTL0_H = CSKEY_H;

    /* Set DCO to 8 MHz (factory trimmed) */
    CSCTL1 = DCOFSEL_3;
    CSCTL2 = SELA__VLOCLK | SELS__DCOCLK | SELM__DCOCLK;
    CSCTL3 = DIVA__1 | DIVS__1 | DIVM__1;

    /* Lock CS registers */
    CSCTL0_H = 0;
}

static void init_gpio(void)
{
    /* Default all pins to output low to reduce power */
    P1OUT = 0;
    P1DIR = 0xFF;
    P2OUT = 0;
    P2DIR = 0xFF;
    P3OUT = 0;
    P3DIR = 0xFF;
    PJOUT = 0;
    PJDIR = 0xFFFF;

    /* Configure P1.0 for analog input (A0) */
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
    UCA0CTLW0 = UCSWRST;        /* Put eUSCI in reset */
    UCA0CTLW0 |= UCSSEL__SMCLK; /* SMCLK source */

    UCA0BRW = 4;                /* Divider */
    UCA0MCTLW = UCOS16 | UCBRF_5 | 0x5500; /* Oversampling mode */

    UCA0CTLW0 &= ~UCSWRST; /* Initialize eUSCI */
}

static void init_adc(void)
{
    /* Sample-and-hold 32 ADC clocks, ADC on */
    ADC12CTL0 = ADC12SHT0_2 | ADC12ON;

    /* Use sampling timer, single-channel single-conversion */
    ADC12CTL1 = ADC12SHP;

    /* 10-bit resolution (ADC12RES_1) */
    ADC12CTL2 = ADC12RES_1;

    /* AVCC reference, input channel A0 */
    ADC12MCTL0 = ADC12INCH_0 | ADC12VRSEL_0;

    /* Enable interrupt when conversion finished */
    ADC12IER0 = ADC12IE0;
}

static void init_timer_trigger(void)
{
    /*
     * Timer_B0 drives ADC conversions at ~100 Hz by toggling ADC12SC every time
     * TB0CCR0 overflows. 8 MHz / 80000 ≈ 100 Hz.
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
    UCA0TXBUF = ms5 & 0x1F;

    while (!(UCA0IFG & UCTXIFG))
        ;
    UCA0TXBUF = ls5 & 0x1F;
}

/* Timer_B0 CCR0 interrupt: kicks the ADC to start a new conversion */
#pragma vector = TIMER0_B0_VECTOR
__interrupt static void TIMER0_B0_ISR(void)
{
    TB0CCTL0 &= ~CCIFG;

    /* Trigger conversion if ADC is ready */
    if (!(ADC12CTL1 & ADC12BUSY))
    {
        ADC12CTL0 |= ADC12SC;
    }
}

/* ADC12 conversion complete interrupt */
#pragma vector = ADC12_VECTOR
__interrupt static void ADC12_ISR(void)
{
    switch (__even_in_range(ADC12IV, ADC12IV_ADC12RDYIFG))
    {
        case ADC12IV_NONE:
            break;
        case ADC12IV_ADC12IFG0:
        {
            uint16_t raw = ADC12MEM0 & 0x03FF; /* Only 10 bits are valid */
            uint8_t ms5 = (raw >> 5) & 0x1F;
            uint8_t ls5 = raw & 0x1F;
            send_frame(ms5, ls5);
            __bic_SR_register_on_exit(LPM0_bits);
            break;
        }
        default:
            break;
    }
}
