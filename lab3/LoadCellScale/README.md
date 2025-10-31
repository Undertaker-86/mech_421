# Load Cell Scale

This WinForms application pairs with the MSP430FR5739 firmware in `../firmware/load_cell_adc.c` to turn an amplified load-cell signal into a calibrated digital scale with a responsive UI.

## Build & Run

1. Install the .NET Desktop Runtime/SDK with WinForms support (the project targets .NET Framework 4.7.2).
2. From `LoadCellScale`, compile with:
   ```powershell
   dotnet build LoadCellScale.csproj
   ```
3. Launch `bin\Debug\LoadCellScale.exe` (or the Release build). Ensure the MSP430 board is connected via USB before opening the app.

## Serial Data Format

The firmware streams a repeating 3-byte frame at 115200 bps:

| Byte | Meaning                          |
|------|----------------------------------|
| 0    | 0xFF frame marker                |
| 1    | MS5B – most-significant 5 bits   |
| 2    | LS5B – least-significant 5 bits  |

The UI reassembles the 10-bit value, converts it to voltage (0–3.6 V) and, after calibration, to mass.

## UI Overview

- **Connection** – choose the COM port exposed by the MSP430 and connect/disconnect.
- **Live Measurement** – displays raw ADC, voltage, instantaneous weight, smoothed weight, sample rate, smoothing span, and a rolling stability indicator:
  - Stability turns **green** when the smoothed readings vary less than the selected threshold over the past 500 ms.
  - Press **Tare** to zero the current load and recenter the smoothing buffer.
- **Chart** – plots raw vs. smoothed weight (up to 600 recent points) for quick trend inspection.
- **Calibration** – capture reference points, compute the linear fit, and review the resulting equation (including R²).

## Calibration Workflow

1. Gather at least two accurately known masses within the 0–2 kg range.
2. With the MSP430 streaming data, place the first mass on the scale. Wait for the stability indicator to turn green.
3. Enter the mass in kilograms (e.g., `0.500`) and click **Capture calibration point**. The app averages the most recent samples to reduce noise.
4. Repeat for each reference mass. Using at least three points (including no-load) improves accuracy.
5. Click **Compute fit** to apply a least-squares line (`Mass = slope * ADC + intercept`). The tare offset is reset so the new curve starts from the captured baseline.
6. (Optional) Use **Clear points** to restart the calibration process.

Once calibrated, the **Weight (kg)** and **Smoothed (kg)** fields report mass, and the stability indicator reflects the configured variability threshold.

## MSP430 Firmware Highlights

- Samples analog channel A0 at ~100 Hz using the ADC12_B configured for 10-bit resolution with AVCC (3.6 V) as reference.
- Uses Timer_B0 to trigger conversions and USCI_A0 for UART streaming.
- Packs each sample into the specified 3-byte frame (`firmware/load_cell_adc.c`).

Before operating, adjust the instrumentation amplifier so the no-load and maximum-load outputs remain below 2.5 V to preserve headroom. Verify the analog front-end wiring matches the MSP430 pin assignments documented in the firmware comments.
