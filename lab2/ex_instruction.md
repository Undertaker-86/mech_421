# Lab Exercises and Code Solution

## Exercise 1: Clock Configuration

1. Set up SMCLK to run on the DCO.
2. Configure the DCO to run at 8 MHz.
3. Set up SMCLK with a divider of 32.
4. Configure P3.4 as an output and route SMCLK to it. The SMCLK frequency should be ~250 kHz—verify on an oscilloscope.

**code**
```c
#include <msp430.h>
void main (void)
{
    WDTCTL = WDTPW | WDTHOLD;

    CSCTL0_H = CSKEY_H;
    CSCTL1 = DCOFSEL_3;
    CSCTL2 = SELM__DCOCLK | SELS__DCOCLK | SELA__VLOCLK;
    CSCTL3 = DIVS__1 | DIVS__32 | DIVS__1;
    CSCTL0_H = 0;

    P3DIR |= BIT4;
    P3SEL0 |= BIT4;
    P3SEL1 |= BIT4;
    PM5CTL0 &= ~LOCKLPM5;

    while (1) {
        __no_operation();
    }
}
```

## Exercise 2: Digital I/O

1. Configure PJ.0, PJ.1, PJ.2, PJ.3, P3.4, P3.5, P3.6, and P3.7 as digital outputs.
2. Set LEDs 1–8 to output `10010011`.
3. Write a program that blinks the LEDs that are `0` in step 2, using a visible delay in the main loop.
**code**
```c
#include <msp430.h>

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

```
## Exercise 3: Interrupts

1. Configure P4.0 and P4.1 as digital inputs.
2. The switches S1 and S2 connect to P4.0 and P4.1. Enable the internal pull-up resistors for both.
3. Configure interrupts on rising edges (i.e., when the user releases the button). Enable local and global interrupts.
4. Configure P3.6 (LED7) and P3.7 (LED8) as outputs.
5. Write an ISR to toggle LED7 and LED8 on rising edges from S1 and S2, respectively.
**code**
```c
#include <msp430.h>
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
```
## Exercise 4: UART

1. Configure UART for 9600, 8-N-1.
2. Set P2.0 and P2.1 for UART functionality.
3. Periodically transmit the letter `a` on the serial port.
4. Verify transmission using the CCS serial console (or PuTTY).
5. Enable UART receive interrupt and global interrupt.
6. In the RX ISR, echo the received byte back to the serial port. Verify in CCS terminal/PuTTY.
7. In addition to echoing, also transmit the next ASCII byte. Verify in PuTTY.
8. Turn on LED1 when `j` is received and turn off LED1 when `k` is received.
**code**
```c
#include <msp430.h>

/**
 * main.c
 * Exercise 4: UART
 *
 * This program configures the eUSCI_A0 module for UART communication at 9600 baud.
 * It initially transmits the character 'a' periodically. The final version has this
 * part disabled and instead enables a receive interrupt.
 *
 * The ISR handles the following:
 * 1. Echoes any received character back to the terminal.
 * 2. Transmits the next character in the ASCII table.
 * 3. Turns on LED1 (PJ.0) when 'j' is received.
 * 4. Turns off LED1 (PJ.0) when 'k' is received.
 *
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
```
## Exercise 5: Timer I

1. (Tip: see the end of the lab for help finding the correct timer interrupt vector.)
2. Set up Timer B in **up-count** mode.
3. Configure TB1.1 to produce a 500 Hz square wave. Use dividers as needed. Output on P3.4; verify on an oscilloscope. LED5 should be lit.
4. Configure TB1.2 to produce a 500 Hz square wave at 25% duty cycle. Output on P3.5; verify on an oscilloscope. LED6 should be lit and dimmer than LED5.
**code**
```c
#include <msp430.h>

/*
 * Exercise 5: Timer B outputs on LED5 (P3.4) and LED6 (P3.5).
 *
 * Timer1_B is configured in up mode using SMCLK at 1 MHz.
 * - CCR0 sets the period to 2 ms (500 Hz).
 * - CCR1 drives TB1.1 at 50% duty cycle (LED5, P3.4).
 * - CCR2 drives TB1.2 at 25% duty cycle (LED6, P3.5).
 */
int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;          // Stop watchdog

    // Route P3.4 and P3.5 to Timer_B outputs and make them outputs.
    P3DIR  |= BIT4 | BIT5;             // LED5/LED6 direction
    P3SEL0 |= BIT4 | BIT5;             // Select TB1.1 / TB1.2 function
    P3SEL1 &= ~(BIT4 | BIT5);

    PM5CTL0 &= ~LOCKLPM5;              // Enable previously configured ports

    // Timer1_B setup: SMCLK source (1 MHz), up mode, clear TAR.
    TB1CTL = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB1CCR0 = 2000 - 1;                // 1 MHz / 2000 = 500 Hz

    TB1CCTL1 = OUTMOD_7;               // Reset/set PWM mode
    TB1CCR1  = 1000;                   // 50% duty cycle (LED5 bright)

    TB1CCTL2 = OUTMOD_7;               // Reset/set PWM mode
    TB1CCR2  = 500;                    // 25% duty cycle (LED6 dimmer)

    while (1)
    {
        __no_operation();              // CPU idle; timer hardware drives outputs
    }
}

```
## Exercise 6: Timer II

