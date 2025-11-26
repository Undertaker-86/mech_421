#include <msp430.h>
#include <stdint.h>

// ==================== FUNCTION PROTOTYPES ====================
void clockSetup(void);
void timerA1Setup(void);
void TimerBSetup(void);
void UARTSetup(void);
void UARTTx(char TxByte);
char UARTRx(void);
void step_motor(uint8_t step);
unsigned int isQueueFull(void);
unsigned int isQueueEmpty(void);
void enqueue(unsigned char newItem);
unsigned char dequeue(void);
void printError(const char *message);
void processInstructions(void);

// ==================== PIN DEFINES ====================
// Queue
#define QUEUE_SIZE 50

// Stepper coils (through driver, NOT directly to motor!)
#define COIL_A1 BIT5   // P1.5 -> TB0.2
#define COIL_A2 BIT4   // P1.4 -> TB0.1
#define COIL_B1 BIT5   // P3.5 -> TB1.2
#define COIL_B2 BIT4   // P3.4 -> TB1.1

// ==================== GLOBALS ====================
volatile unsigned char dataReceived;
volatile unsigned char queue[QUEUE_SIZE];
volatile unsigned int front   = 0;
volatile unsigned int numItems = 0;

// Incoming packet bytes
volatile unsigned char startByte;   // 255 = start
volatile unsigned char motorByte;   // (currently unused)
volatile unsigned char DirnByte;    // direction / command
volatile unsigned char dataByte1;   // high byte
volatile unsigned char dataByte2;   // low byte
volatile unsigned char escapeByte;  // escape bits
volatile unsigned int  combinedBytes;

volatile uint8_t state = 0;         // current step index 0..7

// Half-step lookup table: bit0=A1, bit1=A2, bit2=B1, bit3=B2
static const uint8_t stepper_table[8] = {
    0b0001, // Step 1: A1
    0b0101, // Step 2: A1 + B1
    0b0100, // Step 3: B1
    0b0110, // Step 4: B1 + A2
    0b0010, // Step 5: A2
    0b1010, // Step 6: A2 + B2
    0b1000, // Step 7: B2
    0b1001  // Step 8: B2 + A1
};

// ==================== CLOCK SETUP (8 MHz DCO) ====================
void clockSetup(void)
{
    WDTCTL = WDTPW | WDTHOLD;      // Stop watchdog

    // Unlock clock system
    CSCTL0_H = CSKEY_H;

    // DCO = 8 MHz
    CSCTL1 = DCOFSEL_3;                         // 8 MHz
    CSCTL2 = SELS__DCOCLK | SELA__DCOCLK | SELM__DCOCLK; // SMCLK, ACLK, MCLK = DCO
    CSCTL3 = DIVA__1 | DIVS__1 | DIVM__1;       // no dividers

    CSCTL0_H = 0;               // lock clock system
}

// ==================== TIMER A1 – STEP CLOCK ====================
// SMCLK (8 MHz) /8 → 1 MHz timer clock
// TA1CCR0 sets step rate: f_step = 1 MHz / (CCR0+1)
void timerA1Setup(void)
{
    TA1CTL = TASSEL__SMCLK | ID_3 | MC__UP | TACLR; // SMCLK, /8, up mode, clear
    TA1CCR0 = 1000;          // default step frequency = 1 kHz
    TA1CCTL0 = CCIE;         // enable CCR0 interrupt
}

// ==================== TIMER B0/B1 – PWM FOR COILS ====================
void TimerBSetup(void)
{
    // Timer B0: P1.4 (TB0.1, A2) and P1.5 (TB0.2, A1)
    TB0CCR0 = 1000 - 1;      // PWM period (adjust as needed)

    TB0CCTL1 = OUTMOD_7;     // reset/set
    TB0CCR1  = 250;          // 25% duty (A2)

    TB0CCTL2 = OUTMOD_7;     // reset/set
    TB0CCR2  = 250;          // 25% duty (A1)

    TB0CTL = TBSSEL__SMCLK | MC__UP | TBCLR; // SMCLK, up mode, clear

    // Timer B1: P3.4 (TB1.1, B2) and P3.5 (TB1.2, B1)
    TB1CCR0 = 1000 - 1;

    TB1CCTL1 = OUTMOD_7;     // reset/set
    TB1CCR1  = 250;          // 25% duty (B2)

    TB1CCTL2 = OUTMOD_7;     // reset/set
    TB1CCR2  = 250;          // 25% duty (B1)

    TB1CTL = TBSSEL__SMCLK | MC__UP | TBCLR;

    // PWM outputs pins
    // A coils on P1.4, P1.5
    P1DIR  |= COIL_A1 | COIL_A2;
    P1SEL0 |= COIL_A1 | COIL_A2;
    P1SEL1 &= ~(COIL_A1 | COIL_A2);

    // B coils on P3.4, P3.5
    P3DIR  |= COIL_B1 | COIL_B2;
    P3SEL0 |= COIL_B1 | COIL_B2;
    P3SEL1 &= ~(COIL_B1 | COIL_B2);
}

// ==================== UART A1 SETUP (9600 baud) ====================
void UARTSetup(void)
{
    UCA1CTLW0 |= UCSWRST;           // hold in reset
    UCA1CTLW0 |= UCSSEL__ACLK;      // use ACLK (you set ACLK=DCO above)

    // Baud settings for 8 MHz ACLK, 9600 baud (from your lab table)
    UCA1MCTLW |= UCOS16;
    UCA1MCTLW |= UCBRF0;
    UCA1MCTLW |= 0x4900;            // UCBRSx bits
    UCA1BRW   = 52;

    UCA1CTLW0 &= ~UCSWRST;          // release reset
    UCA1IE    |= UCRXIE;            // enable RX interrupt

    // P2.5 (RX) & P2.6 (TX) UART function
    P2SEL1 |= (BIT5 | BIT6);
    P2SEL0 &= ~(BIT5 | BIT6);
}

