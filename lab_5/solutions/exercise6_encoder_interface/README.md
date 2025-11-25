# Exercise 6 – Shaft Encoder Interface

Contents
- `firmware/main.c`: MSP430FR5739 firmware that uses TA0CLK as an external step clock and a DIR GPIO to build a signed 32-bit position counter.
- `csharp/EncoderReaderConsole.csproj` + `Program.cs`: console helper to read/zero counts via UART.

Firmware summary
- Clock: DCO 1 MHz.
- TA0 runs in continuous mode using TA0CLK (default pin P1.2). DIR is read from P1.3. Adjust `ENC_*` macros if your board routes these differently.
- UART0 on P2.0 (TX) / P2.1 (RX) at 115200-8N1.
- Commands: `r` = read signed tick count, `z` = zero counter, `h` = help.

Build/flash
1) Add `firmware/main.c` to a CCS/Makefile project targeting MSP430FR5739 and build.
2) Program through the JTAG header on the assembled board.

Usage
1) Connect your hardware decoder outputs: step pulses -> TA0CLK pin, direction -> DIR pin.
2) Open a serial terminal at 115200-8N1.
3) Send `r` repeatedly to stream counts or `z` to zero between trials.
