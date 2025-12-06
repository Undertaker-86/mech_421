#include <msp430.h>
#include <stdint.h>

// ==================== PROTOTYPES ====================
void init_clocks(void);
void init_timer_a1(void);
void init_timer_b(void);
void init_uart(void);
void uart_send_byte(char byte);
char uart_read_byte(void);
void set_phase_outputs(uint8_t step_idx);
unsigned int is_fifo_full(void);
unsigned int is_fifo_empty(void);
void fifo_push(unsigned char item);
unsigned char fifo_pop(void);
void send_error_msg(const char *msg);
void decode_command(void);

// ==================== CONSTANTS & PINS ====================
#define RX_BUF_LEN 50

// Coil mappings (Timer B outputs)
// A1: P1.5 (TB0.2), A2: P1.4 (TB0.1)
// B1: P3.5 (TB1.2), B2: P3.4 (TB1.1)
#define PHASE_A1 BIT5
#define PHASE_A2 BIT4
#define PHASE_B1 BIT5
#define PHASE_B2 BIT4

// ==================== GLOBAL VARIABLES ====================
volatile unsigned char rx_byte;
volatile unsigned char rx_fifo[RX_BUF_LEN];
volatile unsigned int fifo_head = 0;
volatile unsigned int fifo_count = 0;

// Packet fields
volatile unsigned char pkt_start;
volatile unsigned char pkt_cmd;
volatile unsigned char pkt_data_h;
volatile unsigned char pkt_data_l;
volatile unsigned char pkt_esc;
volatile unsigned int  pkt_val_combined;

volatile uint8_t step_state = 0; // 0-7

// Half-step sequence (A1, A2, B1, B2)
// Bits: 0=A1, 1=A2, 2=B1, 3=B2
static const uint8_t half_step_seq[8] = {
    0x01, // 0001: A1
    0x05, // 0101: A1+B1
    0x04, // 0100: B1
    0x06, // 0110: B1+A2
    0x02, // 0010: A2
    0x0A, // 1010: A2+B2
    0x08, // 1000: B2
    0x09  // 1001: B2+A1
};

// ==================== INITIALIZATION ====================

void init_clocks(void)
{
    WDTCTL = WDTPW | WDTHOLD; // Disable watchdog

    CSCTL0_H = CSKEY_H; // Unlock CS

    // Set DCO to 8MHz
    CSCTL1 = DCOFSEL_3;
    CSCTL2 = SELS__DCOCLK | SELA__DCOCLK | SELM__DCOCLK;
    CSCTL3 = DIVA__1 | DIVS__1 | DIVM__1;

    CSCTL0_H = 0; // Lock CS
}

void init_timer_a1(void)
{
    // Timer A1 for step timing
    // Clock: SMCLK/8 = 1MHz
    TA1CTL = TASSEL__SMCLK | ID_3 | MC__UP | TACLR;
    TA1CCR0 = 1000; // Default 1kHz
    TA1CCTL0 = CCIE;
}

void init_timer_b(void)
{
    // Timer B0 for Coils A (P1.4, P1.5)
    TB0CCR0 = 999; // PWM Period
    
    TB0CCTL1 = OUTMOD_7; // Reset/Set
    TB0CCR1 = 250;       // 25% Duty (A2)
    
    TB0CCTL2 = OUTMOD_7;
    TB0CCR2 = 250;       // 25% Duty (A1)
    
    TB0CTL = TBSSEL__SMCLK | MC__UP | TBCLR;

    // Timer B1 for Coils B (P3.4, P3.5)
    TB1CCR0 = 999;

    TB1CCTL1 = OUTMOD_7;
    TB1CCR1 = 250;       // 25% Duty (B2)
    
    TB1CCTL2 = OUTMOD_7;
    TB1CCR2 = 250;       // 25% Duty (B1)
    
    TB1CTL = TBSSEL__SMCLK | MC__UP | TBCLR;

    // Configure GPIOs
    P1DIR  |= PHASE_A1 | PHASE_A2;
    P1SEL0 |= PHASE_A1 | PHASE_A2;
    P1SEL1 &= ~(PHASE_A1 | PHASE_A2);

    P3DIR  |= PHASE_B1 | PHASE_B2;
    P3SEL0 |= PHASE_B1 | PHASE_B2;
    P3SEL1 &= ~(PHASE_B1 | PHASE_B2);
}

void init_uart(void)
{
    // Configure USCI_A1 for 9600 baud
    UCA1CTLW0 |= UCSWRST;
    UCA1CTLW0 |= UCSSEL__ACLK; // 8MHz

    // Baud rate calculation
    UCA1MCTLW |= UCOS16 | UCBRF0 | 0x4900;
    UCA1BRW = 52;

    UCA1CTLW0 &= ~UCSWRST;
    UCA1IE |= UCRXIE;

    // P2.5=RX, P2.6=TX
    P2SEL1 |= (BIT5 | BIT6);
    P2SEL0 &= ~(BIT5 | BIT6);
}

