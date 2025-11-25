# Exercise 5 – Motor Driver Checkout

Contents
- `firmware/main.c`: MSP430FR5739 code that produces a PWM on TB0.1 (P1.4) and a GPIO direction pin on P1.5. Commands arrive over UART0 (P2.0/P2.1) at 115200-8N1.
- `csharp/`: simple .NET console helper to send commands and log acknowledgements.

Firmware notes
- Default clock = 1 MHz DCO for predictable UART/PWM settings.
- PWM period = 25 kHz (`TB0CCR0=4000`), default duty = 25%. Direction is a GPIO.
- UART commands (ASCII, newline or carriage return to apply):
  - `d0` / `d1` sets direction low/high.
  - `p###` sets duty 0–100 (%). Example: `p025` = 25%.
  - `?` prints current direction and duty.
- Pin mapping is defined at the top of `firmware/main.c`; adjust if your PCB routes the DRV8841 differently (e.g., different TB channel or direction pin).

Build/flash
1) Create a CCS/Makefile project that pulls in `firmware/main.c` for target MSP430FR5739.
2) Flash via the 6-pin JTAG on the green board (per lab note).

Usage
1) Wire DRV8841 PWM input to P1.4 (TB0.1) and direction to P1.5. Ensure `nSLEEP` is held high.
2) Power the board, open a serial terminal at 115200-8N1, and type commands (`p050` then `d1`, etc.).
3) Motor should spin; use `?` to confirm settings.
