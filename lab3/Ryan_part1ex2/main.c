// main.c  — Thermistor ADC → 3-byte frames (255, MS5B, LS5B)
#include <msp430.h>
#include <stdint.h>

#define START_BYTE   0xFF
#define SAMPLE_HZ    5u

static void gpio_init(void) {
    // Route P1.4 to ADC10 A4 (datasheet shows P1.4/TB0.1/UCA0STE/A4)  [A4]
    P1DIR  &= ~BIT4;
    P1SEL0 |=  BIT4;
    P1SEL1 |=  BIT4;

    // UART pins: P2.0=UCA0TXD, P2.1=UCA0RXD (back-channel)
    P2SEL1 |=  (BIT0 | BIT1);
    P2SEL0 &= ~(BIT0 | BIT1);

    PM5CTL0 &= ~LOCKLPM5; // enable configured I/Os (FRAM devices)
}

static void uart_init_9600_smclk1m(void) {
    UCA0CTLW0 = UCSWRST | UCSSEL__SMCLK; // hold in reset, SMCLK
    UCA0BRW   = 6;                       // 1 MHz / 16 / 9600 ≈ 6.51
    UCA0MCTLW = 0x2081;                  // UCOS16 | BRF=8 | BRS≈0x20
    UCA0CTLW0 &= ~UCSWRST;
}

static void adc_init_a4(void) {
    ADC10CTL0 &= ~ADC10ENC;                       // cfg gate (ENC=0)
    ADC10CTL0  = ADC10SHT_2 | ADC10ON;            // 16 S&H, ON
    ADC10CTL1  = ADC10SHP | ADC10SSEL_3;          // SAMPCON timer, SMCLK
    ADC10CTL2  = ADC10RES;                        // 10-bit
    ADC10MCTL0 = ADC10SREF_0 | ADC10INCH_4;       // AVCC/AVSS, channel A4
    ADC10CTL0 |= ADC10ENC;                        // arm
}

static void timer_init_50hz(void) {
    // TA0 Up mode, SMCLK ~1 MHz → CCR0 = 1e6 / 50 = 20000
    TA0CCR0  = 20000 - 1;
    TA0CCTL0 = CCIE;
    TA0CTL   = TASSEL__SMCLK | MC__UP | TACLR;
}

static inline void uart_send(uint8_t b) {
    while (!(UCA0IFG & UCTXIFG)) ;
    UCA0TXBUF = b;
}

static inline uint16_t adc_read_blocking(void) {
    ADC10CTL0 |= ADC10SC;                      // start
    while (ADC10CTL1 & ADC10BUSY) ;            // wait
    return ADC10MEM0 & 0x03FF;                 // 10-bit
}

int main(void) {
    WDTCTL = WDTPW | WDTHOLD;

    gpio_init();
    uart_init_9600_smclk1m();
    adc_init_a4();
    timer_init_50hz();

    __bis_SR_register(GIE);                    // IRQs on
    for (;;)
        __no_operation();
}

#pragma vector = TIMER0_A0_VECTOR
__interrupt void TIMER0_A0_ISR(void) {
    uint16_t adc = adc_read_blocking();        // 0..1023
    uint8_t ms5  = (adc >> 5) & 0x1F;
    uint8_t ls5  =  adc       & 0x1F;

    uart_send(START_BYTE);
    uart_send(ms5);
    uart_send(ls5);
}
