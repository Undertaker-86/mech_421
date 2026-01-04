#include "driverlib.h"
#include "in430.h"
#include "msp430fr5739.h"

// DEFINE CONSTANTS --------------------------------------------------------------------------------------------
#define QUEUE_SIZE 50

// DECLARE FUNCTIONS -------------------------------------------------------------------------------------------
void setupClocks();
void setupUART();
void setupTimerBOut();
void setupLEDs();
void setupDCDriver();

unsigned int isQueueFull();
unsigned int isQueueEmpty();
void enqueue(unsigned char newItem);
unsigned char dequeue();
void printError(const char* message);

void updateTimerB();
void processInstructions(); // Typo fixed: Intructions -> Instructions

// DECLARE GLOBAL VARIABLES ------------------------------------------------------------------------------------
volatile unsigned int test = 0;
volatile unsigned char dataReceived;       // Store received data
volatile unsigned char dataDequeued;       // Store dequeued data
volatile unsigned char queue[QUEUE_SIZE];  // Queue
volatile unsigned int front = 0;           // CPSC 259 circular queue algorithm
volatile unsigned int numItems = 0;        // CPSC 259 circular queue algorithm

// Store instructions
volatile unsigned char startByte;    // Indicates start of instructions
volatile unsigned char commandByte;  // Indicates task
volatile unsigned char dataByte1;    // Used to set period/duty cycle (combined with dataByte2)
volatile unsigned char dataByte2;    // Used to set period/duty cycle (combined with dataByte1)
volatile unsigned char escapeByte;   // Determine if either dataByte1 or dataByte2 need to be changed to 255
volatile unsigned int combinedBytes; // Store combined value of dataByte1 and dataByte2

// MAIN CODE LOGIC ---------------------------------------------------------------------------------------------
void main(void)
{
    // Stop watchdog timer
    WDTCTL = WDTPW | WDTHOLD;

    // Setup registers
    setupClocks();
    setupUART();
    setupTimerBOut();
    setupLEDs();
    setupDCDriver();

    _EINT(); // Enable global interrupts

    // Ideally, processInstructions() should be called here in a loop, not in the ISR
    while(1)
    {
        // Main loop
    }
}

// REGISTER SETUP ---------------------------------------------------------------------------------------------

// Setup clocks
void setupClocks()
{
    CSCTL0 = CSKEY;                                      // Unlock clocks for configuration
    CSCTL1 = DCORSEL | DCOFSEL_3;                        // Set DCO range, Set DCO frequency 24MHz
    CSCTL2 = SELA__DCOCLK | SELS__DCOCLK | SELM__DCOCLK; // ACLK=DCO, SMCLK=DCO, MCLK=DCO
    CSCTL3 = DIVA__2 | DIVS__1 | DIVM_0;                 // ACLK div by 2 = 12MHz, SMCLK div by 0, MCLK div by 1
    CSCTL0_H = 0;                                        // Lock clocks
}

// Configure UART
void setupUART()
{
    // Set P2.0 and P2.1 for UART
    P2SEL1 |= BIT5 | BIT6;
    P2SEL0 &= ~(BIT5 | BIT6);

    // Configure UART
    UCA1CTLW0 = UCSWRST;                      // Reset mode for UART configuration
    UCA1CTLW0 &= ~(UCPEN | UC7BIT | UCSPB);   // Parity disabled, 8 bit data, 1 stop bit
    UCA1CTLW0 |= UCSSEL__ACLK;                // SRC=ACLK

    // Set baud rate to 9600 w/ 12MHz (approx) ACLK - Check datasheet calculations
    UCA1BRW = 78;
    UCA1MCTLW |= UCOS16 | UCBRF_2 | 0x0000;
    UCA1CTLW0 &= ~UCSWRST;                    // Lock UART

    // Set UART interrupts
    UCA1IE |= UCRXIE;                         // Enable receive interrupt
}

