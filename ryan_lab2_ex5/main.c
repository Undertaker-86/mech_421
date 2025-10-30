#include <msp430.h>

/*
 * Exercise 5: Timer B outputs on LED5 (P3.4) and LED6 (P3.5).
 *
 * Timer1_B is configured in up mode using SMCLK at 1 MHz.
 * - CCR0 sets the period to 2 ms (500 Hz).
 * - CCR1 drives TB1.1 at 50% duty cycle (LED5, P3.4).
 * - CCR2 drives TB1.2 at 25% duty cycle (LED6, P3.5).
 */
int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;          // Stop watchdog

    // Route P3.4 and P3.5 to Timer_B outputs and make them outputs.
    P3DIR  |= BIT4 | BIT5;             // LED5/LED6 direction
    P3SEL0 |= BIT4 | BIT5;             // Select TB1.1 / TB1.2 function
    P3SEL1 &= ~(BIT4 | BIT5);

    PM5CTL0 &= ~LOCKLPM5;              // Enable previously configured ports

    // Timer1_B setup: SMCLK source (1 MHz), up mode, clear TAR.
    TB1CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB1CCR0 = 2000 - 1;                // 1 MHz / 2000 = 500 Hz

    TB1CCTL1 = OUTMOD_7;               // Reset/set PWM mode
    TB1CCR1  = 1000;                   // 50% duty cycle (LED5 bright)

    TB1CCTL2 = OUTMOD_7;               // Reset/set PWM mode
    TB1CCR2  = 500;                    // 25% duty cycle (LED6 dimmer)

    while (1)
    {
        __no_operation();              // CPU idle; timer hardware drives outputs
    }
}


