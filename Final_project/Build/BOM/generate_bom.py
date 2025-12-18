
import pandas as pd

data = [
    {"Component": "Servo Motor", "Description": "DS3225 25kg High Torque Servo 180°", "Qty": 2, "Approx Price": "€28.78", "Notes": "Main body transformation"},
    {"Component": "Drone Motor", "Description": "Brushless Motor 2812 Pro 1100KV", "Qty": 4, "Approx Price": "€44.00", "Notes": "Flight motors (4x)"},
    {"Component": "Drive Motor", "Description": "Brushless Motor 5010 360KV", "Qty": 2, "Approx Price": "€34.58", "Notes": "Wheel drive motors"},
    {"Component": "Linear Actuator", "Description": "Micro Linear Actuator 12V 100mm Stroke 60N", "Qty": 2, "Approx Price": "€35.40", "Notes": "Original was 60N (too weak). Upgrade to 100N+ recommended."},
    {"Component": "Actuator Driver", "Description": "DRV8871 H-Bridge Driver", "Qty": 2, "Approx Price": "€6.30", "Notes": "Controls linear actuators"},
    {"Component": "Propellers", "Description": "8x4.1 or 8x6 Propellers", "Qty": 4, "Approx Price": "€3.68", "Notes": "2CW + 2CCW"},
    {"Component": "Flight ESC", "Description": "4-in-1 45A Brushless ESC", "Qty": 1, "Approx Price": "€30.00", "Notes": "For drone motors"},
    {"Component": "Drive ESC", "Description": "35A Bidirectional ESC", "Qty": 2, "Approx Price": "€20.58", "Notes": "For tank drive (reverse needed)"},
    {"Component": "Bearings", "Description": "12x8x3.5 mm Bearings", "Qty": 6, "Approx Price": "€7.20", "Notes": "Wheel/Arm pivot bearings"},
    {"Component": "Chassis Plate", "Description": "Carbon Fiber Plate 2mm 300x200mm", "Qty": 2, "Approx Price": "€160.00", "Notes": "Main frame"},
    {"Component": "Battery", "Description": "6S LiPo 2200mAh 100C+", "Qty": 2, "Approx Price": "€50.00", "Notes": "High discharge needed"},
    {"Component": "Controller", "Description": "Teensy 4.0", "Qty": 1, "Approx Price": "€30.40", "Notes": "Flight controller / Main brain"},
    {"Component": "BEC 5V", "Description": "Step-Down Voltage Regulator 22.2V -> 5V", "Qty": 1, "Approx Price": "€2.00", "Notes": "Power for Teensy/Rx"},
    {"Component": "BEC 12V", "Description": "Step-Down Voltage Regulator 22.2V -> 12V", "Qty": 1, "Approx Price": "€2.00", "Notes": "Power for Actuators/Video"},
    {"Component": "PCB", "Description": "Custom PCB/Verification", "Qty": 1, "Approx Price": "€8.31", "Notes": "Custom carrier board"},
    {"Component": "Connectors", "Description": "XT60 Plugs", "Qty": 10, "Approx Price": "€20.00", "Notes": "Power distribution"}
]

df = pd.DataFrame(data)
output_path = r'd:\Repository\mech_421\Final_project\Build\BOM\Transformer_Robot_BOM.xlsx'
df.to_excel(output_path, index=False)
print(f"BOM saved to {output_path}")