// Setup timer B to operate in continuous mode
// FIXED: Changed to Timer B1 to match P3.4 pin output
void setupTimerBOut()
{
    // Configure timer TB1.x
    TB1CTL = TBCLR; // Clear TB1 before configuring
    TB1CTL |= (TBSSEL__SMCLK | ID__1 | MC__CONTINUOUS); // SRC=SMCLK, Div 1, Continuous mode

    // Configure TB1.1 as output (Matches P3.4)
    TB1CCTL1 |= OUTMOD_7; // Set output mode 7 (reset/set)

    // Set the initial duty cycle
    TB1CCR1 = 32768;      // 50% duty cycle

    // Set P3.4 as TB1.1 output (PWM signal)
    P3DIR |= BIT4;        // Set P3.4 as output
    P3SEL1 &= ~BIT4;      // Select Timer_B function for P3.4 (Check specific datasheet for SEL0/1 combo)
    P3SEL0 |= BIT4;
}

// Setup LEDs
void setupLEDs()
{
    // Set PJ.0 as output
    PJDIR |= (BIT0);
    PJSEL1 &= ~(BIT0);
    PJSEL0 &= ~(BIT0);

    // Turn off LED 1 initially
    PJOUT = 0;
}

// Setup driver
void setupDCDriver()
{
    // Set P3.6 and P3.7 as output
    P3DIR |= (BIT6 | BIT7);
    P3SEL1 &= ~(BIT6 | BIT7);
    P3SEL0 &= ~(BIT6 | BIT7);
}

// OPERATIONS -------------------------------------------------------------------------------------------------

// Check if buffer is full
unsigned int isQueueFull()
{
    if (numItems == QUEUE_SIZE)
        return 1;
    else
        return 0;
}

// Check if buffer is empty
unsigned int isQueueEmpty()
{
    if (numItems == 0)
        return 1;
    else
        return 0;
}

// Enqueue data to circular queue
void enqueue(unsigned char newItem)
{
    queue[(front + numItems) % QUEUE_SIZE] = newItem;
    numItems++;
}

// Dequeue data from circular queue
unsigned char dequeue()
{
    unsigned char returnVal;

    returnVal = queue[front];
    front = (front + 1) % QUEUE_SIZE;
    numItems--;

    return returnVal;
}

// Print error message
void printError(const char* message)
{
    // Print each character from string onto serial port
    while(*message)
    {
        while(!(UCA1IFG & UCTXIFG));
        UCA1TXBUF = *message++;
    }
}

// Change period and duty cycle of timer B
// FIXED: Updated to write to TB1 register
void updateTimerB()
{
    TB1CCR1 = combinedBytes;
}

// Process instructions
void processInstructions()
{
    // Get received instructions from queue
    startByte = dequeue();
    commandByte = dequeue();
    dataByte1 = dequeue();
    dataByte2 = dequeue();
    escapeByte = dequeue();

    // If start byte is 255, process instructions
    if (startByte == 255)
    {
        // Handle escape bytes
        if (escapeByte & BIT0)
            dataByte2 = 255;
        if (escapeByte & BIT1)
            dataByte1 = 255;

        // Combine dataByte1 and dataByte2
        combinedBytes = (dataByte1 << 8) | dataByte2;

        // Execute task based on command byte
        if (commandByte == 1) // Motor CCW
        {
            updateTimerB();
            P3OUT = (P3OUT & ~BIT7) | BIT6;
        }
        else if (commandByte == 2) // Motor CW
        {
            updateTimerB();
            P3OUT = BIT7 | (P3OUT & ~BIT6);
        }
    }
}

// INTERRUPT SERVICE ROUTINES ---------------------------------------------------------------------------------

// Interrupt if data is received by UART
#pragma vector = USCI_A1_VECTOR
__interrupt void queueData()
{
    // Check correct flag has been set
    if (UCA1IFG & UCRXIFG)
    {
        dataReceived = UCA1RXBUF; // Data received via UART

        // Enqueue data
        if (!isQueueFull())
        {
            enqueue(dataReceived);
        }
        else
        {
            // Print error if queue is full
            printError("ERROR: Queue is FULL!");
        }

        // Process the instructions once the full message has been received
        if (numItems == 5)
            processInstructions(); // Typo fixed here

        UCA1IFG &= ~UCRXIFG; // Clear interrupt flag
    }
}