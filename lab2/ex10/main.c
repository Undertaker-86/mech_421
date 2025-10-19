#include <msp430.h>
#include <stdint.h>

/* ===================== Configuration ===================== */
#define CB_SIZE   50u
#define BAUD_9600_MCLK_1MHZ_BRW   6u
#define UCA0MCTLW_1MHZ_9600       0x2081  // oversampling config

/* Command map (per your request) */
#define CMD_LED_OFF       0x00
#define CMD_LED_ON        0x01
#define CMD_SET_PWM_FREQ  0x02
#define CMD_SET_DUTY      0x03
#define CMD_ALL_OFF       0x04   // stop PWM (pin low) and turn LED1 off

/* ===================== Circular Buffer ===================== */
static volatile uint8_t  cb[CB_SIZE];
static volatile uint16_t cb_head = 0;
static volatile uint16_t cb_tail = 0;
static volatile uint16_t cb_count = 0;

static volatile uint8_t flag_overrun  = 0;
static volatile uint8_t flag_underrun = 0;

/* ===================== Packet Assembly ===================== */
#define PACKET_LEN 5
static volatile uint8_t pkt_buf[PACKET_LEN];
static volatile uint8_t pkt_index = 0;
static volatile uint8_t collecting = 0;

/* ===================== UART helpers ===================== */
static inline void uart_putc(char c) {
    while (!(UCA0IFG & UCTXIFG));
    UCA0TXBUF = (uint8_t)c;
}
static void uart_puts(const char *s) { while (*s) uart_putc(*s++); }
static void uart_putu8(uint8_t v) {
    const char hex[17] = "0123456789ABCDEF";
    uart_putc(hex[v >> 4]);
    uart_putc(hex[v & 0xF]);
}
static void uart_putu16(uint16_t v) {
    uart_putu8((v >> 8) & 0xFF);
    uart_putu8(v & 0xFF);
}

/* ===================== Circular Buffer ===================== */
static inline uint8_t cb_push(uint8_t b) {
    if (cb_count >= CB_SIZE) return 0;
    cb[cb_tail++] = b;
    if (cb_tail == CB_SIZE) cb_tail = 0;
    cb_count++;
    return 1;
}
static inline uint8_t cb_pop(uint8_t *out) {
    if (cb_count == 0) return 0;
    *out = cb[cb_head++];
    if (cb_head == CB_SIZE) cb_head = 0;
    cb_count--;
    return 1;
}

/* ===================== Init UART/GPIO ===================== */
static void init_uart(void) {
    P2SEL1 |= (BIT0 | BIT1);
    P2SEL0 &= ~(BIT0 | BIT1);
    UCA0CTLW0  = UCSWRST;
    UCA0CTLW0 |= UCSSEL__SMCLK;
    UCA0BRW   = BAUD_9600_MCLK_1MHZ_BRW;
    UCA0MCTLW = UCA0MCTLW_1MHZ_9600;
    UCA0CTLW0 &= ~UCSWRST;
    UCA0IE    |= UCRXIE;
}

static void init_gpio(void) {
    PM5CTL0 &= ~LOCKLPM5;   // unlock GPIO

    /* LED1 on PJ.0 */
    PJSEL0 &= ~BIT0;
    PJSEL1 &= ~BIT0;
    PJDIR  |= BIT0;
    PJOUT  &= ~BIT0;

    /* PWM output on P3.4 (LED5) */
    P3DIR  |= BIT4;
    P3SEL0 |= BIT4;   // TB1.1
    P3SEL1 &= ~BIT4;
}

/* ===================== TimerB1 (PWM on P3.4/TB1.1) ===================== */
static void init_timerB1_pwm(void) {
    TB1CTL   = TBSSEL__SMCLK | MC__STOP | TBCLR; // stop initially
    TB1CCTL1 = OUTMOD_7;       // Reset/set mode
    TB1CCR0  = 2000 - 1;       // default 500 Hz
    TB1CCR1  = 1000;           // 50 % duty
}

