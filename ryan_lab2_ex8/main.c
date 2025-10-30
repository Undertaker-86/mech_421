#include <msp430.h>
#include <stdint.h>

/* ---------------- UART helpers (polling) ---------------- */
static void uart_putc(char c) {
    while (!(UCA0IFG & UCTXIFG)) ;   // wait for TX buffer ready
    UCA0TXBUF = c;
}
static void uart_puts(const char *s) {
    while (*s) uart_putc(*s++);
}
static void uart_putu8(uint8_t v) {   // print fixed 3 digits (000..255)
    char out[4];
    uint8_t t = v;
    out[0] = '0' + (t / 100); t %= 100;
    out[1] = '0' + (t / 10);
    out[2] = '0' + (t % 10);
    out[3] = 0;
    uart_puts(out);
}

/* ---------------- LED helpers ---------------- */
static inline void leds_all_off(void) {
    P3OUT &= ~(BIT4 | BIT5 | BIT6 | BIT7);
    PJOUT &= ~(BIT0 | BIT1 | BIT2 | BIT3);
}
static void leds_set_level(uint8_t lvl) {
    if (lvl > 8) lvl = 8;
    leds_all_off();
    if (lvl >= 1) PJOUT |= BIT0;  // LED1
    if (lvl >= 2) PJOUT |= BIT1;  // LED2
    if (lvl >= 3) PJOUT |= BIT2;  // LED3
    if (lvl >= 4) PJOUT |= BIT3;  // LED4
    if (lvl >= 5) P3OUT |= BIT4;  // LED5
    if (lvl >= 6) P3OUT |= BIT5;  // LED6
    if (lvl >= 7) P3OUT |= BIT6;  // LED7
    if (lvl >= 8) P3OUT |= BIT7;  // LED8
}

/* ---------------- Tiny delay ---------------- */
static void delay_cycles_coarse(volatile unsigned long n) {
    while (n--) __no_operation();
}

/* ---------------- Init: GPIO / NTC power / LEDs ---------------- */
static void init_gpio(void) {
    /* Power the NTC divider from P2.7 (drive high) */
    P2DIR |= BIT7;     // output
    P2OUT |= BIT7;     // high (3.3V)
    P2SEL0 &= ~BIT7;
    P2SEL1 &= ~BIT7;

    /* NTC sense node on P1.4/A4 -> route to ADC10 */
    P1DIR &= ~BIT4;
    P1SEL0 |= BIT4;
    P1SEL1 |= BIT4;

    /* LEDs as GPIO outputs (PJ.0..3 and P3.4..7) */
    PJDIR |= (BIT0 | BIT1 | BIT2 | BIT3);
    P3DIR |= (BIT4 | BIT5 | BIT6 | BIT7);

    /* Make sure LED pins are GPIO (not alternate functions) */
    P3SEL0 &= ~(BIT4 | BIT5 | BIT6 | BIT7);
    P3SEL1 &= ~(BIT4 | BIT5 | BIT6 | BIT7);

    leds_all_off();
}

/* ---------------- Init: UART A0 @ 9600 using SMCLK ≈ 1 MHz ----------------
   For 1 MHz SMCLK with oversampling:
   - BRW = 6
   - BRF = 8
   - BRS ≈ 0x20
   UCA0MCTLW literal used (no _OFS macros):
     UCOS16 (bit0) | (BRF<<4) | (BRS<<8) = 0x0001 | (8<<4) | (0x20<<8) = 0x2081
--------------------------------------------------------------------------- */
static void init_uart_9600_smclk_1mhz(void) {
    /* Route pins: P2.0=UCA0TXD, P2.1=UCA0RXD
       On FR57xx: secondary function -> P2SEL1=1, P2SEL0=0 for these bits */
    P2SEL1 |= (BIT0 | BIT1);
    P2SEL0 &= ~(BIT0 | BIT1);

    UCA0CTLW0 = UCSWRST;            // hold eUSCI in reset
    UCA0CTLW0 |= UCSSEL__SMCLK;     // SMCLK as clock source

    UCA0BRW   = 6;                  // prescaler
    UCA0MCTLW = 0x2081;             // UCOS16 + BRF=8 + BRS=0x20

    UCA0CTLW0 &= ~UCSWRST;          // release for operation
}

/* ---------------- Init: ADC10_B on A4 (P1.4), 10-bit ---------------- */
static void init_adc_a4(void) {
    ADC10CTL0 = ADC10SHT_2 | ADC10ON;            // S/H 16 cycles, ADC on
    ADC10CTL1 = ADC10SHP;                         // use sampling timer
    ADC10CTL2 = ADC10RES;                         // 10-bit mode (explicit)
    ADC10MCTL0 = ADC10SREF_0 | ADC10INCH_4;       // AVCC/AVSS, channel A4
    ADC10CTL0 |= ADC10ENC;                        // enable conversions
}

/* ---------------- One blocking ADC sample -> 8-bit ---------------- */
static uint8_t adc_read_8bit(void) {
    ADC10CTL0 |= ADC10SC;                         // start
    while (ADC10CTL1 & ADC10BUSY) ;               // wait
    return (uint8_t)(ADC10MEM0 >> 2);             // 10-bit to 8-bit
}

/* ============================== main ============================== */
int main(void) {
    uint32_t acc;
    uint8_t baseline;
    const uint8_t range = 5;  /* counts below baseline -> map to 8 LEDs (tune) */
    uint8_t v;
    int16_t delta;
    uint8_t level;
    unsigned i;

    WDTCTL = WDTPW | WDTHOLD;     // stop watchdog
    PM5CTL0 &= ~LOCKLPM5;         // unlock GPIO (FRAM devices)

    init_gpio();
    init_uart_9600_smclk_1mhz();
    init_adc_a4();

    uart_puts("MSP430 NTC (8-bit) ready @9600\r\n");

    /* Average a baseline at room temp so only LED1 is lit at rest */
    acc = 0;
    for (i = 0; i < 32; ++i) acc += adc_read_8bit();
    baseline = (uint8_t)(acc / 32);

    for (;;) {
        v = adc_read_8bit();                      // 0..255
        /* NTC: hotter -> LOWER code (negative temp coefficient) */
        delta = (int16_t)baseline - (int16_t)v;   // positive when hotter

        /* Map to 1..8 LEDs (LED1 at baseline; more LEDs as it warms) */
        level = 1;
        if (delta > 0) {
            uint8_t extra = (uint8_t)((7 * delta) / range);  // 0..7
            if (extra > 7) extra = 7;
            level = (uint8_t)(1 + extra);
        }
        leds_set_level(level);

        /* Print the reading */
        uart_puts("NTC=");
        uart_putu8(v);
        uart_puts("\r\n");

        __delay_cycles(100000UL);                // ~100 ms @ ~1 MHz MCLK
    }
}
