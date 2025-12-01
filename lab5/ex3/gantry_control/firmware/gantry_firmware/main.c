#include <msp430.h>
#include <stdint.h>
#include <stdlib.h>

// ==================== DEFINES ====================
#define QUEUE_SIZE 50

// Motor 1 (X-Axis) Pins - Using Timer B0/B1 PWM logic from example
// A1: P1.5 (TB0.2), A2: P1.4 (TB0.1)
// B1: P3.5 (TB1.2), B2: P3.4 (TB1.1)
// Note: The example used PWM CCRs to drive these. We will replicate that.

// Motor 2 (Y-Axis) Pins - New Driver
// PWMA, PWMB: P1.3 (Shared Enable/PWM)
// AIN2: P2.0
// AIN1: P2.1
// BIN1: P3.2
// BIN2: P3.3
#define M2_PWM_PIN BIT3 // P1.3
#define M2_AIN2    BIT0 // PJ.0
#define M2_AIN1    BIT1 // PJ.1
#define M2_BIN1    BIT2 // PJ.2
#define M2_BIN2    BIT3 // PJ.3

// PWM Settings (8MHz Clock)
// 8kHz: Period=1000
// 20kHz: Period=400 (Silent)
#define PWM_PERIOD 400
#define PWM_DUTY   (PWM_PERIOD * 80 / 100)

// Stepper Constants
#define STEPS_PER_REV 200 // Standard 1.8 deg stepper
#define MICROSTEPS 1      // Full stepping for now
// Assuming some calibration: Steps per CM. 
// Let's assume 1 rev = 4 cm travel (example). -> 50 steps/cm.
// User can adjust this calibration in C# or here. 
// For now, we will receive "Steps" directly from C# to keep firmware simple, 
// OR we receive CM and convert. The prompt says "takes a relative [X,Y] distance".
// It's better to do the math in C# and send raw steps to MCU.
// So Packet will contain STEPS.

// ==================== GLOBALS ====================
volatile unsigned char queue[QUEUE_SIZE];
volatile unsigned int front = 0;
volatile unsigned int numItems = 0;

// Packet Buffer
volatile unsigned char startByte;
volatile unsigned char cmdByte; // 1=Move, 2=Stop
volatile unsigned char xH, xL, yH, yL;
volatile unsigned char velByte;
volatile unsigned char escapeByte;

// Motion Control Globals
volatile int32_t target_x_steps = 0;
volatile int32_t target_y_steps = 0;
volatile int32_t current_x_steps = 0;
volatile int32_t current_y_steps = 0;

volatile int32_t delta_x = 0;
volatile int32_t delta_y = 0;
volatile int32_t step_x_inc = 0;
volatile int32_t step_y_inc = 0;
volatile int32_t error_term = 0;
volatile uint32_t total_steps_needed = 0;
volatile uint32_t steps_taken = 0;

volatile uint8_t motor1_state = 0;
volatile uint8_t motor2_state = 0;
volatile uint8_t is_moving = 0;

// Half-step lookup table (8 steps)
// bit0=A1, bit1=A2, bit2=B1, bit3=B2
static const uint8_t stepper_table[8] = {
    0b0001, // 1: A1
    0b0101, // 2: A1+B1
    0b0100, // 3: B1
    0b0110, // 4: B1+A2
    0b0010, // 5: A2
    0b1010, // 6: A2+B2
    0b1000, // 7: B2
    0b1001  // 8: B2+A1
};

// ==================== PROTOTYPES ====================
void clockSetup(void);
void gpioSetup(void);
void timerSetup(void);
void uartSetup(void);
void step_motor1(uint8_t step);
void step_motor2(uint8_t step);
void process_packet(void);
void start_move(int16_t dx, int16_t dy, uint8_t speed);

// ==================== SETUP FUNCTIONS ====================
void clockSetup(void) {
    CSCTL0_H = CSKEY_H;
    CSCTL1 = DCOFSEL_3; // 8 MHz
    CSCTL2 = SELS__DCOCLK | SELA__DCOCLK | SELM__DCOCLK;
    CSCTL3 = DIVA__1 | DIVS__1 | DIVM__1;
    CSCTL0_H = 0;
}

void gpioSetup(void) {
    // Motor 1 (Existing) - PWM pins handled in Timer Setup, but we need to ensure directions
    // P1.4, P1.5, P3.4, P3.5 are used by Timer B0/B1 in the example.
    // We will stick to the example's method of using TBxCCRx for Motor 1.
    
    // Motor 2 (New) - GPIO Control
    // P1.3 (PWM/Enable) -> Output, High
    P1DIR |= M2_PWM_PIN;
    P1OUT |= M2_PWM_PIN; // Enable driver

    // Port J (PJ.0 - PJ.3) for Motor 2 Coils
    // Note: PJ is often shared with JTAG. Ensure JTAG is not interfering if debugging.
    PJDIR |= M2_AIN2 | M2_AIN1 | M2_BIN1 | M2_BIN2;
    PJOUT &= ~(M2_AIN2 | M2_AIN1 | M2_BIN1 | M2_BIN2);
    // Clear SEL bits to ensure GPIO mode if necessary (though usually default is GPIO for PJ on some devices, check datasheet)
    // On FR5739, PJ.0-3 are JTAG. To use as GPIO, we might need to be careful.
    // Assuming user has this working or knows the setup.
    PJSEL0 &= ~(M2_AIN2 | M2_AIN1 | M2_BIN1 | M2_BIN2);
    PJSEL1 &= ~(M2_AIN2 | M2_AIN1 | M2_BIN1 | M2_BIN2);

    // Unlock GPIO
    PM5CTL0 &= ~LOCKLPM5;
}

