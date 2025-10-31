#include <msp430.h>
#include <stdint.h>

#define START_BYTE          0xFF
#define SAMPLE_RATE_HZ      100u
#define SMCLK_HZ            1000000u          // default ~1 MHz unless you change CS
#define TA0_PERIOD          (SMCLK_HZ / SAMPLE_RATE_HZ)  // 10 ms ticks

static inline void uart_send(uint8_t b) {
    while (!(UCA0IFG & UCTXIFG)) ;
    UCA0TXBUF = b;
}

/* ---- one blocking ADC read on channel A4 (P1.4) ---- */
static inline uint16_t adc_read_A4_10bit(void) {
    while (ADC10CTL1 & ADC10BUSY) ;           // wait if busy
    ADC10CTL0 &= ~ADC10ENC;                    // allow MCTL change
    ADC10MCTL0 = ADC10SREF_0 | ADC10INCH_4;    // AVCC/AVSS, input channel A4
    ADC10CTL0 |= ADC10ENC | ADC10SC;           // start
    while (ADC10CTL1 & ADC10BUSY) ;            // wait end
    return ADC10MEM0;                           // 10-bit result (0..1023)
}

/* ---- init blocks ---- */
static void init_gpio_uart(void) {
    // Route P2.0=UCA0TXD, P2.1=UCA0RXD (eUSCI_A0)
    // (Function mapping per datasheet: P2.0/UCA0TXD, P2.1/UCA0RXD)
    P2SEL1 |= BIT0 | BIT1;
    P2SEL0 &= ~(BIT0 | BIT1);

    // Analog input on P1.4 (A4*): select peripheral
    P1SEL0 |= BIT4;
    P1SEL1 |= BIT4;
}

static void init_uart_9600_smclk1m(void) {
    UCA0CTLW0 = UCSWRST;                 // hold eUSCI_A0
    UCA0CTLW0 |= UCSSEL__SMCLK;          // SMCLK clock
    // 1 MHz / 16 / 9600: BRW=6, BRF=8, BRS≈0x20 (oversampling)
    UCA0BRW   = 6;
    UCA0MCTLW = UCOS16 | UCBRF_8 | 0x2000;
    UCA0CTLW0 &= ~UCSWRST;               // enable
}

static void init_adc10_a4(void) {
    // ADC10_B: 10-bit, sampling timer, AVCC/AVSS
    ADC10CTL0 = ADC10SHT_2 | ADC10ON;    // sample-and-hold 16 cycles, ADC on
    ADC10CTL1 = ADC10SHP;                // use sampling timer (MODCLK source by default)
    ADC10CTL2 = ADC10RES;                // 10-bit resolution
    // Input channel is set in adc_read_A4_10bit() via ADC10MCTL0
}

static void init_timer100Hz(void) {
    TA0CCR0  = TA0_PERIOD - 1;
    TA0CCTL0 = CCIE;                      // interrupt on CCR0
    TA0CTL   = TASSEL__SMCLK | MC__UP | TACLR;
}

int main(void) {
    WDTCTL   = WDTPW | WDTHOLD;          // stop watchdog
    PM5CTL0 &= ~LOCKLPM5;                // unlock GPIO (FRAM devices)

    init_gpio_uart();
    init_uart_9600_smclk1m();
    init_adc10_a4();
    init_timer100Hz();

    __bis_SR_register(GIE);              // global IRQs on
    while (1) {
        __no_operation();                // all work done in timer ISR
    }
}

/* ---- 100 Hz sampler, packetizes [0xFF, MS5B, LS5B] ---- */
#pragma vector = TIMER0_A0_VECTOR
__interrupt void TIMER0_A0_ISR(void) {
    uint16_t raw = adc_read_A4_10bit();       // 0..1023 corresponds to ~0..AVCC
    uint8_t ms5  = (raw >> 5) & 0x1F;         // upper 5 bits
    uint8_t ls5  =  raw       & 0x1F;         // lower 5 bits

    uart_send(START_BYTE);
    uart_send(ms5);
    uart_send(ls5);
}
