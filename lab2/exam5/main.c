/* --COPYRIGHT--,BSD
 * Copyright (c) 2017, Texas Instruments Incorporated
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * *  Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 *
 * *  Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 *
 * *  Neither the name of Texas Instruments Incorporated nor the names of
 *    its contributors may be used to endorse or promote products derived
 *    from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
 * OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
 * EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * --/COPYRIGHT--*/
#include "driverlib.h"
#include <msp430.h>
#include <stdint.h>

//******************************************************************************
// Exam 5: Computer Controlled 3-Axis Shock Sensor
//
// Features:
// 1. ADC10_B samples accelerometer axes (A13/A14/A15) periodically at 100 Hz
// 2. Detects shock when acceleration exceeds per-axis thresholds
// 3. Software PWM dimming orb effect (~2 s fade) on PJ.0/PJ.1/PJ.2
// 4. UART protocol: [0xFF][Axis:0x01-0x03][Threshold:0x00-0xFE]
//
// Hardware:
// - P2.7: Accelerometer power (drive high)
// - P3.0/P3.1/P3.2: A12/A13/A14 (X/Y/Z accelerometer axes)
// - PJ.0/PJ.1/PJ.2: LEDs for X/Y/Z axes
// - P2.0/P2.1: UART TX/RX @ 9600 baud
//******************************************************************************

/* ===================== Configuration ===================== */
#define PWM_FREQ_HZ         2000u       // Software PWM base frequency
#define PWM_STEPS           20u         // Brightness levels (0..20)
#define SAMPLE_FREQ_HZ      100u        // ADC sampling rate
#define FADE_STEP_MS        100u        // Brightness decrease every 100ms
#define DEFAULT_THRESHOLD   150u         // Default shock threshold (8-bit)

/* ===================== Global State (volatile) ===================== */
// Baselines (captured at startup)
static volatile uint8_t baseline_x = 128;
static volatile uint8_t baseline_y = 128;
static volatile uint8_t baseline_z = 157;

// Thresholds (controlled via UART)
static volatile uint8_t threshold_x = DEFAULT_THRESHOLD;
static volatile uint8_t threshold_y = DEFAULT_THRESHOLD;
static volatile uint8_t threshold_z = DEFAULT_THRESHOLD;

// Brightness state (0..20)
static volatile uint8_t brightness_x = 0;
static volatile uint8_t brightness_y = 0;
static volatile uint8_t brightness_z = 0;

// PWM substep counter (0..19)
static volatile uint8_t pwm_substep = 0;

// Sample scheduler counter (for 100ms fade steps)
static volatile uint8_t fade_counter = 0;

// UART packet state
static volatile uint8_t uart_state = 0;     // 0=wait start, 1=got start, 2=got axis
static volatile uint8_t uart_axis = 0;
static volatile uint8_t uart_threshold = 0;

/* ===================== Function Prototypes ===================== */
static void init_gpio(void);
static void init_adc(void);
static void init_uart(void);
static void init_timers(void);
static void capture_baseline(void);
static uint8_t adc_sample_channel(uint16_t channel);
static void uart_send_byte(uint8_t byte);
static inline uint8_t abs_diff(uint8_t a, uint8_t b);

/* ===================== Main ===================== */
void main(void)
{
    WDTCTL = WDTPW | WDTHOLD;           // Stop watchdog timer

    init_gpio();
    init_uart();
    init_adc();

    PM5CTL0 &= ~LOCKLPM5;               // Unlock GPIO

    // Capture baseline accelerometer values
    // capture_baseline();

    // Send startup message
    uart_send_byte(0xFF);
    uart_send_byte(0xAA);               // Startup marker
    uart_send_byte(baseline_x);
    uart_send_byte(baseline_y);
    uart_send_byte(baseline_z);

    init_timers();                      // Start timers last

    __bis_SR_register(GIE);             // Enable global interrupts

    // Main loop: idle in LPM0, all work in ISRs
    while (1)
    {
        __bis_SR_register(LPM0_bits);
        __no_operation();
    }
}

