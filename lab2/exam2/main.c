#include <msp430.h>
#include <stdint.h>

#define LED_COUNT 8
#define PJ_LED_MASK (BIT0 | BIT1 | BIT2 | BIT3)
#define P3_LED_MASK (BIT4 | BIT5 | BIT6 | BIT7)

static volatile uint8_t led_index = 0;

static void drive_current_led(void);

int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;              // Stop watchdog timer

    // Configure LED ports as GPIO outputs.
    PJDIR |= PJ_LED_MASK;
    PJSEL0 &= ~PJ_LED_MASK;
    PJSEL1 &= ~PJ_LED_MASK;

    P3DIR |= P3_LED_MASK;
    P3SEL0 &= ~P3_LED_MASK;
    P3SEL1 &= ~P3_LED_MASK;

    PJOUT &= ~PJ_LED_MASK;
    P3OUT &= ~P3_LED_MASK;
    drive_current_led();                   // Start with PJ.0 illuminated

    // Configure S1 (P4.0) as input with pull-up and falling-edge interrupt.
    P4DIR &= ~BIT0;
    P4REN |= BIT0;                         // Enable resistor network
    P4OUT |= BIT0;                         // Select pull-up
    P4IES |= BIT0;                         // Detect high-to-low transitions
    P4IFG &= ~BIT0;                        // Clear pending flag
    P4IE  |= BIT0;                         // Enable local interrupt

    PM5CTL0 &= ~LOCKLPM5;                  // Unlock I/O pins

    __bis_SR_register(GIE);                // Enable global interrupts

    while (1)
    {
        __no_operation();                  // CPU idle; ISR handles state changes
    }
}

static void drive_current_led(void)
{
    PJOUT &= ~PJ_LED_MASK;
    P3OUT &= ~P3_LED_MASK;

    if (led_index < 4)
    {
        PJOUT |= (uint8_t)(BIT0 << led_index);
    }
    else
    {
        uint8_t offset = led_index - 4;
        P3OUT |= (uint8_t)(BIT4 << offset);
    }
}

#pragma vector = PORT4_VECTOR
__interrupt void Port_4_ISR(void)
{
    switch (__even_in_range(P4IV, P4IV_P4IFG7))
    {
        case P4IV_P4IFG0:
            led_index++;
            if (led_index >= LED_COUNT)
            {
                led_index = 0;
            }
            drive_current_led();
            break;

        default:
            break;
    }
}
