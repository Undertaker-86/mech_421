# Distance Sensor Monitor Application

A C# Windows Forms application for monitoring and calibrating a distance sensor connected to an MSP430FR5739 microcontroller.

## Features

### Monitor Tab
- **Serial Port Connection**: Connect to MSP430 via COM port (9600 baud default)
- **Real-time Data Display**: 
  - Current ADC value (0-1023, 10-bit)
  - Converted distance in cm
  - Range status indicator
- **Dual Live Charts**:
  - ADC value vs. time
  - Distance vs. time
- **Data Logging**: Start/stop logging with timestamp
- **CSV Export**: Export logged data to CSV file

### Calibration Tab
- **Calibration Point Entry**: Add distance-ADC value pairs
- **Multiple Curve Fitting Options**:
  - Linear: y = mx + b
  - Polynomial (2nd order): y = ax² + bx + c
  - Polynomial (3rd order): y = ax³ + bx² + cx + d
  - Power law: y = a·x^b
  - Inverse: y = a/(x-b) + c
- **Visualization**: Plot calibration points and fitted curve
- **R² Quality Metric**: Evaluate fit quality
- **Save/Load Calibration**: Store calibration as JSON
- **Range Configuration**: Set min/max ADC thresholds

## System Requirements

- Windows 7 or later
- .NET 9.0 Runtime
- Visual Studio 2022 (for development)
- Serial port connection to MSP430FR5739

## Firmware Protocol

The MSP430 sends data in 3-byte packets at 10ms intervals:
- **Byte 1**: Start byte (0xFF)
- **Byte 2**: MS5B (bits 9-5 of ADC value)
- **Byte 3**: LS5B (bits 4-0 of ADC value)

The application reassembles the 10-bit ADC value: `ADC = (MS5B << 5) | LS5B`

## Building the Application

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build DistanceSensorApp.csproj

# Run the application
dotnet run
```

Or open `DistanceSensorApp.csproj` in Visual Studio and press F5.

## Usage Instructions

### 1. Connect to Sensor
1. Connect your MSP430 to a USB-to-Serial adapter
2. Open the Monitor tab
3. Select the appropriate COM port
4. Click "Connect"

### 2. Perform Calibration
1. Switch to the Calibration tab
2. Place the sensor at a known distance
3. Note the ADC value from the Monitor tab
4. Enter the distance and ADC value
5. Click "Add Point"
6. Repeat for at least 5 different distances
7. Select a fit type (Polynomial 2nd order recommended)
8. Click "Fit Curve"
9. Review the equation and R² value
10. Click "Save Calibration" to store the calibration
11. Click "Apply to Monitor" to use the calibration

### 3. Monitor Distance
1. Return to the Monitor tab
2. Real-time distance will be displayed using the calibration
3. Click "Start Logging" to begin recording data
4. Click "Export to CSV" to save the data

## Calibration Tips

- **Use 5-10 calibration points** across the sensor's range
- **Polynomial 2nd order** works well for most photodiode distance sensors
- **R² > 0.99** indicates a good fit
- Test the calibration at known distances to verify accuracy
- **Inverse fit** often works best for IR distance sensors

## CSV Export Format

```
Timestamp,ADC Value,Distance (cm),In Range
2025-11-07 17:15:30.123,512,15.340,True
2025-11-07 17:15:30.223,514,15.456,True
...
```

## Troubleshooting

### No COM Ports Available
- Check device connections
- Verify USB-to-Serial driver is installed
- Try unplugging and reconnecting the device

### Connection Fails
- Ensure correct baud rate (9600)
- Check that no other application is using the port
- Verify the MSP430 firmware is running

### Erratic Readings
- Check sensor connections
- Ensure proper power supply
- Verify ADC reference voltage (should be AVCC)
- Check for electromagnetic interference

### Out of Range Indicator
- Sensor too close or too far
- Adjust Min ADC and Max ADC values in calibration tab
- Recalibrate if working range has changed

## NuGet Dependencies

- **ScottPlot.WinForms** 5.1.57: Real-time plotting
- **System.IO.Ports** 9.0.10: Serial communication
- **MathNet.Numerics** 5.0.0: Curve fitting algorithms
- **Newtonsoft.Json** 13.0.4: Calibration data serialization

## Project Structure

```
DistanceSensorApp/
├── Models/
│   ├── SensorReading.cs       # Data point model
│   ├── CalibrationPoint.cs    # Calibration point model
│   └── CalibrationData.cs     # Calibration with curve fit
├── Services/
│   ├── SerialPortService.cs   # Serial communication
│   ├── CalibrationService.cs  # Curve fitting logic
│   └── DataLogger.cs          # Data storage and export
├── MainForm.cs                # Main UI with tabs
└── Program.cs                 # Application entry point
```

## Lab Requirements Checklist

### Exercise 6
- ✅ MSP430 firmware digitizes to 10 bits (0-3.3V)
- ✅ Data split into MS5B and LS5B bytes
- ✅ C# program connects to serial port
- ✅ ADC data reassembly (MS5B and LS5B)
- ✅ Display, graph, and store ADC data
- ✅ User-friendly interface

### Exercise 7
- ✅ Measure ADC at 5+ different distances
- ✅ Plot calibration data
- ✅ Fit function to calibration data
- ✅ Visualize raw data and fitted curve
- ✅ Display R² value
- ✅ Convert ADC to distance
- ✅ Display both ADC and distance
- ✅ Out-of-range indicator
- ✅ Record data for RMS noise measurement
- ✅ Calculate standard deviation

## License

This project is for educational purposes as part of MECH 421 lab coursework.

## Author

Created for MECH 421 Distance Sensor Lab - Phase 3