1. Set up Timer A to measure pulse high time (rising-to-falling edge).
2. Physically jumper the output from Exercise 5 to this timer’s input (female-female wire).
3. Use the debugger to check the measured 16-bit value.
**code**
```c
#include <msp430.h>
#include <stdint.h>

/*
 * Exercise 6: Measure high pulse width using Timer_A.
 *
 * The square wave generated in Exercise 5 (e.g. P3.4/TB1.1) is jumper-wired
 * to P1.2, which feeds the Timer1_A CCR1 capture input.
 * Timer1_A runs from SMCLK (default ~1 MHz) in continuous mode. Each capture
 * interrupt records the time between a rising edge and the subsequent falling
 * edge. The duration in SMCLK ticks is stored in the global variable
 * pulse_ticks, which you can inspect in the debugger.
 */

#define CAPTURE_COMMON   (CCIS_0 | CAP | SCS | CCIE)
#define CAPTURE_RISING   (CM_1 | CAPTURE_COMMON)
#define CAPTURE_FALLING  (CM_2 | CAPTURE_COMMON)

volatile uint16_t rising_edge = 0;      // Timestamp of the most recent rising edge
volatile uint16_t pulse_ticks = 0;      // High-time width in SMCLK cycles
volatile uint8_t  waiting_for_fall = 0; // 0: waiting for rising, 1: waiting for falling
volatile uint8_t  measurement_ready = 0;

static void uart_init(void);
static void uart_putc(char c);
static void uart_print(const char *s);
static void uart_print_uint16(uint16_t value);
static void timerb_init(void);

int main(void)
{
    WDTCTL = WDTPW | WDTHOLD;          // Stop watchdog timer

    // Route P1.2 to Timer1_A capture input (TA1.1) and configure as input.
    P1DIR  &= ~BIT2;
    P1SEL0 |= BIT2;
    P1SEL1 &= ~BIT2;

    timerb_init();

    PM5CTL0 &= ~LOCKLPM5;              // Unlock GPIO pins

    uart_init();
    uart_print("\r\nTimer capture ready.\r\n");

    // Timer1_A: SMCLK source, continuous mode, clear TAR.
    TA1CTL = TASSEL__SMCLK | MC__CONTINUOUS | TACLR;

    // Start by watching for the rising edge of the pulse.
    TA1CCTL1 = CAPTURE_RISING;

    __bis_SR_register(GIE);            // Enable global interrupts

    while (1)
    {
        if (measurement_ready)
        {
            measurement_ready = 0;
            uart_print("pulse_ticks = ");
            uart_print_uint16(pulse_ticks);
            uart_print(" ticks\r\n");
        }
        __no_operation();
    }
}

#pragma vector = TIMER1_A1_VECTOR
__interrupt void TIMER1_A1_ISR(void)
{
    switch (__even_in_range(TA1IV, TA1IV_TAIFG))
    {
        case TA1IV_NONE:
            break;

        case TA1IV_TACCR1:
        {
            uint16_t captured = TA1CCR1;

            if (!waiting_for_fall)
            {
                rising_edge = captured;
                waiting_for_fall = 1;
                TA1CCTL1 = CAPTURE_FALLING;
            }
            else
            {
                pulse_ticks = captured - rising_edge;
                waiting_for_fall = 0;
                TA1CCTL1 = CAPTURE_RISING;
                measurement_ready = 1;
            }
            break;
        }

        default:
            break;
    }
}

static void uart_init(void)
{
    P2SEL1 |= BIT0 | BIT1;            // P2.0=TXD, P2.1=RXD
    P2SEL0 &= ~(BIT0 | BIT1);

    UCA0CTL1 |= UCSWRST;             // Hold USCI in reset
    UCA0CTL1 |= UCSSEL__SMCLK;       // SMCLK source (1 MHz default)

    UCA0BRW = 6;                     // 1 MHz / 16 / 9600 = 6.xx
    UCA0MCTLW = UCOS16 | UCBRF_8 | 0x2000;

    UCA0CTL1 &= ~UCSWRST;            // Release for operation
}

static void uart_putc(char c)
{
    while (!(UCA0IFG & UCTXIFG));
    UCA0TXBUF = c;
}

static void uart_print(const char *s)
{
    while (*s)
    {
        uart_putc(*s++);
    }
}

static void uart_print_uint16(uint16_t value)
{
    char buf[6];
    int i = 0;

    if (value == 0)
    {
        uart_putc('0');
        return;
    }

    while (value > 0 && i < 5)
    {
        buf[i++] = '0' + (value % 10);
        value /= 10;
    }

    while (i > 0)
    {
        uart_putc(buf[--i]);
    }
}

static void timerb_init(void)
{
    // Configure P3.4 (TB1.1) and P3.5 (TB1.2) for PWM output to generate the test pulse.
    P3DIR  |= BIT4 | BIT5;
    P3SEL0 |= BIT4 | BIT5;
    P3SEL1 &= ~(BIT4 | BIT5);

    // Timer1_B setup: SMCLK @ 1 MHz, up mode; CCR0 sets a 2 ms period (500 Hz).
    TB1CTL  = TBSSEL__SMCLK | MC__UP | TBCLR;
    TB1CCR0 = 2000 - 1;

    TB1CCTL1 = OUTMOD_7;               // Reset/set mode -> 50% duty cycle on P3.4
    TB1CCR1  = 1000;

    TB1CCTL2 = OUTMOD_7;               // Reset/set mode -> 25% duty cycle on P3.5
    TB1CCR2  = 500;
}
```
## Exercise 7: ADC I

