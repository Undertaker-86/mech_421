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
#include <msp430.h>
#include <stdint.h>

/*
 * Exam 4: Inclinometer
 *
 * Requirements:
 * 1. Timer_A @ 100 Hz (10 ms) triggers ADC sample on X-axis (A12).
 * 2. 16-sample circular buffer for X-axis; running average via sum.
 * 3. TB1.1 PWM on P3.4 (LED5) controlled by X-axis average.
 * 4. Brightness changes with tilt (0–90°).
 * 5. Use shifts for multiply/divide by 2.
 *
 * SMCLK: 1 MHz (default DCO).
 * Accelerometer: A12=P3.0 (X), powered by P2.7.
 * LED5: P3.4 = TB1.1 PWM output.
 */

/* ===================== Circular Buffer & Average ===================== */
#define BUF_SIZE 16
static volatile uint16_t xbuf[BUF_SIZE];   // circular buffer for X-axis samples
static volatile uint8_t  idx = 0;          // current index (0..15)
static volatile uint16_t sum = 0;          // running sum of last 16 samples
static volatile uint16_t avg = 0;          // average = sum >> 4

static uint16_t baseline = 512;            // calibrated baseline for flat board (mid-scale)

/* ===================== Init Functions ===================== */
static void init_gpio(void) {
    // Power accelerometer: P2.7 high
    P2DIR |= BIT7;
    P2OUT |= BIT7;
    P2SEL0 &= ~BIT7;
    P2SEL1 &= ~BIT7;

    // X-axis on P3.0/A12 as analog input
    P3DIR &= ~BIT0;
    P3SEL0 |= BIT0;
    P3SEL1 |= BIT0;

    // LED5 on P3.4 for TB1.1 PWM output
    P3DIR |= BIT4;
    P3SEL0 |= BIT4;
    P3SEL1 &= ~BIT4;

    PM5CTL0 &= ~LOCKLPM5;   // unlock GPIO
}

static void init_adc(void) {
    ADC10CTL0 &= ~ADC10ENC;                    // disable to configure
    ADC10CTL0 = ADC10ON | ADC10SHT_2;          // ADC on, 16-cycle sample
    ADC10CTL1 = ADC10SHP;                      // use sampling timer
    ADC10CTL2 = ADC10RES;                      // 10-bit resolution
    ADC10MCTL0 = ADC10SREF_0 | ADC10INCH_12;   // Vcc/Vss ref, channel A12 (X-axis)
    ADC10IE |= ADC10IE0;                       // enable EOC interrupt
    ADC10CTL0 |= ADC10ENC;                     // enable conversions
}

static void init_timer_a0(void) {
    // Timer_A0 @ 100 Hz: SMCLK 1 MHz / 10000 = 100 Hz
    TA0CCR0 = 10000 - 1;                       // 10 ms period
    TA0CCTL0 = CCIE;                           // enable CCR0 interrupt
    TA0CTL = TASSEL__SMCLK | MC__UP | TACLR;   // SMCLK, up mode, clear
}

static void init_timer_b1_pwm(void) {
    // TB1 PWM @ 1 kHz on TB1.1 (P3.4/LED5)
    TB1CTL = TBSSEL__SMCLK | MC__UP | TBCLR;   // SMCLK, up mode, clear
    TB1CCR0 = 1000 - 1;                        // 1 kHz PWM frequency
    TB1CCTL1 = OUTMOD_7;                       // reset/set mode
    TB1CCR1 = 0;                               // start with 0% duty (LED off)
}

static uint16_t adc_sample_blocking(void) {
    // Take one blocking ADC sample (used for calibration only)
    ADC10CTL0 &= ~ADC10ENC;
    ADC10MCTL0 = ADC10SREF_0 | ADC10INCH_12;
    ADC10CTL0 |= ADC10ENC | ADC10SC;           // start conversion
    while (ADC10CTL1 & ADC10BUSY);             // wait for completion
    return ADC10MEM0;
}

static void calibrate_baseline(void) {
    // Average 32 samples to get baseline when board is flat
    uint32_t acc = 0;
    uint8_t i;
    for (i = 0; i < 32; i++) {
        acc += adc_sample_blocking();
        __delay_cycles(10000);                 // 10 ms delay between samples
    }
    baseline = (uint16_t)(acc >> 5);           // divide by 32 (shift right 5)
}

/* ===================== Interrupt Service Routines ===================== */
#pragma vector = TIMER0_A0_VECTOR
__interrupt void TIMER0_A0_ISR(void) {
    // Triggered every 10 ms (100 Hz)
    // Trigger ADC conversion on X-axis (A12)
    ADC10CTL0 |= ADC10SC;                      // start conversion
}

#pragma vector = ADC10_VECTOR
__interrupt void ADC10_ISR(void) {
    switch (__even_in_range(ADC10IV, ADC10IV_ADC10IFG)) {
        case ADC10IV_NONE:
            break;
        case ADC10IV_ADC10IFG: {
            uint16_t sample = ADC10MEM0;       // read 10-bit result (0..1023)

            // Update circular buffer and running sum
            sum -= xbuf[idx];                  // subtract oldest sample
            sum += sample;                     // add new sample
            xbuf[idx] = sample;                // store new sample
            idx = (idx + 1) & 0x0F;            // increment index (mod 16)
            
            avg = sum >> 4;                    // average = sum / 16 (shift right 4)

            // Calculate delta from baseline (tilt magnitude)
            int16_t delta = (int16_t)avg - (int16_t)baseline;
            if (delta < 0) delta = -delta;     // absolute value

            // Scale to PWM duty cycle using shifts
            // Multiply by 16 to expand range: delta << 4
            // This maps ~0-64 counts (0-90°) to 0-1024 PWM range
            uint16_t scaled = (uint16_t)delta << 4;
            
            // Clamp to max duty (CCR0)
            if (scaled > TB1CCR0) {
                scaled = TB1CCR0;
            }
            
            TB1CCR1 = scaled;                  // update PWM duty cycle
            break;
        }
        default:
            break;
    }
}

/* ===================== Main ===================== */
void main(void) {
    WDTCTL = WDTPW | WDTHOLD;                  // stop watchdog timer

    init_gpio();
    init_adc();
    init_timer_b1_pwm();

    // Calibrate baseline with board flat before starting sampling
    calibrate_baseline();

    // Initialize circular buffer with baseline
    uint8_t i;
    for (i = 0; i < BUF_SIZE; i++) {
        xbuf[i] = baseline;
    }
    sum = baseline << 4;                       // sum = baseline * 16 (shift left 4)
    avg = baseline;

    init_timer_a0();                           // start 100 Hz timer (triggers ADC)

    __bis_SR_register(GIE);                    // enable global interrupts

    while (1) {
        __no_operation();                      // CPU idle; interrupts handle everything
    }
}
