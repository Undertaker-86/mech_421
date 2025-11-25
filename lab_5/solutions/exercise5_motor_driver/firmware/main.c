#include <msp430.h>

/*
 * MSP430FR5739 firmware for Exercise 5 (Motor driver checkout).
 *
 * - Drives DRV8841 inputs with a 25 kHz PWM on TB0.1 (P1.4) and a direction
 *   pin on P1.5. Adjust the pin definitions below if your board is wired
 *   differently.
 * - UART0 at 115200-8N1 on P2.0 (TXD) / P2.1 (RXD) accepts simple ASCII
 *   commands:
 *     d0 or d1   -> set direction low/high on DIR_PIN
 *     pXXX       -> set duty cycle (0-100 percent)
 *     ?          -> prints current duty/direction
 *
 * Suggested hookup on the green lab board:
 *   PWM -> P1.4 / TB0.1
 *   DIR -> P1.5 (GPIO)
 *   nSLEEP -> pull high (external) or repurpose another pin as needed.
 */

#define PWM_PORT_DIR   P1DIR
#define PWM_PORT_SEL0  P1SEL0
#define PWM_PORT_SEL1  P1SEL1
#define PWM_BIT        BIT4           /* P1.4 TB0.1 */

#define DIR_PORT_DIR   P1DIR
#define DIR_PORT_OUT   P1OUT
#define DIR_BIT        BIT5           /* P1.5 GPIO */

#define UART_PORT_SEL0 P2SEL0
#define UART_PORT_SEL1 P2SEL1
#define UART_TX_BIT    BIT0           /* P2.0 UCA0TXD */
#define UART_RX_BIT    BIT1           /* P2.1 UCA0RXD */

static volatile unsigned int duty_ticks = 1000; /* CCR1 value */
static const unsigned int pwm_period = 4000;    /* ~25 kHz when SMCLK=1 MHz */

static void clock_init(void);
static void uart_init(void);
static void pwm_init(void);
static void dir_init(void);
static void uart_write_str(const char *s);
static int  ascii_to_uint(const char *buf, unsigned len);

void main(void)
{
    WDTCTL = WDTPW | WDTHOLD;

    clock_init();
    uart_init();
    pwm_init();
    dir_init();

    __enable_interrupt();
    uart_write_str("Motor driver ready\r\n");

    /* Simple command interpreter */
    char rx_buf[8] = {0};
    unsigned idx = 0;
    while (1) {
        if (UCA0IFG & UCRXIFG) {
            char c = UCA0RXBUF;
            if (c == '\r' || c == '\n') {
                rx_buf[idx] = '\0';
                idx = 0;
                if (rx_buf[0] == 'd' && (rx_buf[1] == '0' || rx_buf[1] == '1')) {
                    if (rx_buf[1] == '0') {
                        DIR_PORT_OUT &= ~DIR_BIT;
                    } else {
                        DIR_PORT_OUT |= DIR_BIT;
                    }
                    uart_write_str("DIR set\r\n");
                } else if (rx_buf[0] == 'p') {
                    int val = ascii_to_uint(&rx_buf[1], 3);
                    if (val >= 0 && val <= 100) {
                        duty_ticks = (pwm_period * val) / 100;
                        TB0CCR1 = duty_ticks;
                        uart_write_str("PWM updated\r\n");
                    } else {
                        uart_write_str("ERR duty 0-100\r\n");
                    }
                } else if (rx_buf[0] == '?') {
                    char msg[32];
                    unsigned dir = (DIR_PORT_OUT & DIR_BIT) ? 1 : 0;
                    unsigned pct = (TB0CCR1 * 100) / pwm_period;
                    /* Simple itoa */
                    msg[0] = 'D'; msg[1] = 'I'; msg[2] = 'R'; msg[3] = '=';
                    msg[4] = dir ? '1' : '0'; msg[5] = ' ';
                    msg[6] = 'P'; msg[7] = 'W'; msg[8] = 'M'; msg[9] = '=';
                    msg[10] = (pct / 100) % 10 + '0';
                    msg[11] = ((pct / 10) % 10) + '0';
                    msg[12] = (pct % 10) + '0';
                    msg[13] = '%'; msg[14] = '\r'; msg[15] = '\n'; msg[16] = '\0';
                    uart_write_str(msg);
                } else if (rx_buf[0] != '\0') {
                    uart_write_str("Unknown cmd\r\n");
                }
            } else if (idx < sizeof(rx_buf) - 1) {
                rx_buf[idx++] = c;
            } else {
                idx = 0; /* overflow, reset */
            }
        }
    }
}

static void clock_init(void)
{
    /* Run DCO at 1 MHz for predictable UART and PWM settings. */
    CSCTL0_H = CSKEY_H;
    CSCTL1 = DCOFSEL_0;                 /* DCO = 1 MHz */
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
    /* 1 MHz / 115200 -> BRW=8, MCTLW tuned for low error */
    UCA0BRW = 8;
    UCA0MCTLW = UCOS16 | UCBRF_10 | 0xF700; /* UCBRS=0xF7 */
    UCA0CTLW0 &= ~UCSWRST;
}

static void pwm_init(void)
{
    PWM_PORT_DIR |= PWM_BIT;
    PWM_PORT_SEL0 |= PWM_BIT;
    PWM_PORT_SEL1 &= ~PWM_BIT;

    TB0CCR0 = pwm_period;
    TB0CCR1 = duty_ticks;
    TB0CCTL1 = OUTMOD_7; /* Reset/set */
    TB0CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
}

static void dir_init(void)
{
    DIR_PORT_DIR |= DIR_BIT;
    DIR_PORT_OUT &= ~DIR_BIT; /* Default direction low */
}

static void uart_write_str(const char *s)
{
    while (*s) {
        while (!(UCA0IFG & UCTXIFG)) { }
        UCA0TXBUF = *s++;
    }
}

static int ascii_to_uint(const char *buf, unsigned len)
{
    int v = 0;
    for (unsigned i = 0; i < len && buf[i] != '\0'; ++i) {
        if (buf[i] < '0' || buf[i] > '9') {
            return -1;
        }
        v = v * 10 + (buf[i] - '0');
    }
    return v;
}