1. Drive P2.7 high to power the accelerometer.
2. Configure the ADC to sample A12, A13, and A14.
3. Sample all three accelerometer axes.
4. Right-shift each 10-bit result to obtain 8-bit values.
5. Set a timer interrupt to trigger every 10 ms (100 Hz).
6. In the timer ISR, transmit a packet over UART with start byte 255: `255, X, Y, Z`. Verify using CCS Terminal/PuTTY/MECH 423 Serial Communicator.
7. Sample the ADC inside the timer ISR and transmit. Check the 100 Hz transmission rate by probing UART TX (P3.4) with an oscilloscope.
8. Test MSP430 code using your Lab 1 C# program.
**code**
```c
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

```
## Exercise 8: ADC II

1. Configure the ADC to sample the NTC temperature sensor (ensure P2.7 powers the NTC).
2. Sample the NTC; right-shift the 10-bit result to 8-bit.
3. Transmit results via UART. With a finger on the NTC, observe the value range in CCS Terminal/PuTTY/MECH 423 Serial Communicator.

   * Values decrease as temperature increases (NTC).
   * Avoid injecting noise by touching the board; insulate with thin plastic/tape.
4. Turn the EXP board into a digital thermometer using LED1–LED8 as readouts. At room temperature, only LED1 should be lit (LED1+LED2 also acceptable). More LEDs should light as temperature rises; touching the NTC should light all LEDs.
5. Test: hold a finger on the NTC for 15 s, then release. All LEDs should light, then turn off one by one. Demonstrate to a TA.
**code**
```c
#include <msp430.h>
#include <stdint.h>

/* ---------------- UART helpers (polling) ---------------- */
static void uart_putc(char c) {
    while (!(UCA0IFG & UCTXIFG)) ;   // wait for TX buffer ready
    UCA0TXBUF = c;
}
static void uart_puts(const char *s) {
    while (*s) uart_putc(*s++);
}
static void uart_putu8(uint8_t v) {   // print fixed 3 digits (000..255)
    char out[4];
    uint8_t t = v;
    out[0] = '0' + (t / 100); t %= 100;
    out[1] = '0' + (t / 10);
    out[2] = '0' + (t % 10);
    out[3] = 0;
    uart_puts(out);
}

/* ---------------- LED helpers ---------------- */
static inline void leds_all_off(void) {
    P3OUT &= ~(BIT4 | BIT5 | BIT6 | BIT7);
    PJOUT &= ~(BIT0 | BIT1 | BIT2 | BIT3);
}
static void leds_set_level(uint8_t lvl) {
    if (lvl > 8) lvl = 8;
    leds_all_off();
    if (lvl >= 1) PJOUT |= BIT0;  // LED1
    if (lvl >= 2) PJOUT |= BIT1;  // LED2
    if (lvl >= 3) PJOUT |= BIT2;  // LED3
    if (lvl >= 4) PJOUT |= BIT3;  // LED4
    if (lvl >= 5) P3OUT |= BIT4;  // LED5
    if (lvl >= 6) P3OUT |= BIT5;  // LED6
    if (lvl >= 7) P3OUT |= BIT6;  // LED7
    if (lvl >= 8) P3OUT |= BIT7;  // LED8
}

/* ---------------- Tiny delay ---------------- */
static void delay_cycles_coarse(volatile unsigned long n) {
    while (n--) __no_operation();
}

/* ---------------- Init: GPIO / NTC power / LEDs ---------------- */
static void init_gpio(void) {
    /* Power the NTC divider from P2.7 (drive high) */
    P2DIR |= BIT7;     // output
    P2OUT |= BIT7;     // high (3.3V)
    P2SEL0 &= ~BIT7;
    P2SEL1 &= ~BIT7;

    /* NTC sense node on P1.4/A4 -> route to ADC10 */
    P1DIR &= ~BIT4;
    P1SEL0 |= BIT4;
    P1SEL1 |= BIT4;

    /* LEDs as GPIO outputs (PJ.0..3 and P3.4..7) */
    PJDIR |= (BIT0 | BIT1 | BIT2 | BIT3);
    P3DIR |= (BIT4 | BIT5 | BIT6 | BIT7);

    /* Make sure LED pins are GPIO (not alternate functions) */
    P3SEL0 &= ~(BIT4 | BIT5 | BIT6 | BIT7);
    P3SEL1 &= ~(BIT4 | BIT5 | BIT6 | BIT7);

    leds_all_off();
}

/* ---------------- Init: UART A0 @ 9600 using SMCLK ≈ 1 MHz ----------------
   For 1 MHz SMCLK with oversampling:
   - BRW = 6
   - BRF = 8
   - BRS ≈ 0x20
   UCA0MCTLW literal used (no _OFS macros):
     UCOS16 (bit0) | (BRF<<4) | (BRS<<8) = 0x0001 | (8<<4) | (0x20<<8) = 0x2081
--------------------------------------------------------------------------- */
static void init_uart_9600_smclk_1mhz(void) {
    /* Route pins: P2.0=UCA0TXD, P2.1=UCA0RXD
       On FR57xx: secondary function -> P2SEL1=1, P2SEL0=0 for these bits */
    P2SEL1 |= (BIT0 | BIT1);
    P2SEL0 &= ~(BIT0 | BIT1);

    UCA0CTLW0 = UCSWRST;            // hold eUSCI in reset
    UCA0CTLW0 |= UCSSEL__SMCLK;     // SMCLK as clock source

    UCA0BRW   = 6;                  // prescaler
    UCA0MCTLW = 0x2081;             // UCOS16 + BRF=8 + BRS=0x20

    UCA0CTLW0 &= ~UCSWRST;          // release for operation
}

/* ---------------- Init: ADC10_B on A4 (P1.4), 10-bit ---------------- */
static void init_adc_a4(void) {
    ADC10CTL0 = ADC10SHT_2 | ADC10ON;            // S/H 16 cycles, ADC on
    ADC10CTL1 = ADC10SHP;                         // use sampling timer
    ADC10CTL2 = ADC10RES;                         // 10-bit mode (explicit)
    ADC10MCTL0 = ADC10SREF_0 | ADC10INCH_4;       // AVCC/AVSS, channel A4
    ADC10CTL0 |= ADC10ENC;                        // enable conversions
}

/* ---------------- One blocking ADC sample -> 8-bit ---------------- */
static uint8_t adc_read_8bit(void) {
    ADC10CTL0 |= ADC10SC;                         // start
    while (ADC10CTL1 & ADC10BUSY) ;               // wait
    return (uint8_t)(ADC10MEM0 >> 2);             // 10-bit to 8-bit
}

/* ============================== main ============================== */
int main(void) {
    uint32_t acc;
    uint8_t baseline;
    const uint8_t range = 8;  /* counts below baseline -> map to 8 LEDs (tune) */
    uint8_t v;
    int16_t delta;
    uint8_t level;
    unsigned i;

    WDTCTL = WDTPW | WDTHOLD;     // stop watchdog
    PM5CTL0 &= ~LOCKLPM5;         // unlock GPIO (FRAM devices)

    init_gpio();
    init_uart_9600_smclk_1mhz();
    init_adc_a4();

    uart_puts("MSP430 NTC (8-bit) ready @9600\r\n");

    /* Average a baseline at room temp so only LED1 is lit at rest */
    acc = 0;
    for (i = 0; i < 32; ++i) acc += adc_read_8bit();
    baseline = (uint8_t)(acc / 32);

    for (;;) {
        v = adc_read_8bit();                      // 0..255
        /* NTC: hotter -> LOWER code (negative temp coefficient) */
        delta = (int16_t)baseline - (int16_t)v;   // positive when hotter

        /* Map to 1..8 LEDs (LED1 at baseline; more LEDs as it warms) */
        level = 1;
        if (delta > 0) {
            uint8_t extra = (uint8_t)((7 * delta) / range);  // 0..7
            if (extra > 7) extra = 7;
            level = (uint8_t)(1 + extra);
        }
        leds_set_level(level);

        /* Print the reading */
        uart_puts("NTC=");
        uart_putu8(v);
        uart_puts("\r\n");

        __delay_cycles(100000UL);                // ~100 ms @ ~1 MHz MCLK
    }
}
```
## Exercise 9: Circular Buffer