/* ===================== Initialization ===================== */
static void init_gpio(void)
{
    // Power accelerometer: P2.7 as output high
    P2DIR |= BIT7;
    P2OUT |= BIT7;
    P2SEL0 &= ~BIT7;
    P2SEL1 &= ~BIT7;

    // Accelerometer analog inputs: P3.0/P3.1/P3.2 → A12/A13/A14
    P3DIR &= ~(BIT0 | BIT1 | BIT2);
    P3SEL0 |= BIT0 | BIT1 | BIT2;
    P3SEL1 |= BIT0 | BIT1 | BIT2;

    // LEDs: PJ.0/PJ.1/PJ.2 as GPIO outputs (initially off)
    PJSEL0 &= ~(BIT0 | BIT1 | BIT2);
    PJSEL1 &= ~(BIT0 | BIT1 | BIT2);
    PJDIR |= BIT0 | BIT1 | BIT2;
    PJOUT &= ~(BIT0 | BIT1 | BIT2);
}

static void init_adc(void)
{
    ADC10CTL0 &= ~ADC10ENC;                     // Ensure ENC clear
    ADC10CTL0 = ADC10SHT_2 | ADC10ON;           // 16-cycle S/H, ADC on
    ADC10CTL1 = ADC10SHP;                        // Use sampling timer
    ADC10CTL2 = ADC10RES;                        // 10-bit resolution
}

static void init_uart(void)
{
    // UART on P2.0 (TX) / P2.1 (RX)
    P2SEL1 |= BIT0 | BIT1;
    P2SEL0 &= ~(BIT0 | BIT1);

    UCA0CTL1 |= UCSWRST;                        // Hold in reset
    UCA0CTL1 = UCSWRST | UCSSEL__SMCLK;         // SMCLK source

    // 9600 baud from 1 MHz SMCLK
    UCA0BRW = 6;
    UCA0MCTLW = UCOS16 | UCBRF_8 | 0x2000;

    UCA0CTL1 &= ~UCSWRST;                       // Release
    UCA0IE |= UCRXIE;                           // Enable RX interrupt
}

static void init_timers(void)
{
    // Timer_A0: 2 kHz for software PWM substeps
    // CCR0 = (1 MHz / 2000 Hz) - 1 = 499
    TA0CCR0 = 499;
    TA0CCTL0 = CCIE;                            // Enable CCR0 interrupt
    TA0CTL = TASSEL__SMCLK | MC__UP | TACLR;    // SMCLK, up mode

    // Timer_A1: 100 Hz for ADC sampling and fade control
    // CCR0 = (1 MHz / 100 Hz) - 1 = 9999
    TA1CCR0 = 9999;
    TA1CCTL0 = CCIE;                            // Enable CCR0 interrupt
    TA1CTL = TASSEL__SMCLK | MC__UP | TACLR;    // SMCLK, up mode
}

/* ===================== ADC Functions ===================== */
static void capture_baseline(void)
{
    uint32_t sum_x = 0, sum_y = 0, sum_z = 0;
    uint8_t i;

    // Average 32 samples per axis
    for (i = 0; i < 32; i++)
    {
        sum_x += adc_sample_channel(ADC10INCH_12);
        sum_y += adc_sample_channel(ADC10INCH_13);
        sum_z += adc_sample_channel(ADC10INCH_14);
        __delay_cycles(1000);                   // Small delay between samples
    }

    baseline_x = (uint8_t)((sum_x / 32) >> 2);  // 10-bit to 8-bit
    baseline_y = (uint8_t)((sum_y / 32) >> 2);
    baseline_z = (uint8_t)((sum_z / 32) >> 2);
}

static uint8_t adc_sample_channel(uint16_t channel)
{
    while (ADC10CTL1 & ADC10BUSY);              // Wait if busy

    ADC10CTL0 &= ~ADC10ENC;                     // Disable to change channel
    ADC10MCTL0 = channel;                        // Set channel
    ADC10CTL0 |= ADC10ENC | ADC10SC;            // Enable and start

    while (ADC10CTL1 & ADC10BUSY);              // Wait for completion

    return (uint8_t)(ADC10MEM0 >> 2);           // 10-bit to 8-bit
}