void timerSetup(void) {
    // --- Timer A1: Step Clock ---
    // We will use TA1 for the motion tick.
    // Frequency will be set dynamically based on speed.
    TA1CTL = TASSEL__SMCLK | MC__STOP | TACLR; 
    TA1CCTL0 = CCIE;

    // --- Timer B0/B1: Motor 1 PWM Generation (from example) ---
    // TB0: P1.4(A2), P1.5(A1)
    TB0CCR0 = PWM_PERIOD - 1; 
    TB0CCTL1 = OUTMOD_7; // Reset/Set
    TB0CCTL2 = OUTMOD_7; 
    TB0CTL = TBSSEL__SMCLK | MC__UP | TBCLR;

    // TB1: P3.4(B2), P3.5(B1)
    TB1CCR0 = PWM_PERIOD - 1;
    TB1CCTL1 = OUTMOD_7;
    TB1CCTL2 = OUTMOD_7;
    TB1CTL = TBSSEL__SMCLK | MC__UP | TBCLR;

    // Set Pins for TB0/TB1
    P1DIR |= BIT4 | BIT5;
    P1SEL0 |= BIT4 | BIT5; 
    P1SEL1 &= ~(BIT4 | BIT5);

    P3DIR |= BIT4 | BIT5;
    P3SEL0 |= BIT4 | BIT5;
    P3SEL1 &= ~(BIT4 | BIT5);
}

void uartSetup(void) {
    // P2.5=RX, P2.6=TX
    P2SEL1 |= BIT5 | BIT6;
    P2SEL0 &= ~(BIT5 | BIT6);

    UCA1CTLW0 |= UCSWRST;
    UCA1CTLW0 |= UCSSEL__SMCLK; // Use SMCLK (8MHz)
    // 9600 Baud from 8MHz
    // N = 8000000/9600 = 833.33
    // UCBRx = 52, UCBRFx = 1, UCBRSx = 0x49 (from example)
    // Wait, example used ACLK=8MHz. We set SMCLK=8MHz. Same difference.
    UCA1MCTLW = UCOS16 | 0x4900 | 0x0010; // UCBRF=1
    UCA1BRW = 52;
    
    UCA1CTLW0 &= ~UCSWRST;
    UCA1IE |= UCRXIE;
}

// ==================== MOTOR CONTROL ====================
void step_motor1(uint8_t step) {
    // Motor 1 uses Timer PWMs (TB0, TB1)
    uint8_t mask = stepper_table[step & 0x07];

    // A1 (TB0.2)
    TB0CCR2 = (mask & 0x01) ? PWM_DUTY : 0; 
    // A2 (TB0.1)
    TB0CCR1 = (mask & 0x02) ? PWM_DUTY : 0;
    // B1 (TB1.2)
    TB1CCR2 = (mask & 0x04) ? PWM_DUTY : 0;
    // B2 (TB1.1)
    TB1CCR1 = (mask & 0x08) ? PWM_DUTY : 0;
}

void step_motor2(uint8_t step) {
    // Motor 2 uses GPIOs directly
    uint8_t mask = stepper_table[step & 0x07];

    // A1 (AIN2?) - Let's map bit0->AIN2, bit1->AIN1
    // Actually, let's just map logically.
    // Coil A: AIN1, AIN2. Coil B: BIN1, BIN2.
    // Table: bit0=A1, bit1=A2, bit2=B1, bit3=B2
    
    // A1 (AIN1)
    if (mask & 0x01) PJOUT |= M2_AIN1; else PJOUT &= ~M2_AIN1;
    // A2 (AIN2)
    if (mask & 0x02) PJOUT |= M2_AIN2; else PJOUT &= ~M2_AIN2;
    
    // B1 (BIN1)
    if (mask & 0x04) PJOUT |= M2_BIN1; else PJOUT &= ~M2_BIN1;
    // B2 (BIN2)
    if (mask & 0x08) PJOUT |= M2_BIN2; else PJOUT &= ~M2_BIN2;
}

