#include <msp430.h>
#include <stdint.h>

/*
 * Exercise 6: Measure high pulse width using Timer_A.
 *
 * The square wave generated in Exercise 5 (e.g. P3.4/TB1.1) is jumper-wired
 * to P1.2, which feeds the Timer1_A CCR1 capture input.
 * Timer1_A runs from SMCLK (default ~1 MHz) in continuous mode. Each capture
 * interrupt records the time between a rising edge and the subsequent falling
 * edge. The duration in SMCLK ticks is stored in the global variable
 * pulse_ticks, which you can inspect in the debugger.
 */

#define CAPTURE_COMMON   (CCIS_0 | CAP | SCS | CCIE)
#define CAPTURE_RISING   (CM_1 | CAPTURE_COMMON)
#define CAPTURE_FALLING  (CM_2 | CAPTURE_COMMON)

volatile uint16_t rising_edge = 0;      // Timestamp of the most recent rising edge
volatile uint16_t pulse_ticks = 0;      // High-time width in SMCLK cycles
volatile uint8_t  waiting_for_fall = 0; // 0: waiting for rising, 1: waiting for falling
volatile uint8_t  measurement_ready = 0;

static void uart_init(void);
static void uart_putc(char c);
static void uart_print(const char *s);
static void uart_print_uint16(uint16_t value);
static void timerb_init(void);

int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;          // Stop watchdog timer

    // Route P1.2 to Timer1_A capture input (TA1.1) and configure as input.
    P1DIR  &= ~BIT2;
    P1SEL0 |= BIT2;
    P1SEL1 &= ~BIT2;

    timerb_init();

    PM5CTL0 &= ~LOCKLPM5;              // Unlock GPIO pins

    uart_init();
    uart_print("\r\nTimer capture ready.\r\n");

    // Timer1_A: SMCLK source, continuous mode, clear TAR.
    TA1CTL = TASSEL__SMCLK | MC__CONTINUOUS | TACLR;

    // Start by watching for the rising edge of the pulse.
    TA1CCTL1 = CAPTURE_RISING;

    __bis_SR_register(GIE);            // Enable global interrupts

    while (1)
    {
        if (measurement_ready)
        {
            measurement_ready = 0;
            uart_print("pulse_ticks = ");
            uart_print_uint16(pulse_ticks);
            uart_print(" ticks\r\n");
        }
        __no_operation();
    }
}

#pragma vector = TIMER1_A1_VECTOR
__interrupt void TIMER1_A1_ISR(void)
{
    switch (__even_in_range(TA1IV, TA1IV_TAIFG))
    {
        case TA1IV_NONE:
            break;

        case TA1IV_TACCR1:
        {
            uint16_t captured = TA1CCR1;

            if (!waiting_for_fall)
            {
                rising_edge = captured;
                waiting_for_fall = 1;
                TA1CCTL1 = CAPTURE_FALLING;
            }
            else
            {
                pulse_ticks = captured - rising_edge;
                waiting_for_fall = 0;
                TA1CCTL1 = CAPTURE_RISING;
                measurement_ready = 1;
            }
            break;
        }

        default:
            break;
    }
}

static void uart_init(void)
{
    P2SEL1 |= BIT0 | BIT1;            // P2.0=TXD, P2.1=RXD
    P2SEL0 &= ~(BIT0 | BIT1);

    UCA0CTL1 |= UCSWRST;             // Hold USCI in reset
    UCA0CTL1 |= UCSSEL__SMCLK;       // SMCLK source (1 MHz default)

    UCA0BRW = 6;                     // 1 MHz / 16 / 9600 = 6.xx
    UCA0MCTLW = UCOS16 | UCBRF_8 | 0x2000;

    UCA0CTL1 &= ~UCSWRST;            // Release for operation
}

static void uart_putc(char c)
{
    while (!(UCA0IFG & UCTXIFG));
    UCA0TXBUF = c;
}

static void uart_print(const char *s)
{
    while (*s)
    {
        uart_putc(*s++);
    }
}

static void uart_print_uint16(uint16_t value)
{
    char buf[6];
    int i = 0;

    if (value == 0)
    {
        uart_putc('0');
        return;
    }

    while (value > 0 && i < 5)
    {
        buf[i++] = '0' + (value % 10);
        value /= 10;
    }

    while (i > 0)
    {
        uart_putc(buf[--i]);
    }
}

static void timerb_init(void)
{
    // Configure P3.4 (TB1.1) and P3.5 (TB1.2) for PWM output to generate the test pulse.
    P3DIR  |= BIT4 | BIT5;
    P3SEL0 |= BIT4 | BIT5;
    P3SEL1 &= ~(BIT4 | BIT5);

    // Timer1_B setup: SMCLK @ 1 MHz, up mode; CCR0 sets a 2 ms period (500 Hz).
    TB1CTL  = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB1CCR0 = 2000 - 1;

    TB1CCTL1 = OUTMOD_7;               // Reset/set mode -> 50% duty cycle on P3.4
    TB1CCR1  = 1000;

    TB1CCTL2 = OUTMOD_7;               // Reset/set mode -> 25% duty cycle on P3.5
    TB1CCR2  = 500;
}