/* ===================== UART Functions ===================== */
static void uart_send_byte(uint8_t byte)
{
    while (!(UCA0IFG & UCTXIFG));
    UCA0TXBUF = byte;
}

/* ===================== Utility ===================== */
static inline uint8_t abs_diff(uint8_t a, uint8_t b)
{
    return (a > b) ? (a - b) : (b - a);
}

/* ===================== ISR: Timer_A0 (2 kHz PWM) ===================== */
#pragma vector = TIMER0_A0_VECTOR
__interrupt void TIMER0_A0_ISR(void)
{
    // Software PWM: substep 0..19
    // LED on if substep < brightness, else off

    if (pwm_substep < brightness_x)
        PJOUT |= BIT0;
    else
        PJOUT &= ~BIT0;

    if (pwm_substep < brightness_y)
        PJOUT |= BIT1;
    else
        PJOUT &= ~BIT1;

    if (pwm_substep < brightness_z)
        PJOUT |= BIT2;
    else
        PJOUT &= ~BIT2;

    pwm_substep++;
    if (pwm_substep >= PWM_STEPS)
        pwm_substep = 0;
}

/* ===================== ISR: Timer_A1 (100 Hz Sampling) ===================== */
#pragma vector = TIMER1_A0_VECTOR
__interrupt void TIMER1_A0_ISR(void)
{
    uint8_t sample_x, sample_y, sample_z;

    // Sample all three axes
    sample_x = adc_sample_channel(ADC10INCH_12);
    sample_y = adc_sample_channel(ADC10INCH_13);
    sample_z = adc_sample_channel(ADC10INCH_14);

    // Detect shock and trigger full brightness
    if (abs_diff(sample_x, baseline_x) >= threshold_x)
        brightness_x = PWM_STEPS;

    if (abs_diff(sample_y, baseline_y) >= threshold_y)
        brightness_y = PWM_STEPS;

    if (abs_diff(sample_z, baseline_z) >= threshold_z)
        brightness_z = PWM_STEPS;

    // Fade control: decrease brightness every 100ms (10 ticks at 100 Hz)
    fade_counter++;
    if (fade_counter >= 10)
    {
        fade_counter = 0;

        if (brightness_x > 0) brightness_x--;
        if (brightness_y > 0) brightness_y--;
        if (brightness_z > 0) brightness_z--;
    }
}

/* ===================== ISR: UART RX ===================== */
#pragma vector = USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void)
{
    switch (__even_in_range(UCA0IV, 0x08))
    {
        case 0x00: break;                       // No interrupt
        case 0x02:                              // UCRXIFG
        {
            uint8_t rx = UCA0RXBUF;

            switch (uart_state)
            {
                case 0:                         // Waiting for start byte
                    if (rx == 0xFF)
                    {
                        uart_state = 1;
                    }
                    break;

                case 1:                         // Expecting axis byte
                    if (rx >= 0x01 && rx <= 0x03)
                    {
                        uart_axis = rx;
                        uart_state = 2;
                    }
                    else
                    {
                        uart_state = 0;         // Invalid, reset
                    }
                    break;

                case 2:                         // Expecting threshold byte
                    if (rx <= 0xFE)             // Valid range 0x00-0xFE
                    {
                        uart_threshold = rx;

                        // Update threshold for specified axis
                        if (uart_axis == 0x01)
                            threshold_x = uart_threshold;
                        else if (uart_axis == 0x02)
                            threshold_y = uart_threshold;
                        else if (uart_axis == 0x03)
                            threshold_z = uart_threshold;

                        // Send ACK: echo back the packet
                        uart_send_byte(0xFF);
                        uart_send_byte(uart_axis);
                        uart_send_byte(uart_threshold);
                    }
                    uart_state = 0;             // Reset for next packet
                    break;

                default:
                    uart_state = 0;
                    break;
            }
            break;
        }
        default: break;
    }
}