void start_move(int16_t dx, int16_t dy, uint8_t speed) {
    // Disable interrupt to setup
    TA1CTL &= ~MC_3;

    delta_x = dx;
    delta_y = dy;
    
    step_x_inc = (dx > 0) ? 1 : -1;
    step_y_inc = (dy > 0) ? 1 : -1;
    
    delta_x = abs(delta_x);
    delta_y = abs(delta_y);

    total_steps_needed = (delta_x > delta_y) ? delta_x : delta_y;
    steps_taken = 0;

    // Initialize error term for Bresenham's
    // We will use a simplified DDA:
    // We have a major axis and a minor axis.
    // But actually, we can just use floating point logic scaled up, or just standard Bresenham.
    // Let's use a counter approach for both.
    // We want to complete 'total_steps_needed' ticks.
    // On each tick:
    //   AccumulatorX += delta_x
    //   if AccumulatorX >= total_steps_needed: step X, AccumulatorX -= total_steps_needed
    //   AccumulatorY += delta_y
    //   if AccumulatorY >= total_steps_needed: step Y, AccumulatorY -= total_steps_needed
    
    // Reset accumulators
    // We'll use static vars in ISR or globals.
    // Let's use globals 'current_x_steps' as accumulator for this move? No, that tracks position.
    // Let's add new globals for accumulators.
    
    // Speed: 100% = Max Speed.
    // Timer CCR0 = Base / Speed.
    // Base 8MHz / 8 = 1MHz.
    // Max speed (100%) -> 1kHz stepping? 
    // CCR0 = 1000 -> 1kHz.
    // If speed is 100, CCR0 = 1000.
    // If speed is 10, CCR0 = 10000.
    // Formula: CCR0 = 100000 / speed (if speed 1..100)
    if (speed == 0) speed = 1;
    TA1CCR0 = 40000 / speed; // Adjust constant to tune max speed

    is_moving = 1;
    TA1CTL |= MC__UP; // Start Timer
}

// Globals for DDA
volatile int32_t acc_x = 0;
volatile int32_t acc_y = 0;

// ==================== MAIN ====================
int main(void) {
    WDTCTL = WDTPW | WDTHOLD;
    clockSetup();
    gpioSetup();
    timerSetup();
    uartSetup();

    __enable_interrupt();

    while (1) {
        if (numItems >= 8) {
            process_packet();
        }
    }
}

// ==================== PACKET PROCESSING ====================
void process_packet(void) {
    // Packet: [255][CMD][XH][XL][YH][YL][VEL][ESC]
    // Check start byte
    if (queue[front] != 255) {
        // Invalid start, consume one byte
        front = (front + 1) % QUEUE_SIZE;
        numItems--;
        return;
    }

    // Extract bytes
    unsigned char pkt[8];
    int i;
    for (i = 0; i < 8; i++) {
        pkt[i] = queue[(front + i) % QUEUE_SIZE];
    }

    // Handle Escape
    unsigned char esc = pkt[7];
    if (esc & 0x01) pkt[6] = 255; // Vel
    if (esc & 0x02) pkt[5] = 255; // YL
    if (esc & 0x04) pkt[4] = 255; // YH
    if (esc & 0x08) pkt[3] = 255; // XL
    if (esc & 0x10) pkt[2] = 255; // XH
    // CMD usually doesn't need escape if it's small enum

    int16_t dx = (int16_t)((pkt[2] << 8) | pkt[3]);
    int16_t dy = (int16_t)((pkt[4] << 8) | pkt[5]);
    uint8_t vel = pkt[6];
    uint8_t cmd = pkt[1];

    // Consume queue
    front = (front + 8) % QUEUE_SIZE;
    numItems -= 8;

    if (cmd == 1) { // MOVE
        start_move(dx, dy, vel);
    } else if (cmd == 2) { // STOP
        TA1CTL &= ~MC_3;
        is_moving = 0;
    }
}

// ==================== ISRs ====================
#pragma vector = USCI_A1_VECTOR
__interrupt void USCI_A1_ISR(void) {
    if (UCA1IFG & UCRXIFG) {
        unsigned char rx = UCA1RXBUF;
        if (numItems < QUEUE_SIZE) {
            queue[(front + numItems) % QUEUE_SIZE] = rx;
            numItems++;
        }
    }
}

#pragma vector = TIMER1_A0_VECTOR
__interrupt void Timer_A1_ISR(void) {
    if (!is_moving) return;

    // DDA Algorithm
    // We step the dominant axis? No, we step based on accumulators.
    // Actually, to ensure straight line, we should use the 'total_steps' approach.
    
    // Add delta to accumulators
    acc_x += delta_x;
    acc_y += delta_y;

    if (acc_x >= total_steps_needed) {
        motor1_state = (motor1_state + step_x_inc) & 0x07;
        step_motor1(motor1_state);
        acc_x -= total_steps_needed;
    }
    
    if (acc_y >= total_steps_needed) {
        motor2_state = (motor2_state + step_y_inc) & 0x07;
        step_motor2(motor2_state);
        acc_y -= total_steps_needed;
    }

    steps_taken++;
    if (steps_taken >= total_steps_needed) {
        is_moving = 0;
        TA1CTL &= ~MC_3; // Stop timer
        // Reset accumulators for next time (though start_move does this)
        acc_x = 0;
        acc_y = 0;
    }
}
