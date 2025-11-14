# Ryan's Distance Sensor Monitor

An enhanced C# Windows Forms application for monitoring and calibrating a distance sensor with a modern, streamlined UI.

## 🎯 Key Features

### **Enhanced UI Design**
- **Split-panel layout**: Controls on left, triple charts on right
- **LED-style connection indicator**: Visual feedback for connection status
- **Color-coded displays**: Each measurement has its own color (ADC=Blue, Voltage=Orange, Distance=Green)
- **Modern flat design**: Clean, professional appearance with grouped controls

### **Triple Chart Display (with Auto-Scaling)**
1. **ADC Chart**: Real-time 10-bit ADC values (0-1023)
2. **Voltage Chart**: Calculated voltage (0-3.3V)
3. **Distance Chart**: Calibrated distance in cm

All charts auto-scale and display the last 30 seconds of data.

### **Simplified Calibration Workflow**
- **Auto-capture ADC**: Click "📍 Capture Point" to automatically use the current live ADC reading
- **No manual ADC entry**: Just enter the distance and capture!
- **Streamlined process**: Connect → Set distance → Capture → Repeat → Fit curve
- **Multiple fit types**: Linear, Polynomial (2nd/3rd), Power, Inverse

### **Enhanced Readings Display**
- **Three measurements**: ADC, Voltage, and Distance
- **Large, readable fonts**: Easy to monitor from a distance
- **Real-time voltage calculation**: V = ADC × 3.3 / 1023
- **Range indicator**: Visual warning when out of range

## 📊 UI Layout

```
┌─────────────────────────────────────────────────────────────────┐
│ Ryan's Distance Sensor Monitor                                  │
├──────────────┬──────────────────────────────────────────────────┤
│              │  ADC VALUE (0-1023)                              │
│ CONNECTION   │  [═══════════════════════════════════]           │
│ ● Connected  │                                                   │
│ COM3  9600   │  ───────────────────────────────────────────     │
│ [Connect]    │                                                   │
│              │                                                   │
│ READINGS     │  VOLTAGE (0-3.3V)                                │
│ ADC: 512     │  [═══════════════════════════════════]           │
│ Voltage: 1.65│                                                   │
│ Distance: 15 │  ───────────────────────────────────────────     │
│ ● In Range   │                                                   │
│              │                                                   │
│ CALIBRATION  │  DISTANCE (cm)                                   │
│ Distance: 10 │  [═══════════════════════════════════]           │
│ [📍 Capture] │                                                   │
│ Fit: Poly(2) │  ───────────────────────────────────────────     │
│ [Fit Curve]  │                                                   │
│ R² = 0.9995  │                                                   │
│              │                                                   │
│ DATA LOGGING │                                                   │
│ [▶ Start]    │                                                   │
│ [■ Stop]     │                                                   │
│ Samples: 0   │                                                   │
│ [Export CSV] │                                                   │
│ [Save Calib] │                                                   │
└──────────────┴──────────────────────────────────────────────────┘
```

## 🚀 Quick Start Guide

### 1. **Connect to Sensor**
- Select COM port from dropdown
- Choose baud rate (default: 9600)
- Click **Connect**
- LED indicator turns green when connected

### 2. **Calibrate (Easy Method!)**
1. Connect to sensor first
2. Place sensor at known distance (e.g., 10 cm)
3. Enter the distance in the box
4. Click **📍 Capture Point** (automatically grabs current ADC value!)
5. Move sensor to new distance
6. Repeat steps 3-5 for at least 5 distances
7. Select fit type (Polynomial 2nd order recommended)
8. Click **Fit Curve**
9. Check R² value (should be > 0.99 for good fit)

### 3. **Monitor Data**
- Watch real-time ADC, Voltage, and Distance values
- View all three charts updating automatically
- Charts show last 30 seconds with auto-scaling
- Range indicator warns if sensor is out of calibrated range

### 4. **Log Data**
- Click **▶ Start Logging** to begin recording
- Click **■ Stop Logging** to finish
- Click **Export to CSV** to save data

