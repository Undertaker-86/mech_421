#include <msp430.h>

/**
 * main.c
 * Exercise 4: UART
 * Hardware connections:
 * - P2.0 (UCA0TXD) -> To PC's UART RX
 * - P2.1 (UCA0RXD) -> To PC's UART TX
 * - PJ.0 -> LED1 on Experimenter Board
 */



void main(void)
{
    // Stop watchdog timer
    WDTCTL = WDTPW | WDTHOLD;

    // --- GPIO Configuration ---

    // 8. Configure LED1 (PJ.0) as an output
    PJSEL0 &= ~BIT0;            // Route PJ.0 to GPIO function
    PJSEL1 &= ~BIT0;
    PJDIR |= BIT0;              // Set PJ.0 to output direction
    PJOUT &= ~BIT0;             // Ensure LED1 is initially off

    // 2. Set up P2.0 and P2.1 for UART communications.
    // P2.0 is UCA0TXD, P2.1 is UCA0RXD
    P2SEL1 |= BIT0 | BIT1;      // Set P2.0 and P2.1 to secondary peripheral function (UART)
    P2SEL0 &= ~(BIT0 | BIT1);   //

    // Disable the GPIO power-on default high-impedance mode to activate
    // previously configured port settings
    PM5CTL0 &= ~LOCKLPM5;

    // --- UART Configuration (eUSCI_A0) ---

    // 1. Configure the UART to operate at 9600, 8, N, 1.
    // Put eUSCI in reset
    UCA0CTL1 |= UCSWRST;

    // Set clock source to SMCLK (Sub-Main Clock), typically 1MHz by default
    UCA0CTL1 |= UCSSEL__SMCLK;

    // Baud Rate calculation for 9600 from 1MHz SMCLK
    // N = F_clock / Baudrate = 1,000,000 / 9600 = 104.1667
    // Use oversampling mode (UCOS16 = 1)
    // UCBRx = INT(N/16) = INT(104.1667/16) = INT(6.51) = 6
    // UCBRFx = round(( (N/16) - INT(N/16) ) * 16) = round(0.51 * 16) = 8
    // UCBRSx value from User Guide Table for fraction 0.1667 is 0x20
    UCA0BRW = 6;  // Set clock prescaler in the Baud Rate Word Register
    UCA0MCTLW = UCOS16 | UCBRF_8 | 0x2000; // Modulation control register

    // Initialize eUSCI
    UCA0CTL1 &= ~UCSWRST;

    // 5. Enable UART receive interrupt.
    UCA0IE |= UCRXIE;           // Enable USCI_A0 RX interrupt

    // 3. Code to periodically transmit the letter 'a'.
    // NOTE: This section is commented out because the final requirements
    // are based on an interactive echo, which conflicts with this.
    // To test this part, uncomment the while loop and comment out the
    // global interrupt enable and the final LPM loop.
    /*
    while(1)
    {
        // Wait until the transmit buffer is ready
        while (!(UCA0IFG & UCTXIFG));
        // Transmit the character
        UCA0TXBUF = 'a';
        // Add a delay
        __delay_cycles(100000); // Delay for ~100ms at 1MHz
    }
    */

    // 5. Enable global interrupt.
    __bis_SR_register(GIE);     // Enable General Interrupts

    // Enter Low Power Mode 0 with interrupts enabled.
    // The CPU will sleep until a character is received.
    __bis_SR_register(LPM0_bits);
}


// 6. Set up an interrupt service routine for UART receive
#pragma vector=USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void)
{
    char received_char;

    // Switch on the interrupt vector register
    switch(__even_in_range(UCA0IV, USCI_UART_UCTXCPTIFG))
    {
        case USCI_NONE: break;
        case USCI_UART_UCRXIFG:
            // Read the received byte from the buffer. This also clears the RX flag.
            received_char = UCA0RXBUF;

            // 8. Add code to turn on/off LED1
            if (received_char == 'j')
            {
                PJOUT |= BIT0;      // Turn on LED1
            }
            else if (received_char == 'k')
            {
                PJOUT &= ~BIT0;     // Turn off LED1
            }

            // 6. Echo the received byte back to the serial port.
            // Wait for the transmit buffer to be empty
            while (!(UCA0IFG & UCTXIFG));
            UCA0TXBUF = received_char;

            // 7. Transmit the next byte in the ASCII table.
            // Wait for the transmit buffer to be empty again
            while (!(UCA0IFG & UCTXIFG));
            UCA0TXBUF = received_char + 1;

            break;
        case USCI_UART_UCTXIFG: break;
        default: break;
    }
}