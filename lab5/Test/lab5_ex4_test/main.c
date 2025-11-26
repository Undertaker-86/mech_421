// MECH 421 2025W - Lab 5 Assignment 1 Test Software
// Sam Berryman (former TA, original author)
// Erik Lamoureux and Yifei Liu (TAs)

#include <msp430.h>

volatile unsigned int position = 0;
void testOutputs(int position);
void changePosition(void);

void main(void)
{
    WDTCTL = WDTPW | WDTHOLD;                               // Stop watchdog timer
    CSCTL0_H = 0xA5;                                        //Need this to change all the clock stuff (Password key)
    CSCTL1 &= ~DCORSEL;                                     //Set DCO to Low Freq
    CSCTL1 |= DCOFSEL_3;                                    //Set DCO to 8MHz

    //Set Up Clock
    CSCTL2 = SELS__DCOCLK;                                  //Set SMCLK to DCO (24MHz)
    CSCTL3 = DIVS__32;                                      //SMCLK = SMCLK/32 (0.25MHz)
    CSCTL4 &= ~SMCLKOFF;                                    //SMCLK = ON

    //Set up Timer
    TA0CTL = TASSEL__SMCLK + MC__UP + ID__8;                //Set SMCLK, Up Count, CLK/8 (31.25kHz), and interrupt enable
    TA0CCR0 = 15625-1;                                      //Interrupt every TACCR0/31.25kHz=500ms
    TA0CCTL0 |= CCIE;

    //Set up I/O
    P1DIR = 0xFF;
    P1SEL0 = 0;
    P1SEL1 = 0;
    P2DIR = 0xFF;
    P2SEL0 = 0;
    P2SEL1 = 0;
    P3DIR = 0xFF;
    P3SEL0 = 0;
    P3SEL1 = 0;
    PJDIR = 0x0F;
    PJSEL0 = 0;
    PJSEL1 = 0;

    _EINT();
}

#pragma vector = TIMER0_A0_VECTOR
__interrupt void TIMER0_A0_ISR()
{
   testOutputs(position);
   changePosition();
}

void changePosition(void)
{
    if(position == 27)
        position = 0;
    else
        position++;
}

void testOutputs(int position)
{
    //    ___________________________________
    // <-| PJ.4                         AVSS |->
    // <-| PJ.5                         P2.4 |->
    // <-| AVSS                         P2.3 |->
    // <-| AVCC                         P2.7 |->
    // <-| P1.0                         DVCC |->
    // <-| P1.1                         DVSS |->
    // <-| P1.2                        VCORE |->
    // <-| P3.0                         P1.7 |->
    // <-| P3.1                         P1.6 |->
    // <-| P3.2                         P3.7 |->
    // <-| P3.3                         P3.6 |->
    // <-| P1.3                         P3.5 |->
    // <-| P1.4                         P3.4 |->
    // <-| P1.5                         P2.2 |->
    // <-| PJ.0                         P2.1 |->
    // <-| PJ.1                         P2.0 |->
    // <-| PJ.2                      SBWTDIO |->
    // <-| PJ.3                       SBWTCK |->
    // <-| P2.5                         P2.6 |->
    //    -----------------------------------

    P1OUT = 0;
    P2OUT = 0;
    P3OUT = 0;
    PJOUT = 0;

    switch(position)
    {

        case 0:
        P2OUT = BIT4;
        break;

        case 1:
        P2OUT = BIT3;
        break;

        case 2:
        P2OUT = BIT7;
        break;

        case 3:
        P1OUT = BIT7;
        break;

        case 4:
        P1OUT = BIT6;
        break;

        case 5:
        P3OUT = BIT7;
        break;

        case 6:
        P3OUT = BIT6;
        break;

        case 7:
        P3OUT = BIT5;
        break;

        case 8:
        P3OUT = BIT4;
        break;

        case 9:
        P2OUT = BIT2;
        break;

        case 10:
        P2OUT = BIT1;
        break;

        case 11:
        P2OUT = BIT0;
        break;

        case 12:
        P2OUT = BIT6;
        break;

        case 13:
        P2OUT = BIT5;
        break;

        case 14:
        PJOUT = BIT3;
        break;

        case 15:
        PJOUT = BIT2;
        break;

        case 16:
        PJOUT = BIT1;
        break;

        case 17:
        PJOUT = BIT0;
        break;

        case 18:
        P1OUT = BIT5;
        break;

        case 19:
        P1OUT = BIT4;
        break;

        case 20:
        P1OUT = BIT3;
        break;

        case 21:
        P3OUT = BIT3;
        break;

        case 22:
        P3OUT = BIT2;
        break;

        case 23:
        P3OUT = BIT1;
        break;

        case 24:
        P3OUT = BIT0;
        break;

        case 25:
        P1OUT = BIT2;
        break;

        case 26:
        P1OUT = BIT1;
        break;

        case 27:
        P1OUT = BIT0;
        break;

    }
}