// ==================== UART HELPERS ====================
void UARTTx(char TxByte)
{
    while (!(UCA1IFG & UCTXIFG));   // wait for TX buffer empty
    UCA1TXBUF = TxByte;
}

char UARTRx(void)
{
    while (!(UCA1IFG & UCRXIFG));   // wait for RX
    return UCA1RXBUF;
}

// ==================== QUEUE HELPERS ====================
unsigned int isQueueFull(void)
{
    return (numItems == QUEUE_SIZE);
}

unsigned int isQueueEmpty(void)
{
    return (numItems == 0);
}

void enqueue(unsigned char newItem)
{
    queue[(front + numItems) % QUEUE_SIZE] = newItem;
    numItems++;
}

unsigned char dequeue(void)
{
    unsigned char returnVal = queue[front];
    front = (front + 1) % QUEUE_SIZE;
    numItems--;
    return returnVal;
}

void printError(const char *message)
{
    while (*message)
    {
        while (!(UCA1IFG & UCTXIFG));
        UCA1TXBUF = *message++;
    }
}

// ==================== PROCESS INSTRUCTIONS FROM UART ====================
void processInstructions(void)
{
    // Expected packet: [start][Dirn][data1][data2][escape]
    startByte  = dequeue();
    // motorByte = dequeue();    // not used right now
    DirnByte   = dequeue();
    dataByte1  = dequeue();
    dataByte2  = dequeue();
    escapeByte = dequeue();

    if (startByte == 255)
    {
        // Escape bits: bit0 -> data2=255, bit1 -> data1=255
        if (escapeByte & BIT0) dataByte2 = 255;
        if (escapeByte & BIT1) dataByte1 = 255;

        combinedBytes = ((unsigned int)dataByte1 << 8) | dataByte2;

        // Command 1: continuous CCW
        if (DirnByte == 1)
        {
            TA1CTL = (TA1CTL & ~MC_3) | MC__UP;  // ensure up mode
            TA1CCR0 = combinedBytes;            // step speed
            state   = (state == 0) ? 7 : state - 1;
        }
        // Command 2: continuous CW
        else if (DirnByte == 2)
        {
            TA1CTL = (TA1CTL & ~MC_3) | MC__UP;
            TA1CCR0 = combinedBytes;
            state   = (state + 1) & 0x07;
        }
        // Command 3: single CCW step
        else if (DirnByte == 3)
        {
            TA1CTL = (TA1CTL & ~MC_3) | MC__STOP;
            state = (state == 0) ? 7 : state - 1;
            step_motor(state);
        }
        // Command 4: single CW step
        else if (DirnByte == 4)
        {
            TA1CTL = (TA1CTL & ~MC_3) | MC__STOP;
            state = (state + 1) & 0x07;
            step_motor(state);
        }
    }
}

// ==================== STEPPER OUTPUT ====================
void step_motor(uint8_t step)
{
    // A1 (P1.5 / TB0.2)
    if (stepper_table[step] & 0x01)
        TB0CCR2 = 250;
    else
        TB0CCR2 = 0;

    // A2 (P1.4 / TB0.1)
    if (stepper_table[step] & 0x02)
        TB0CCR1 = 250;
    else
        TB0CCR1 = 0;

    // B1 (P3.5 / TB1.2)
    if (stepper_table[step] & 0x04)
        TB1CCR2 = 250;
    else
        TB1CCR2 = 0;

    // B2 (P3.4 / TB1.1)
    if (stepper_table[step] & 0x08)
        TB1CCR1 = 250;
    else
        TB1CCR1 = 0;
}

// ==================== MAIN ====================
int main(void)
{
    clockSetup();
    timerA1Setup();
    TimerBSetup();
    UARTSetup();

    // FRAM devices: unlock GPIO (very important!)
    PM5CTL0 &= ~LOCKLPM5;   // enable port pins:contentReference[oaicite:0]{index=0}

    // Just in case, make coils outputs and start disabled
    P1DIR |= COIL_A1 | COIL_A2;
    P3DIR |= COIL_B1 | COIL_B2;
    P1OUT &= ~(COIL_A1 | COIL_A2);
    P3OUT &= ~(COIL_B1 | COIL_B2);

    __enable_interrupt();   // global interrupt enable

    while (1)
    {
        // Main loop idle – everything happens in ISRs
        __no_operation();
    }
}

// ==================== ISRs ====================

// Timer A1 CCR0 ISR – step clock
#pragma vector = TIMER1_A0_VECTOR
__interrupt void Timer_A1_ISR(void)
{
    step_motor(state);      // output for current step

    // update state based on DirnByte (1=CCW, 2=CW)
    if (DirnByte == 1)
        state = (state + 1) & 0x07;
    else if (DirnByte == 2)
        state = (state == 0) ? 7 : state - 1;

    TA1CCTL0 &= ~CCIFG;     // clear flag (usually auto-cleared anyway)
}

// UART A1 ISR – enqueue received bytes
#pragma vector = USCI_A1_VECTOR
__interrupt void USCI_A1_ISR(void)
{
    if (UCA1IFG & UCRXIFG)
    {
        dataReceived = UARTRx();  // fetch received byte

        if (!isQueueFull())
        {
            enqueue(dataReceived);
        }
        else
        {
            printError("ERROR: Queue is FULL!\r\n");
        }

        // 5 bytes per packet: [start][Dirn][d1][d2][escape]
        if (numItems >= 5)
        {
            processInstructions();
        }

        UCA1IFG &= ~UCRXIFG;     // clear RX flag
    }
}
