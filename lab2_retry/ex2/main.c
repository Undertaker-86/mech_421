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
    PM5CTL0 = ~LOCKLPM5;

    PJDIR |= BIT0 | BIT1 | BIT2 | BIT3;
    P3DIR |= BIT4 | BIT5 | BIT6 | BIT7;

    PJSEL0 &= ~(BIT0 | BIT1 | BIT2 | BIT3);
    PJSEL1 &= ~(BIT0 | BIT1 | BIT2 | BIT3);

    P3SEL0 &= ~(BIT4 | BIT5 | BIT6 | BIT7);
    P3SEL1 &= ~(BIT4 | BIT5 | BIT6 | BIT7);

    PJOUT |= BIT0 | BIT3;
    PJOUT &= ~(BIT1 | BIT2);

    P3OUT |= BIT6 | BIT7;
    P3OUT &= ~(BIT4 | BIT5);

    while(1)
    {
        PJOUT ^= (BIT1 | BIT2);
        P3OUT ^= (BIT4 | BIT5);

        __delay_cycles(25e5);
    }
}
