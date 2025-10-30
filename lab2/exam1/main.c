#include <msp430.h>

#define SMCLK_HZ         1000000UL
#define PWM_FREQUENCY    5000UL
#define PWM_PERIOD_TICKS (SMCLK_HZ / PWM_FREQUENCY)
#define PWM_DUTY_TICKS   ((PWM_PERIOD_TICKS * 10UL) / 100UL)

int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;              // Halt watchdog timer

    P3DIR  |= BIT4;                        // Route P3.4 (LED5) to TB1.1
    P3SEL0 |= BIT4;
    P3SEL1 &= ~BIT4;

    PM5CTL0 &= ~LOCKLPM5;                  // Release GPIO pins from high-Z mode

    TB1CCR0 = PWM_PERIOD_TICKS - 1;        // 5 kHz period from 1 MHz SMCLK
    TB1CCTL1 = OUTMOD_7;                   // Reset/set PWM mode on TB1.1
    TB1CCR1 = PWM_DUTY_TICKS;              // 10% duty cycle (~20 us high time)
    TB1CTL = TBSSEL__SMCLK | MC__UP | TBCLR;

    while (1)
    {
        __no_operation();                  // Timer hardware drives PWM autonomously
    }
}