1. Implement a circular buffer with 50 elements.
2. Configure UART RX interrupts so each received byte is appended to the buffer.
3. Type characters in CCS Terminal/PuTTY and verify correct storage in the buffer via the debugger.
4. When a carriage return (ASCII 13) is received, remove one character from the head of the buffer and transmit it back to the PC. Verify in CCS Terminal/PuTTY.
5. Add error checks to prevent buffer overrun (>50) and underrun (<0). Send error messages to the serial port.
6. Verify correct operation with CCS Terminal/PuTTY.
7. *(Optional, not graded)* Re-implement without UART interrupts using DMA (e.g., trigger DMA from `UCA0RXIFG`).
**code**
```c
#include <msp430.h>
#include <stdint.h>

/* ===================== Config ===================== */
#define CB_SIZE   16u      /* circular buffer length */
#define BAUD_9600_MCLK_1MHZ_BRW   6u
/* UCA0MCTLW literal for 1 MHz / 9600, oversampling: UCOS16 | BRF=8 | BRS=0x20 */
#define UCA0MCTLW_1MHZ_9600       0x2081

/* ===================== Circular Buffer ===================== */
static volatile uint8_t  cb[CB_SIZE];
static volatile uint16_t cb_head = 0;    /* index of next pop */
static volatile uint16_t cb_tail = 0;    /* index of next push */
static volatile uint16_t cb_count = 0;   /* # of elements stored */

/* Error flags set inside ISR, handled/printed in main loop */
static volatile uint8_t  flag_overrun  = 0;
static volatile uint8_t  flag_underrun = 0;

/* --------- UART helpers (polling TX, IRQ RX) --------- */
static inline void uart_putc(char c) {
    while (!(UCA0IFG & UCTXIFG)) ;
    UCA0TXBUF = (uint8_t)c;
}
static void uart_puts(const char *s) {
    while (*s) uart_putc(*s++);
}
static void uart_putu16(uint16_t v) {
    /* small decimal print (0..65535) */
    char buf[6];
    int i = 5;
    buf[i] = '\0'; --i;
    if (v == 0) { buf[i]='0'; uart_puts(&buf[i]); return; }
    while (v && i >= 0) { buf[i--] = '0' + (v % 10); v /= 10; }
    uart_puts(&buf[i+1]);
}

/* --------- Push/Pop for circular buffer (IRQ-safe) --------- */
static inline uint8_t cb_push(uint8_t b) {
    if (cb_count >= CB_SIZE) return 0;                 /* overrun */
    cb[cb_tail] = b;
    cb_tail++;
    if (cb_tail == CB_SIZE) cb_tail = 0;
    cb_count++;
    return 1;
}
static inline uint8_t cb_pop(uint8_t *out) {
    if (cb_count == 0) return 0;                       /* underrun */
    *out = cb[cb_head];
    cb_head++;
    if (cb_head == CB_SIZE) cb_head = 0;
    cb_count--;
    return 1;
}

/* ===================== Init: GPIO & UART ===================== */
static void init_uart_9600_smclk_1mhz(void) {
    /* Route P2.0 (TXD) and P2.1 (RXD) to eUSCI_A0 */
    P2SEL1 |= (BIT0 | BIT1);
    P2SEL0 &= ~(BIT0 | BIT1);

    UCA0CTLW0  = UCSWRST;                /* hold in reset */
    UCA0CTLW0 |= UCSSEL__SMCLK;          /* SMCLK as clock */

    UCA0BRW   = BAUD_9600_MCLK_1MHZ_BRW; /* prescaler = 6 */
    UCA0MCTLW = UCA0MCTLW_1MHZ_9600;     /* UCOS16 + BRF=8 + BRS≈0x20 */

    UCA0CTLW0 &= ~UCSWRST;               /* enable */
    UCA0IE    |= UCRXIE;                 /* enable RX interrupt */
}

static void init_gpio(void) {
    /* FRAM devices: unlock GPIO */
    PM5CTL0 &= ~LOCKLPM5;

    /* Nothing else required for this exercise’s UART + buffer */
}

/* ===================== Interrupts ===================== */
#pragma vector=USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void) {
    switch (__even_in_range(UCA0IV, 0x08)) {
        case 0x00: break;                 /* no interrupt */
        case 0x02: {                      /* UCRXIFG: byte received */
            uint8_t rx = UCA0RXBUF;       /* read to clear flag */

            if (rx == 13) {
                /* Carriage return: pop ONE char and echo it back */
                uint8_t outch;
                if (cb_pop(&outch)) {
                    /* echo the popped character */
                    while (!(UCA0IFG & UCTXIFG)) ;
                    UCA0TXBUF = outch;
                } else {
                    flag_underrun = 1;    /* nothing to pop */
                }
            } else {
                /* Normal char: push into circular buffer */
                if (!cb_push(rx)) {
                    flag_overrun = 1;     /* buffer full */
                }
            }
            break;
        }
        case 0x04: break;                 /* UCTXIFG (unused; we poll for TX) */
        default:  break;
    }
}

/* ===================== Main ===================== */
int main(void) {
    WDTCTL = WDTPW | WDTHOLD;   /* stop watchdog */
    init_gpio();
    init_uart_9600_smclk_1mhz();

    uart_puts("\r\nFR5739 Circular Buffer @9600\r\n");
    uart_puts("Type chars; on CR I pop one and echo it.\r\n");
    uart_puts("Buffer size = ");
    uart_putu16(CB_SIZE);
    uart_puts("\r\n");

    __bis_SR_register(GIE);     /* enable global interrupts */

    for (;;) {
        /* Print any errors outside ISR to keep ISR short/fast */
        if (flag_overrun) {
            flag_overrun = 0;
            uart_puts("[ERR] Buffer overrun (dropped input)\r\n");
        }
        if (flag_underrun) {
            flag_underrun = 0;
            uart_puts("[ERR] Buffer underrun (nothing to pop)\r\n");
        }

        /* OPTIONAL: show buffer occupancy periodically (uncomment) */
        /*
        static uint32_t ctr = 0;
        if (++ctr == 50000ul) {
            ctr = 0;
            uart_puts("count=");
            uart_putu16(cb_count);
            uart_puts(" head=");
            uart_putu16(cb_head);
            uart_puts(" tail=");
            uart_putu16(cb_tail);
            uart_puts("\r\n");
        }
        */

        /* Idle; everything happens in RX ISR */
        __no_operation();
    }
}

```
## Exercise 10: Processing Message Packets

