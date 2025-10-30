#include <msp430.h>
#include <stdint.h>

#define SMCLK_HZ              1000000UL
#define TIMER_DIVIDER         40UL                       // SMCLK / (ID * TAIDEX)
#define TIMER_TICK_HZ         (SMCLK_HZ / TIMER_DIVIDER) // 25 kHz
#define TWO_SECONDS_TICKS     (TIMER_TICK_HZ * 2UL)      // 50,000 ticks

static const uint8_t button_bits[2]   = { BIT0, BIT1 };
static const char     button_labels[2] = { '1', '2' };

static volatile uint16_t press_start_ticks[2] = { 0, 0 };
static volatile uint8_t waiting_for_release[2] = { 0, 0 };
static volatile uint8_t pending_button = 0;
static volatile uint8_t pending_value = 0;

static void timer_init(void);
static void button_init(void);
static void uart_init(void);
static void uart_putc(char c);
static void uart_print_uint8(uint8_t value);
static void handle_button_event(uint8_t index);

int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;              // Stop watchdog timer

    timer_init();
    button_init();
    uart_init();

    PM5CTL0 &= ~LOCKLPM5;                  // Unlock GPIOs after configuration

    __bis_SR_register(GIE);                // Enable global interrupts

    while (1)
    {
        if (pending_button != 0)
        {
            uint8_t button;
            uint8_t value;

            __disable_interrupt();
            button = pending_button;
            value = pending_value;
            pending_button = 0;
            __enable_interrupt();

            uart_putc('S');
            uart_putc(button_labels[button - 1]);
            uart_putc(':');
            uart_print_uint8(value);
            uart_putc('\r');
            uart_putc('\n');
        }

        __no_operation();
    }
}

static void timer_init(void)
{
    TA0CTL = TASSEL__SMCLK | MC__CONTINUOUS | TACLR | ID__8;
    TA0EX0 = TAIDEX_4;                     // Additional divide-by-5 (total divide = 40)
}

static void button_init(void)
{
    P4DIR &= ~(BIT0 | BIT1);               // P4.0/P4.1 as inputs (S1/S2)
    P4REN |= BIT0 | BIT1;                  // Enable resistor network
    P4OUT |= BIT0 | BIT1;                  // Select pull-ups

    P4IES |= BIT0 | BIT1;                  // Start by detecting falling edges (button press)
    P4IFG &= ~(BIT0 | BIT1);               // Clear pending flags
    P4IE  |= BIT0 | BIT1;                  // Enable interrupts for S1/S2
}

static void uart_init(void)
{
    P2SEL1 |= BIT0 | BIT1;                 // P2.0=UCA0TXD, P2.1=UCA0RXD
    P2SEL0 &= ~(BIT0 | BIT1);

    UCA0CTL1 |= UCSWRST;                   // Hold eUSCI in reset
    UCA0CTL1 |= UCSSEL__SMCLK;             // SMCLK clock source

    UCA0BRW = 6;                           // 1 MHz / 16 / 9600 ~= 6.51
    UCA0MCTLW = UCOS16 | UCBRF_8 | 0x2000; // Fractional baud-rate settings

    UCA0CTL1 &= ~UCSWRST;                  // Release for operation
}

static void uart_putc(char c)
{
    while (!(UCA0IFG & UCTXIFG))
    {
        ;
    }
    UCA0TXBUF = c;
}

static void uart_print_uint8(uint8_t value)
{
    char buf[4];
    uint8_t i = 0;

    if (value == 0)
    {
        uart_putc('0');
        return;
    }

    while (value > 0 && i < sizeof(buf))
    {
        buf[i++] = (char)('0' + (value % 10U));
        value /= 10U;
    }

    while (i > 0)
    {
        uart_putc(buf[--i]);
    }
}

#pragma vector = PORT4_VECTOR
__interrupt void Port_4_ISR(void)
{
    switch (__even_in_range(P4IV, P4IV_P4IFG7))
    {
        case P4IV_P4IFG0:
            handle_button_event(0);
            break;

        case P4IV_P4IFG1:
            handle_button_event(1);
            break;

        default:
            break;
    }
}

static void handle_button_event(uint8_t index)
{
    uint8_t bit = button_bits[index];

    if (!waiting_for_release[index])
    {
        press_start_ticks[index] = TA0R;
        waiting_for_release[index] = 1;
        P4IES &= ~bit;                      // Next interrupt on rising edge
    }
    else
    {
        uint16_t now = TA0R;
        uint16_t start = press_start_ticks[index];
        uint16_t elapsed;

        if (now >= start)
        {
            elapsed = now - start;
        }
        else
        {
            elapsed = (uint16_t)(now + (uint16_t)0x10000 - start);
        }

        if (elapsed >= TWO_SECONDS_TICKS)
        {
            pending_value = 255;
        }
        else
        {
            uint32_t scaled = (uint32_t)elapsed * 255UL;
            scaled /= TWO_SECONDS_TICKS;
            pending_value = (uint8_t)scaled;
        }

        pending_button = index + 1;
        waiting_for_release[index] = 0;
        P4IES |= bit;                       // Re-arm falling-edge detection
    }
}
