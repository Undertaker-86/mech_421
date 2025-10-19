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

//******************************************************************************
//!
//!   Empty Project that includes driverlib
//!
//******************************************************************************
void main (void)
{
    WDTCTL = WDTPW | WDTHOLD;

    // --- GPIO Config ---

    // P3.6 and P3.7 as output
    P3DIR |= BIT6 | BIT7;
    P3OUT &= ~(BIT6 | BIT7);

    // P4.0 and P4.1 as input
    P4DIR &= ~(BIT0 | BIT1);

    // Pull the pull up resistor for S1 and S2
    P4REN |= BIT0 | BIT1; // Enable resistor for P4.0 and P4.1
    P4OUT |= BIT0 | BIT1; // Resistor to pull-up mode

    // Rising edge detection
    P4IES &= ~(BIT0 | BIT1);

    // clear interrupt flags for P4.0 and P4.1
    P4IFG &= ~(BIT0 | BIT1);

    // Enable local interrupt for P4.0 and P4.1
    P4IE |= BIT0 | BIT1;

    // Lock GPIO config
    PM5CTL0 &= ~LOCKLPM5;

    // Enable global interrupt
    __bis_SR_register(GIE);

    // Enter low-power mode. CPU activates only for interrupts
    while(1)
    {
        __bis_SR_register(LPM0_bits);
    }
}

#pragma vector=PORT4_VECTOR
__interrupt void Port_4_ISR(void)
{
    switch(__even_in_range(P4IV, P4IV_P4IFG7))
    {
        case P4IV_NONE:
            break; // no interrupt
        case P4IV_P4IFG0:
            P3OUT ^= BIT6; // Toggle LED7
            break;
        case P4IV_P4IFG1:
            P3OUT ^= BIT7;
            break;
        default:
            break;
    }
}