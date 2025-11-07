#include <msp430.h>
#include <stdint.h>

#define START_BYTE 0xFF
#define TICK_10MS  10000u     // 10 ms @ SMCLK = 1 MHz

static void gpio_init(void);
static void uart_init_9600_smclk1M(void);
static void adc_init_A4_AVCC(void);
static void timer_init_10ms(void);
static inline void uart_tx(uint8_t b);
static uint16_t adc_sample_blocking(void);

int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;          // stop watchdog

    gpio_init();
    uart_init_9600_smclk1M();
    adc_init_A4_AVCC();
    timer_init_10ms();

    PM5CTL0 &= ~LOCKLPM5;              // unlock I/Os on FRAM parts
    __bis_SR_register(GIE);            // global IRQs on

    while (1) { __no_operation(); }    // all work done in timer ISR
}

/* ---- GPIO: route P1.4 to analog, P2.0/2.1 to UART ---- */
static void gpio_init(void)
{
    // P1.4 as analog input (A4)
    P1DIR  &= ~BIT4;
    P1SEL0 |=  BIT4;
    P1SEL1 |=  BIT4;                   // selects analog function on FR57xx

    // P2.0=TXD, P2.1=RXD for eUSCI_A0
    P2SEL1 |= (BIT0 | BIT1);
    P2SEL0 &= ~(BIT0 | BIT1);
}

/* ---- UART A0 @ 9600 using SMCLK ≈ 1 MHz (oversampling) ---- */
static void uart_init_9600_smclk1M(void)
{
    UCA0CTLW0 = UCSWRST | UCSSEL__SMCLK;     // hold reset, SMCLK
    UCA0BRW   = 6;                           // 1 MHz / (16*9600) ≈ 6.51
    UCA0MCTLW = 0x2081;                      // UCOS16 + BRF=8 + BRS≈0x20
    UCA0CTLW0 &= ~UCSWRST;                   // enable
}

static inline void uart_tx(uint8_t b)
{
    while (!(UCA0IFG & UCTXIFG));
    UCA0TXBUF = b;
}

/* ---- ADC10_B: single-channel, single-conversion on A4, Vref=AVCC ---- */
static void adc_init_A4_AVCC(void)
{
    ADC10CTL0 &= ~ADC10ENC;                  // allow config
    ADC10CTL0  = ADC10SHT_2 | ADC10ON;       // 16-cycle S/H, ADC on
    ADC10CTL1  = ADC10SHP;                    // sample timer
    ADC10CTL2  = ADC10RES;                    // 10-bit result
    ADC10MCTL0 = ADC10SREF_0 | ADC10INCH_4;  // AVCC/AVSS, channel A4
    ADC10CTL0 |= ADC10ENC;
    // (ADC10_B register names/fields per FR57xx User’s Guide.) :contentReference[oaicite:3]{index=3}
}

static uint16_t adc_sample_blocking(void)
{
    ADC10CTL0 |= ADC10SC;                     // start conversion
    while (ADC10CTL1 & ADC10BUSY) ;           // wait
    return ADC10MEM0 & 0x03FF;                // 10-bit value
}

/* ---- TimerA0 fires every 10 ms ---- */
static void timer_init_10ms(void)
{
    TA0CCR0  = TICK_10MS - 1;
    TA0CCTL0 = CCIE;
    TA0CTL   = TASSEL__SMCLK | MC__UP | TACLR;  // SMCLK ~1 MHz
}

/* ---- 10 ms ISR: sample, split to 5+5 bits, send 3 bytes ---- */
#pragma vector = TIMER0_A0_VECTOR
__interrupt void TIMER0_A0_ISR(void)
{
    uint16_t adc = adc_sample_blocking();     // 0..1023 for 0..3.3V (AVCC)
    uint8_t  ms5 = (adc >> 5) & 0x1F;         // bits [9:5]
    uint8_t  ls5 =  adc       & 0x1F;         // bits [4:0]

    uart_tx(START_BYTE);                      // Out byte 1
    uart_tx(ms5);                              // Out byte 2 (MS5B)
    uart_tx(ls5);                              // Out byte 3 (LS5B)
}