/* Start/stop helpers */
static void start_pwm(void) {
    /* route peripheral and run */
    P3SEL0 |= BIT4;
    P3SEL1 &= ~BIT4;
    TB1CCTL1 = OUTMOD_7;
    TB1CTL   = TBSSEL__SMCLK | MC__UP | TBCLR;
}
static void stop_pwm(void) {
    /* stop timer */
    TB1CTL = TBSSEL__SMCLK | MC__STOP | TBCLR;
    TB1CCTL1 &= ~OUTMOD_7;     // stop driving compare output

    /* force pin low as GPIO */
    P3SEL0 &= ~BIT4;
    P3SEL1 &= ~BIT4;
    P3DIR  |= BIT4;
    P3OUT  &= ~BIT4;
}

/* ===================== UART ISR ===================== */
#pragma vector=USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void) {
    switch (__even_in_range(UCA0IV, 0x08)) {
        case 0x00: break;
        case 0x02: {
            uint8_t rx = UCA0RXBUF;

            if (!collecting) {
                if (rx == 0xFF) {
                    collecting = 1;
                    pkt_index = 0;
                    pkt_buf[pkt_index++] = rx;
                }
            } else {
                pkt_buf[pkt_index++] = rx;
                if (pkt_index == PACKET_LEN) {
                    collecting = 0;
                    uint8_t i;
                    for (i = 0; i < PACKET_LEN; i++)
                        if (!cb_push(pkt_buf[i])) flag_overrun = 1;
                }
            }
            break;
        }
        default: break;
    }
}

/* ===================== Packet Processing ===================== */
static void process_packet(uint8_t *pkt) {
    uint8_t cmd   = pkt[1];
    uint8_t dataH = pkt[2];
    uint8_t dataL = pkt[3];
    uint8_t esc   = pkt[4];

    /* Apply escape correction */
    if (esc & BIT0) dataH = 0xFF;
    if (esc & BIT1) dataL = 0xFF;
    uint16_t value = ((uint16_t)dataH << 8) | dataL

    uart_puts("CMD=");
    uart_putu8(cmd);
    uart_puts(" VAL=");
    uart_putu16(value);
    uart_puts("\r\n");

    switch (cmd) {
        case CMD_LED_OFF:
            PJOUT &= ~BIT0;
            uart_puts("LED1 OFF\r\n");
            break;

        case CMD_LED_ON:
            PJOUT |= BIT0;
            uart_puts("LED1 ON\r\n");
            break;

        case CMD_SET_PWM_FREQ: {
            /* value is CCR0 target (period-1). Guard minimum of 1 tick. */
            uint16_t ccr0 = (value == 0) ? 1 : value;
            TB1CCR0 = ccr0;
            /* Keep last duty ratio approx: clamp to period. */
            if (TB1CCR1 > TB1CCR0) TB1CCR1 = TB1CCR0;
            start_pwm();
            uart_puts("PWM frequency updated\r\n");
            break;
        }

        case CMD_SET_DUTY: {
            /* value is CCR1 (high time). Clamp to CCR0. */
            if (value > TB1CCR0) value = TB1CCR0;
            TB1CCR1 = value;
            /* Ensure PWM running so duty takes effect. */
            if ((TB1CTL & MC_3) == MC_0) start_pwm();
            uart_puts("PWM duty updated\r\n");
            break;
        }

        case CMD_ALL_OFF:
            /* Stop PWM and turn LED1 off */
            stop_pwm();
            PJOUT &= ~BIT0;
            uart_puts("ALL OFF\r\n");
            break;

        default:
            uart_puts("Unknown CMD\r\n");
            break;
    }
}

/* ===================== MAIN ===================== */
int main(void) {
    WDTCTL = WDTPW | WDTHOLD;
    init_gpio();
    init_uart();
    init_timerB1_pwm();

    uart_puts("\r\nMSP430FR5739 Packet Control (cmd: 0=LEDoff,1=LEDon,2=freq,3=duty,4=alloff)\r\n");

    __bis_SR_register(GIE);

    for (;;) {
        if (flag_overrun) {
            flag_overrun = 0;
            uart_puts("[ERR] Buffer overrun\r\n");
        }
        if (cb_count >= PACKET_LEN) {
            uint8_t pkt[PACKET_LEN];
            uint8_t i;
            for (i = 0; i < PACKET_LEN; i++) cb_pop(&pkt[i]);
            process_packet(pkt);
        }
        __no_operation();
    }
}