## 📈 Voltage Calculation

The application automatically calculates voltage from ADC:

```
Voltage (V) = ADC Value × 3.3V / 1023
```

For example:
- ADC = 512 → Voltage = 1.65V
- ADC = 1023 → Voltage = 3.30V
- ADC = 0 → Voltage = 0.00V

## 💾 Data Export Format

CSV files include voltage column:

```csv
Timestamp,ADC Value,Voltage (V),Distance (cm),In Range
2025-11-07 17:30:45.123,512,1.6500,15.340,True
2025-11-07 17:30:45.223,514,1.6565,15.456,True
...
```

## 🎨 Color Coding

- **Blue** (#0064C8): ADC values
- **Orange** (#C86400): Voltage measurements
- **Green** (#009600): Distance readings
- **Red**: Disconnected / Out of range
- **Lime Green**: Connected / In range

## 🔧 Technical Details

### Serial Protocol
- **Packet format**: 3 bytes [0xFF, MS5B, LS5B]
- **ADC reassembly**: `ADC = (MS5B << 5) | LS5B`
- **Update rate**: ~10ms (100 Hz max)
- **Baud rates**: 9600, 19200, 38400, 57600, 115200

### Calibration Fits
1. **Linear**: y = mx + b
2. **Polynomial (2nd)**: y = ax² + bx + c ⭐ Recommended
3. **Polynomial (3rd)**: y = ax³ + bx² + cx + d
4. **Power**: y = a·x^b
5. **Inverse**: y = a/(x-b) + c (good for IR sensors)

### System Requirements
- Windows 7 or later
- .NET 9.0 Runtime
- Serial port connection
- Screen resolution: 1400×900 minimum

## 🆚 Differences from Original

| Feature | Original | Ryan's Version |
|---------|----------|----------------|
| **Layout** | Tabbed interface | Split-panel |
| **Charts** | 2 charts (ADC, Distance) | 3 charts (ADC, Voltage, Distance) |
| **Calibration** | Manual ADC entry | Auto-capture current ADC |
| **Voltage** | Not displayed | Real-time display + chart |
| **Connection** | Text status | LED indicator + text |
| **UI Style** | Standard | Modern flat design |
| **Colors** | Default | Color-coded by measurement |

## 🎯 Advantages of Auto-Capture

**Old way**: 
1. Read ADC value from screen
2. Manually type it into calibration form
3. Type distance
4. Add point
5. Prone to typos!

**New way**:
1. Type distance
2. Click "📍 Capture Point"
3. Done! ✨

Much faster and no transcription errors!

## 🏗️ Project Structure

```
RyanSensorApp/
├── Models/
│   ├── SensorReading.cs       # Enhanced with Voltage
│   ├── CalibrationPoint.cs
│   └── CalibrationData.cs
├── Services/
│   ├── SerialPortService.cs   # Serial communication
│   ├── CalibrationService.cs  # Curve fitting
│   └── DataLogger.cs          # Data storage
├── MainForm.cs                # Split-panel UI
├── Program.cs                 # Entry point
└── README.md                  # This file
```

## 🐛 Troubleshooting

### Charts not updating
- Verify connection is active (LED should be green)
- Check if data is coming in (ADC value should change)
- Restart the application

### Calibration capture not working
- Must be connected to sensor first
- Ensure sensor is actually sending data
- Check current ADC value is updating

### Poor calibration fit (low R²)
- Use more calibration points (5-10 recommended)
- Try different fit type
- Ensure calibration points cover the full range
- Check for sensor stability

## 📝 Notes

- All charts auto-scale based on data range
- Charts show rolling 30-second window
- Voltage is automatically calculated for every reading
- CSV export includes voltage column
- Calibration files are compatible with original version

## 🎓 Perfect for Lab Work!

This application is specifically designed for efficient lab work:
- Quick setup and calibration
- Clear visual feedback
- Easy data collection
- Professional-looking interface
- All measurements at a glance

---

**Created for MECH 421 Distance Sensor Lab**  
*Enhanced version with improved UX and voltage monitoring*
