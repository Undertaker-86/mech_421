#include <msp430.h>
#include <stdint.h>

#define START_BYTE          0xFF
#define TIMER_PERIOD_SMCLK  (10000u)    // 10 ms @ 1 MHz SMCLK

static void gpio_init(void);
static void adc_init(void);
static void uart_init(void);
static void timer_init(void);
static uint16_t adc_sample_channel(uint16_t channel);
static void uart_send_byte(uint8_t byte);

int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;    // Stop watchdog timer

    gpio_init();
    uart_init();
    adc_init();
    timer_init();

    PM5CTL0 &= ~LOCKLPM5;        // Enable previously disabled port settings

    __bis_SR_register(GIE);      // Enable global interrupts

    while (1)
    {
        __no_operation();
    }
}

static void gpio_init(void)
{
    // P2.7 powers the accelerometer when driven high.
    P2DIR |= BIT7;
    P2OUT |= BIT7;
    P2SEL0 &= ~BIT7;
    P2SEL1 &= ~BIT7;

    // Configure accelerometer outputs (A12/A13/A14) on P3.0-P3.2 as analog inputs.
    P3DIR &= ~(BIT0 | BIT1 | BIT2);
    P3SEL0 |= BIT0 | BIT1 | BIT2;
    P3SEL1 |= BIT0 | BIT1 | BIT2;
}

static void adc_init(void)
{
    ADC10CTL0 &= ~ADC10ENC;                    // Ensure ENC is clear before configuring
    ADC10CTL0 = ADC10SHT_2 | ADC10ON;          // 16-cycle sample, ADC on
    ADC10CTL1 = ADC10SHP;                      // Use sampling timer (ADC10OSC)
    ADC10CTL2 = ADC10RES;                      // 10-bit resolution (default)
}

static void uart_init(void)
{
    // Use eUSCI_A0 on P2.0/P2.1 for UART TX/RX.
    P2SEL1 |= BIT0 | BIT1;
    P2SEL0 &= ~(BIT0 | BIT1);

    UCA0CTL1 |= UCSWRST;
    UCA0CTL1 = UCSWRST | UCSSEL__SMCLK;

    UCA0BRW = 6;                               // 1 MHz / 16 / 9600 ~= 6.51
    UCA0MCTLW = UCOS16 | UCBRF_8 | 0x2000;     // Fractional divider settings

    UCA0CTL1 &= ~UCSWRST;
}

static void timer_init(void)
{
    TA0CCR0 = TIMER_PERIOD_SMCLK - 1;
    TA0CCTL0 = CCIE;
    TA0CTL = TASSEL__SMCLK | MC__UP | TACLR;
}

static void uart_send_byte(uint8_t byte)
{
    while (!(UCA0IFG & UCTXIFG))
    {
        ;
    }
    UCA0TXBUF = byte;
}

static uint16_t adc_sample_channel(uint16_t channel)
{
    while (ADC10CTL1 & ADC10BUSY)
    {
        ;
    }

    ADC10CTL0 &= ~ADC10ENC;                    // Allow channel selection update
    ADC10MCTL0 = channel;                      // Configure input channel
    ADC10CTL0 |= ADC10ENC | ADC10SC;           // Start conversion

    while (ADC10CTL1 & ADC10BUSY)
    {
        ;
    }

    return ADC10MEM0;
}

#pragma vector = TIMER0_A0_VECTOR
__interrupt void TIMER0_A0_ISR(void)
{
    uint16_t raw_x = adc_sample_channel(ADC10INCH_12);
    uint16_t raw_y = adc_sample_channel(ADC10INCH_13);
    uint16_t raw_z = adc_sample_channel(ADC10INCH_14);

    uart_send_byte(START_BYTE);
    uart_send_byte((uint8_t)(raw_x >> 2));    // Compress 10-bit to 8-bit values
    uart_send_byte((uint8_t)(raw_y >> 2));
    uart_send_byte((uint8_t)(raw_z >> 2));
}