// ==================== UART FUNCTIONS ====================

void uart_send_byte(char byte)
{
    while (!(UCA1IFG & UCTXIFG));
    UCA1TXBUF = byte;
}

char uart_read_byte(void)
{
    while (!(UCA1IFG & UCRXIFG));
    return UCA1RXBUF;
}

void send_error_msg(const char *msg)
{
    while (*msg) {
        uart_send_byte(*msg++);
    }
}

// ==================== FIFO BUFFER ====================

unsigned int is_fifo_full(void)
{
    return (fifo_count == RX_BUF_LEN);
}

unsigned int is_fifo_empty(void)
{
    return (fifo_count == 0);
}

void fifo_push(unsigned char item)
{
    rx_fifo[(fifo_head + fifo_count) % RX_BUF_LEN] = item;
    fifo_count++;
}

unsigned char fifo_pop(void)
{
    unsigned char val = rx_fifo[fifo_head];
    fifo_head = (fifo_head + 1) % RX_BUF_LEN;
    fifo_count--;
    return val;
}

// ==================== COMMAND PROCESSING ====================

void decode_command(void)
{
    // Packet: [Start][Cmd][D1][D2][Esc]
    pkt_start = fifo_pop();
    pkt_cmd   = fifo_pop();
    pkt_data_h = fifo_pop();
    pkt_data_l = fifo_pop();
    pkt_esc    = fifo_pop();

    if (pkt_start == 255) {
        // Handle escape bytes (0xFF protection)
        if (pkt_esc & BIT0) pkt_data_l = 255;
        if (pkt_esc & BIT1) pkt_data_h = 255;

        pkt_val_combined = ((unsigned int)pkt_data_h << 8) | pkt_data_l;

        switch (pkt_cmd) {
            case 1: // Continuous CCW
                TA1CTL = (TA1CTL & ~MC_3) | MC__UP;
                TA1CCR0 = pkt_val_combined;
                step_state = (step_state == 0) ? 7 : step_state - 1;
                break;
            
            case 2: // Continuous CW
                TA1CTL = (TA1CTL & ~MC_3) | MC__UP;
                TA1CCR0 = pkt_val_combined;
                step_state = (step_state + 1) & 0x07;
                break;

            case 3: // Single Step CCW
                TA1CTL = (TA1CTL & ~MC_3) | MC__STOP;
                step_state = (step_state == 0) ? 7 : step_state - 1;
                set_phase_outputs(step_state);
                break;

            case 4: // Single Step CW
                TA1CTL = (TA1CTL & ~MC_3) | MC__STOP;
                step_state = (step_state + 1) & 0x07;
                set_phase_outputs(step_state);
                break;
        }
    }
}

// ==================== MOTOR CONTROL ====================

void set_phase_outputs(uint8_t step_idx)
{
    uint8_t pattern = half_step_seq[step_idx];

    // Update PWM duty cycles based on pattern bits
    // A1 (Bit 0)
    TB0CCR2 = (pattern & 0x01) ? 250 : 0;
    // A2 (Bit 1)
    TB0CCR1 = (pattern & 0x02) ? 250 : 0;
    // B1 (Bit 2)
    TB1CCR2 = (pattern & 0x04) ? 250 : 0;
    // B2 (Bit 3)
    TB1CCR1 = (pattern & 0x08) ? 250 : 0;
}

// ==================== MAIN LOOP ====================

int main(void)
{
    init_clocks();
    init_timer_a1();
    init_timer_b();
    init_uart();

    PM5CTL0 &= ~LOCKLPM5; // Unlock GPIO

    // Ensure coils off initially
    P1OUT &= ~(PHASE_A1 | PHASE_A2);
    P3OUT &= ~(PHASE_B1 | PHASE_B2);

    __enable_interrupt();

    while (1) {
        __no_operation(); // Sleep/Idle
    }
}

// ==================== INTERRUPTS ====================

#pragma vector = TIMER1_A0_VECTOR
__interrupt void Timer_A1_ISR(void)
{
    set_phase_outputs(step_state);

    // Update state for next step
    if (pkt_cmd == 1)      // CCW
        step_state = (step_state + 1) & 0x07;
    else if (pkt_cmd == 2) // CW
        step_state = (step_state == 0) ? 7 : step_state - 1;
    
    TA1CCTL0 &= ~CCIFG;
}

#pragma vector = USCI_A1_VECTOR
__interrupt void USCI_A1_ISR(void)
{
    if (UCA1IFG & UCRXIFG) {
        rx_byte = uart_read_byte();

        if (!is_fifo_full()) {
            fifo_push(rx_byte);
        } else {
            send_error_msg("ERR: FIFO Full\r\n");
        }

        // Check if full packet available (5 bytes)
        if (fifo_count >= 5) {
            decode_command();
        }

        UCA1IFG &= ~UCRXIFG;
    }
}