1. Use the MECH 423 Serial Communicator (C#) to transmit/receive serial messages. Organize transmitted messages as shown below. **Data byte #1** and **Data byte #2** are the upper and lower bytes of a single 16-bit number. User-entered values are clamped to `[0, 255]` (values >255 become 255; values <0 become 0). Since 255 (`0xFF`) is the **Start** byte, do not place 255 in the two Data byte boxes directly. Instead, use single bits in the **Escape** byte to mark any Data bytes that are actually 255 so the MSP430 can restore them.

   | Start byte | Command byte | Data byte #1 | Data byte #2 | Escape byte |
   | :--------: | :----------: | :----------: | :----------: | :---------: |
   |   `0xFF`   |    `0x01`    |              |              |             |

2. Modify the circular-buffer code from Exercise 9 to handle the incoming message. Use the debugger to verify correct packet generation.

3. When a full packet is available in the buffer, extract the two data bytes and combine them into one 16-bit number.

4. Use the **Escape** byte to restore any data bytes that should be `0xFF`.

5. Remove the processed bytes from the buffer.

6. Use the received 16-bit number to set the frequency and/or duty cycle of the square wave produced by Timer B in Exercise 5.

7. Verify output frequency using an oscilloscope or with the code from Exercise 8.

8. Add commands to turn LED1 on/off from the C# program (e.g., `0x02` = on, `0x03` = off; data bytes are irrelevant for this).
**code**
```c
#include <msp430.h>
#include <stdint.h>

/* ===================== Configuration ===================== */
#define CB_SIZE   50u
#define BAUD_9600_MCLK_1MHZ_BRW   6u
#define UCA0MCTLW_1MHZ_9600       0x2081  // oversampling config
#define CMD_ECHO_RESTORE  0x07

/* Command definitions (your current map + new 0x06) */
#define CMD_SET_PWM_FREQ   0x01
#define CMD_LED_ON         0x02
#define CMD_LED_OFF        0x03
#define CMD_SET_DUTY       0x04
#define CMD_STOP_PWM       0x05
#define CMD_SET_FREQ_DUTY8 0x06   /* NEW: Data1=freq byte, Data2=duty % */

/* ===================== Circular Buffer ===================== */
static volatile uint8_t  cb[CB_SIZE];
static volatile uint16_t cb_head = 0;
static volatile uint16_t cb_tail = 0;
static volatile uint16_t cb_count = 0;

static volatile uint8_t flag_overrun  = 0;
static volatile uint8_t flag_underrun = 0;

/* ===================== Packet Assembly ===================== */
#define PACKET_LEN 5
static volatile uint8_t pkt_buf[PACKET_LEN];
static volatile uint8_t pkt_index = 0;
static volatile uint8_t collecting = 0;

/* ===================== UART helpers ===================== */
static inline void uart_putc(char c) {
    while (!(UCA0IFG & UCTXIFG));
    UCA0TXBUF = (uint8_t)c;
}
static void uart_puts(const char *s) { while (*s) uart_putc(*s++); }
static void uart_putu8(uint8_t v) {
    const char hex[17] = "0123456789ABCDEF";
    uart_putc(hex[v >> 4]); uart_putc(hex[v & 0xF]);
}
static void uart_putu16(uint16_t v) {
    uart_putu8((v >> 8) & 0xFF);
    uart_putu8(v & 0xFF);
}
// Print restore bytes
static void print_hex2(uint8_t b) { uart_putu8(b); }

static void print_restore_report(uint8_t rawH, uint8_t rawL, uint8_t esc,
                                 uint8_t fixedH, uint8_t fixedL) {
    uart_puts("RAW H=");  print_hex2(rawH);
    uart_puts(" L=");     print_hex2(rawL);
    uart_puts(" ESC=");   print_hex2(esc);
    uart_puts("  -> FIXED H="); print_hex2(fixedH);
    uart_puts(" L=");          print_hex2(fixedL);
    uart_puts("\r\n");
}

/* ===================== Circular Buffer ===================== */
static inline uint8_t cb_push(uint8_t b) {
    if (cb_count >= CB_SIZE) return 0;
    cb[cb_tail++] = b; if (cb_tail == CB_SIZE) cb_tail = 0;
    cb_count++; return 1;
}
static inline uint8_t cb_pop(uint8_t *out) {
    if (cb_count == 0) return 0;
    *out = cb[cb_head++]; if (cb_head == CB_SIZE) cb_head = 0;
    cb_count--; return 1;
}

/* ===================== Init UART/GPIO ===================== */
static void init_uart(void) {
    P2SEL1 |= (BIT0 | BIT1);
    P2SEL0 &= ~(BIT0 | BIT1);
    UCA0CTLW0  = UCSWRST;
    UCA0CTLW0 |= UCSSEL__SMCLK;
    UCA0BRW   = BAUD_9600_MCLK_1MHZ_BRW;
    UCA0MCTLW = UCA0MCTLW_1MHZ_9600;
    UCA0CTLW0 &= ~UCSWRST;
    UCA0IE    |= UCRXIE;
}

static void init_gpio(void) {
    PM5CTL0 &= ~LOCKLPM5;   // unlock GPIO

    /* LED1 on PJ.0 */
    PJSEL0 &= ~BIT0; PJSEL1 &= ~BIT0;
    PJDIR  |= BIT0;  PJOUT  &= ~BIT0;

    /* PWM output on P3.4 (LED5 -> TB1.1) */
    P3DIR  |= BIT4;
    P3SEL0 |= BIT4;  // TB1.1
    P3SEL1 &= ~BIT4;
}

/* ===================== TimerB1 (PWM) ===================== */
static void init_timerB1_pwm(void) {
    TB1CTL   = TBSSEL__SMCLK | MC__STOP | TBCLR; // stop initially
    TB1CCTL1 = OUTMOD_7;       // Reset/set mode
    TB1CCR0  = 2000 - 1;       // default 500 Hz
    TB1CCR1  = 1000;           // 50 % duty
}

/* Start/stop helpers */
static void start_pwm(void) {
    // (Re)route peripheral function before starting
    P3SEL0 |= BIT4; P3SEL1 &= ~BIT4;
    TB1CCTL1 = OUTMOD_7;
    TB1CTL   = TBSSEL__SMCLK | MC__UP | TBCLR;
}
static void stop_pwm(void) {
    TB1CTL = TBSSEL__SMCLK | MC__STOP | TBCLR;
    TB1CCTL1 &= ~OUTMOD_7;
    // Force output pin low as GPIO
    P3SEL0 &= ~BIT4; P3SEL1 &= ~BIT4;
    P3DIR  |= BIT4;  P3OUT  &= ~BIT4;
}

/* ===================== UART ISR ===================== */
#pragma vector=USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void) {
    switch (__even_in_range(UCA0IV, 0x08)) {
        case 0x00: break;
        case 0x02: {
            uint8_t rx = UCA0RXBUF;
            if (!collecting) {
                if (rx == 0xFF) { collecting = 1; pkt_index = 0; pkt_buf[pkt_index++] = rx; }
            } else {
                pkt_buf[pkt_index++] = rx;
                if (pkt_index == PACKET_LEN) {
                    collecting = 0;
                    uint8_t i; for (i = 0; i < PACKET_LEN; i++) if (!cb_push(pkt_buf[i])) flag_overrun = 1;
                }
            }
            break;
        }
        default: break;
    }
}

/* ============ Helpers for the new combined freq+duty (8-bit each) ============ */
static void apply_freq_duty_8(uint8_t freq_byte, uint8_t duty_byte) {
    /* Period ticks = max(100, freq_byte * 100) */
    uint32_t period_ticks = (uint32_t)freq_byte * 100u;
    if (period_ticks < 100u) period_ticks = 100u;   // floor @ 100 ticks (~10 kHz)
    if (period_ticks > 65535u) period_ticks = 65535u; // safety cap

    TB1CCR0 = (uint16_t)(period_ticks - 1u);

    /* Duty % = duty_byte (0..100), clamp, then compute CCR1 */
    if (duty_byte > 100u) duty_byte = 100u;
    uint32_t ccr1 = (period_ticks * duty_byte + 50u) / 100u;  // rounded
    if (ccr1 > period_ticks - 1u) ccr1 = period_ticks - 1u;
    TB1CCR1 = (uint16_t)ccr1;

    start_pwm();

    uart_puts("FD8 set: CCR0="); uart_putu16(TB1CCR0);
    uart_puts(" CCR1=");          uart_putu16(TB1CCR1);
    uart_puts(" (fB="); uart_putu8(freq_byte);
    uart_puts(" dB=");  uart_putu8(duty_byte);
    uart_puts(")\r\n");
}

/* ===================== Packet Processing ===================== */
static void process_packet(uint8_t *pkt) {
    uint8_t cmd   = pkt[1];
    uint8_t dataH = pkt[2];
    uint8_t dataL = pkt[3];
    uint8_t esc   = pkt[4];

    /* Save raw for diagnostics */
    uint8_t rawH = dataH, rawL = dataL, rawEsc = esc;

    /* Apply escape correction */
    if (esc & BIT0) dataH = 0xFF;
    if (esc & BIT1) dataL = 0xFF;
    uint16_t value = ((uint16_t)dataH << 8) | dataL;

    /* NEW: echo/diagnostic command */
    if (cmd == CMD_ECHO_RESTORE) {
        print_restore_report(rawH, rawL, rawEsc, dataH, dataL);
        uart_puts("VALUE="); uart_putu16(value); uart_puts("\r\n");
        return;  /* no hardware action for echo */
    }

    uart_puts("CMD="); uart_putu8(cmd);
    uart_puts(" VAL="); uart_putu16(value);
    uart_puts("\r\n");

    switch (cmd) {
        case CMD_SET_PWM_FREQ: {
            uint16_t ccr0 = (value == 0) ? 1 : value;
            TB1CCR0 = ccr0;
            if (TB1CCR1 > TB1CCR0) TB1CCR1 = TB1CCR0;
            start_pwm();
            uart_puts("PWM frequency updated. CCR0="); uart_putu16(TB1CCR0);
            uart_puts(" CCR1="); uart_putu16(TB1CCR1); uart_puts("\r\n");
            break;
        }
        case CMD_SET_DUTY: {
            uint16_t ccr1 = value;
            if (ccr1 > TB1CCR0) ccr1 = TB1CCR0;
            TB1CCR1 = ccr1;
            if ( (TB1CTL & MC_3) == MC_0 ) start_pwm();
            uart_puts("PWM duty updated. CCR0="); uart_putu16(TB1CCR0);
            uart_puts(" CCR1="); uart_putu16(TB1CCR1); uart_puts("\r\n");
            break;
        }
        case CMD_STOP_PWM:
            stop_pwm(); uart_puts("PWM stopped\r\n"); break;

        case CMD_LED_ON:
            PJOUT |= BIT0; uart_puts("LED1 ON\r\n"); break;

        case CMD_LED_OFF:
            PJOUT &= ~BIT0; uart_puts("LED1 OFF\r\n"); break;

        case CMD_SET_FREQ_DUTY8: {  /* NEW combined control */
            /* Use the raw (possibly unescaped) bytes as 8-bit fields */
            uint8_t freq_byte = dataH;
            uint8_t duty_byte = dataL;
            apply_freq_duty_8(freq_byte, duty_byte);
            break;
        }

        default:
            uart_puts("Unknown CMD\r\n"); break;
    }
}

/* ===================== MAIN ===================== */
int main(void) {
    WDTCTL = WDTPW | WDTHOLD;
    init_gpio();
    init_uart();
    init_timerB1_pwm();

    uart_puts("\r\nMSP430FR5739 Packet Control\r\n");
    uart_puts("01=freq(16b) 04=duty(16b) 05=stop 02=LEDon 03=LEDoff 06=freq+duty(8b,8b)\r\n");

    __bis_SR_register(GIE);

    for (;;) {
        if (flag_overrun) { flag_overrun = 0; uart_puts("[ERR] Buffer overrun\r\n"); }
        if (cb_count >= PACKET_LEN) {
            uint8_t pkt[PACKET_LEN];
            uint8_t i; for (i = 0; i < PACKET_LEN; i++) cb_pop(&pkt[i]);
            process_packet(pkt);
        }
        __no_operation();
    }
}
```

and here is the explanation of what the code do and how to use it
```c
/* =============================================================================
   MSP430FR5739 – Serial Packet Protocol (with equations)

   PACKET (5 bytes):
     [Start][Cmd][Data1][Data2][Escape]
     Start  = 0xFF
     Cmd    = command ID (see below)
     Data1  = high byte of 16-bit value  (for 0x01 and 0x04)
              or FreqByte (for 0x06)
     Data2  = low  byte of 16-bit value  (for 0x01 and 0x04)
              or DutyByte% (for 0x06)
     Escape = bitmask to restore 0xFF in data bytes:
              bit0=1 → Data1 was 0xFF; bit1=1 → Data2 was 0xFF
              (PC tool can’t send 0xFF in data; MCU restores it using Escape)

   TIMER/PWM BASICS (TB1.1 → P3.4):
     Up mode with SMCLK:
       f_out  = SMCLK / (CCR0 + 1)
       duty%  = 100 * CCR1 / (CCR0 + 1)

   COMMAND MAP:
     0x01  Set PWM Frequency (16-bit CCR0), then start PWM
           • INPUT: value16 = (Data1<<8) | Data2  (after Escape fix)
           • EQUATIONS:
               CCR0 = max(1, value16)     // value is period-1
               f_out = SMCLK / (CCR0 + 1)
           • INVERSE (when choosing bytes for a target freq f_target):
               N     = round(SMCLK / f_target)
               CCR0  = max(1, N - 1)
               Data1 = (CCR0 >> 8) & 0xFF
               Data2 =  CCR0       & 0xFF
               Escape = set bits if any Data byte must be 0xFF

     0x04  Set PWM Duty (16-bit CCR1), auto-starts PWM if needed
           • INPUT: value16 = (Data1<<8) | Data2  (after Escape fix)
           • EQUATIONS:
               CCR1 = clamp(value16, 0, CCR0)
               duty% = 100 * CCR1 / (CCR0 + 1)
           • INVERSE (when choosing bytes for a target duty D% at current CCR0):
               CCR1  = round( (D% / 100) * (CCR0 + 1) )
               CCR1  = clamp(CCR1, 0, CCR0)
               Data1 = (CCR1 >> 8) & 0xFF
               Data2 =  CCR1       & 0xFF
               Escape = set bits if any Data byte must be 0xFF

     0x02  LED1 ON  (PJ.0 high)
     0x03  LED1 OFF (PJ.0 low)
     0x05  STOP PWM (disable TB output, force P3.4 low)

     0x06  Set Frequency + Duty in one packet (8-bit + 8-bit, coarse)
           • INPUT: Data1=FreqByte, Data2=DutyByte (0..100), Escape ignored
           • MAPPING/EQUATIONS:
               period_ticks = max(100, FreqByte * 100)
               CCR0 = period_ticks - 1
               CCR1 = round( period_ticks * DutyByte / 100 )
               CCR1 = clamp(CCR1, 0, CCR0)
               f_out ≈ SMCLK / (CCR0 + 1)
               duty% ≈ 100 * CCR1 / (CCR0 + 1)
           • NOTE: Use 0x01/0x04 for precise values. 0x06 is convenient/coarse.

   WORKED EXAMPLES (SMCLK = 1,000,000 Hz):

     A) Exact 1 kHz:
        Target f = 1,000 Hz → N = round(1e6 / 1,000) = 1000
        CCR0 = N - 1 = 999 = 0x03E7 → Data1=0x03 (3), Data2=0xE7 (231), Escape=0
        Packet: [FF, 01, 03, E7, 00]  (decimal: 255, 1, 3, 231, 0)

     B) Duty 25% at CCR0=999:
        CCR1 = round(0.25 * (999+1)) = round(250) = 250 = 0x00FA
        Data1=0x00 (0), Data2=0xFA (250), Escape=0
        Packet: [FF, 04, 00, FA, 00]  (decimal: 255, 4, 0, 250, 0)

     C) Value containing 0xFF (requires Escape):
        Want value = 0x00FF → Data1=0x00, Data2=0xFF
        PC can’t send 0xFF directly; send placeholder + Escape bit1=1:
        Packet: [FF, <cmd>, 00, 00, 02]  → MCU restores to 00 FF

     D) Combined freq+duty (cmd=0x06, coarse):
        FreqByte=10 → period_ticks = max(100, 10*100) = 1000 → CCR0=999 → ~1 kHz
        DutyByte=25 → CCR1 = round(1000 * 25 / 100) = 250 → 25%
        Packet: [FF, 06, 0A, 19, 00]  (decimal: 255, 6, 10, 25, 0)

   QUICK REFERENCE:
     f_out  = SMCLK / (CCR0 + 1)
     CCR0   = round(SMCLK / f_target) - 1
     duty%  = 100 * CCR1 / (CCR0 + 1)
     CCR1   = round(duty%/100 * (CCR0 + 1)), clamp [0..CCR0]
     period_ticks(0x06) = max(100, FreqByte * 100)
     CCR0(0x06) = period_ticks - 1
     CCR1(0x06) = round(period_ticks * DutyByte / 100), clamp [0..CCR0]
   ============================================================================= */
```