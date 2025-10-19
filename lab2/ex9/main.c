#include <msp430.h>
#include <stdint.h>

/* ===================== Config ===================== */
#define CB_SIZE   16u      /* circular buffer length */
#define BAUD_9600_MCLK_1MHZ_BRW   6u
/* UCA0MCTLW literal for 1 MHz / 9600, oversampling: UCOS16 | BRF=8 | BRS=0x20 */
#define UCA0MCTLW_1MHZ_9600       0x2081

/* ===================== Circular Buffer ===================== */
static volatile uint8_t  cb[CB_SIZE];
static volatile uint16_t cb_head = 0;    /* index of next pop */
static volatile uint16_t cb_tail = 0;    /* index of next push */
static volatile uint16_t cb_count = 0;   /* # of elements stored */

/* Error flags set inside ISR, handled/printed in main loop */
static volatile uint8_t  flag_overrun  = 0;
static volatile uint8_t  flag_underrun = 0;

/* --------- UART helpers (polling TX, IRQ RX) --------- */
static inline void uart_putc(char c) {
    while (!(UCA0IFG & UCTXIFG)) ;
    UCA0TXBUF = (uint8_t)c;
}
static void uart_puts(const char *s) {
    while (*s) uart_putc(*s++);
}
static void uart_putu16(uint16_t v) {
    /* small decimal print (0..65535) */
    char buf[6];
    int i = 5;
    buf[i] = '\0'; --i;
    if (v == 0) { buf[i]='0'; uart_puts(&buf[i]); return; }
    while (v && i >= 0) { buf[i--] = '0' + (v % 10); v /= 10; }
    uart_puts(&buf[i+1]);
}

/* --------- Push/Pop for circular buffer (IRQ-safe) --------- */
static inline uint8_t cb_push(uint8_t b) {
    if (cb_count >= CB_SIZE) return 0;                 /* overrun */
    cb[cb_tail] = b;
    cb_tail++;
    if (cb_tail == CB_SIZE) cb_tail = 0;
    cb_count++;
    return 1;
}
static inline uint8_t cb_pop(uint8_t *out) {
    if (cb_count == 0) return 0;                       /* underrun */
    *out = cb[cb_head];
    cb_head++;
    if (cb_head == CB_SIZE) cb_head = 0;
    cb_count--;
    return 1;
}

/* ===================== Init: GPIO & UART ===================== */
static void init_uart_9600_smclk_1mhz(void) {
    /* Route P2.0 (TXD) and P2.1 (RXD) to eUSCI_A0 */
    P2SEL1 |= (BIT0 | BIT1);
    P2SEL0 &= ~(BIT0 | BIT1);

    UCA0CTLW0  = UCSWRST;                /* hold in reset */
    UCA0CTLW0 |= UCSSEL__SMCLK;          /* SMCLK as clock */

    UCA0BRW   = BAUD_9600_MCLK_1MHZ_BRW; /* prescaler = 6 */
    UCA0MCTLW = UCA0MCTLW_1MHZ_9600;     /* UCOS16 + BRF=8 + BRS≈0x20 */

    UCA0CTLW0 &= ~UCSWRST;               /* enable */
    UCA0IE    |= UCRXIE;                 /* enable RX interrupt */
}

static void init_gpio(void) {
    /* FRAM devices: unlock GPIO */
    PM5CTL0 &= ~LOCKLPM5;

    /* Nothing else required for this exercise’s UART + buffer */
}

/* ===================== Interrupts ===================== */
#pragma vector=USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void) {
    switch (__even_in_range(UCA0IV, 0x08)) {
        case 0x00: break;                 /* no interrupt */
        case 0x02: {                      /* UCRXIFG: byte received */
            uint8_t rx = UCA0RXBUF;       /* read to clear flag */

            if (rx == 13) {
                /* Carriage return: pop ONE char and echo it back */
                uint8_t outch;
                if (cb_pop(&outch)) {
                    /* echo the popped character */
                    while (!(UCA0IFG & UCTXIFG)) ;
                    UCA0TXBUF = outch;
                } else {
                    flag_underrun = 1;    /* nothing to pop */
                }
            } else {
                /* Normal char: push into circular buffer */
                if (!cb_push(rx)) {
                    flag_overrun = 1;     /* buffer full */
                }
            }
            break;
        }
        case 0x04: break;                 /* UCTXIFG (unused; we poll for TX) */
        default:  break;
    }
}

/* ===================== Main ===================== */
int main(void) {
    WDTCTL = WDTPW | WDTHOLD;   /* stop watchdog */
    init_gpio();
    init_uart_9600_smclk_1mhz();

    uart_puts("\r\nFR5739 Circular Buffer @9600\r\n");
    uart_puts("Type chars; on CR I pop one and echo it.\r\n");
    uart_puts("Buffer size = ");
    uart_putu16(CB_SIZE);
    uart_puts("\r\n");

    __bis_SR_register(GIE);     /* enable global interrupts */

    for (;;) {
        /* Print any errors outside ISR to keep ISR short/fast */
        if (flag_overrun) {
            flag_overrun = 0;
            uart_puts("[ERR] Buffer overrun (dropped input)\r\n");
        }
        if (flag_underrun) {
            flag_underrun = 0;
            uart_puts("[ERR] Buffer underrun (nothing to pop)\r\n");
        }

        /* OPTIONAL: show buffer occupancy periodically (uncomment) */
        /*
        static uint32_t ctr = 0;
        if (++ctr == 50000ul) {
            ctr = 0;
            uart_puts("count=");
            uart_putu16(cb_count);
            uart_puts(" head=");
            uart_putu16(cb_head);
            uart_puts(" tail=");
            uart_putu16(cb_tail);
            uart_puts("\r\n");
        }
        */

        /* Idle; everything happens in RX ISR */
        __no_operation();
    }
}
