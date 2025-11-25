#include <msp430.h>
#include <stdint.h>

/*
 * MSP430FR5739 firmware for Exercise 6 (shaft encoder interface).
 *
 * Hardware expectation:
 * - External quadrature decoder generates step pulses on TA0CLK and a DIR line.
 * - TA0CLK assumed on P1.2 (TA0CLK function). Update ENC_CLK_* if your PCB uses a
 *   different pin. DIR line assumed on P1.3.
 * - UART0 on P2.0/P2.1 at 115200-8N1 for host interaction.
 *
 * Commands (ASCII + newline/carriage return):
 *   r  -> returns signed 32-bit count in ticks (text)
 *   z  -> zero the accumulator
 *   h  -> help
 *
 * Counting approach:
 *   TA0 runs in continuous mode, clocked from external pulses. On each read,
 *   delta = (TA0R - last_sample). Delta is added or subtracted from a 32-bit
 *   accumulator based on current DIR level. This keeps math lightweight while
 *   preserving counts across timer rollovers.
 */

#define ENC_CLK_PORT_DIR   P1DIR
#define ENC_CLK_PORT_SEL0  P1SEL0
#define ENC_CLK_PORT_SEL1  P1SEL1
#define ENC_CLK_BIT        BIT2      /* P1.2 TA0CLK (adjust if needed) */

#define ENC_DIR_PORT_DIR   P1DIR
#define ENC_DIR_PORT_IN    P1IN
#define ENC_DIR_BIT        BIT3      /* P1.3 DIR input */

#define UART_PORT_SEL0     P2SEL0
#define UART_PORT_SEL1     P2SEL1
#define UART_TX_BIT        BIT0      /* P2.0 UCA0TXD */
#define UART_RX_BIT        BIT1      /* P2.1 UCA0RXD */

static volatile int32_t encoder_accum = 0;
static volatile uint16_t last_sample = 0;

static void clock_init(void);
static void uart_init(void);
static void encoder_init(void);
static void uart_write_str(const char *s);
static void uart_write_i32(int32_t v);
static void encoder_update_snapshot(void);

void main(void)
{
    WDTCTL = WDTPW | WDTHOLD;
    clock_init();
    uart_init();
    encoder_init();

    __enable_interrupt();
    uart_write_str("Encoder reader ready\r\n");

    char cmd = 0;
    while (1) {
        if (UCA0IFG & UCRXIFG) {
            cmd = UCA0RXBUF;
            if (cmd == 'r' || cmd == 'R') {
                encoder_update_snapshot();
                uart_write_i32(encoder_accum);
                uart_write_str("\r\n");
            } else if (cmd == 'z' || cmd == 'Z') {
                encoder_accum = 0;
                last_sample = TA0R;
                uart_write_str("zeroed\r\n");
            } else if (cmd == 'h' || cmd == 'H') {
                uart_write_str("Commands: r=read, z=zero, h=help\r\n");
            } else if (cmd == '\r' || cmd == '\n') {
                /* ignore */
            } else {
                uart_write_str("?\r\n");
            }
        }
    }
}

static void clock_init(void)
{
    CSCTL0_H = CSKEY_H;
    CSCTL1 = DCOFSEL_0; /* 1 MHz */
    CSCTL2 = SELA__VLOCLK | SELS__DCOCLK | SELM__DCOCLK;
    CSCTL3 = DIVA__1 | DIVS__1 | DIVM__1;
    CSCTL0_H = 0;
}

static void uart_init(void)
{
    UART_PORT_SEL0 |= UART_TX_BIT | UART_RX_BIT;
    UART_PORT_SEL1 &= ~(UART_TX_BIT | UART_RX_BIT);

    UCA0CTLW0 = UCSWRST;
    UCA0CTLW0 |= UCSSEL__SMCLK;
    UCA0BRW = 8;                         /* 1 MHz / 115200 */
    UCA0MCTLW = UCOS16 | UCBRF_10 | 0xF700;
    UCA0CTLW0 &= ~UCSWRST;
}

static void encoder_init(void)
{
    ENC_CLK_PORT_DIR &= ~ENC_CLK_BIT;
    ENC_CLK_PORT_SEL0 |= ENC_CLK_BIT;   /* route TA0CLK */
    ENC_CLK_PORT_SEL1 &= ~ENC_CLK_BIT;

    ENC_DIR_PORT_DIR &= ~ENC_DIR_BIT;   /* input */

    TA0CTL = TASSEL__TACLK | MC__CONTINUOUS | TACLR;
    last_sample = TA0R;
}

static void encoder_update_snapshot(void)
{
    uint16_t now = TA0R;
    uint16_t delta = now - last_sample; /* unsigned math handles rollover */
    last_sample = now;

    if (ENC_DIR_PORT_IN & ENC_DIR_BIT) {
        encoder_accum += delta;
    } else {
        encoder_accum -= delta;
    }
}

static void uart_write_str(const char *s)
{
    while (*s) {
        while (!(UCA0IFG & UCTXIFG)) { }
        UCA0TXBUF = *s++;
    }
}

static void uart_write_i32(int32_t v)
{
    char buf[16];
    int idx = 0;
    if (v < 0) {
        buf[idx++] = '-';
        v = -v;
    }
    int32_t tmp = v;
    int digits = 0;
    do {
        ++digits;
        tmp /= 10;
    } while (tmp > 0);

    for (int i = digits - 1; i >= 0; --i) {
        buf[idx + i] = (v % 10) + '0';
        v /= 10;
    }
    idx += digits;
    buf[idx] = '\0';
    uart_write_str(buf);
}
