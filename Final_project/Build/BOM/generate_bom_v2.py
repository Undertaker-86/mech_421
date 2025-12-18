
import pandas as pd

# Generic search links to ensure they work long-term
data = [
    {
        "Component": "Servo Motor", 
        "Description": "DS3225 25kg High Torque Servo 180°", 
        "Qty": 2, 
        "Approx Price": "€28.78", 
        "Notes": "Main body transformation",
        "Link": "https://www.amazon.com/s?k=DS3225+25kg+servo"
    },
    {
        "Component": "Drone Motor", 
        "Description": "Brushless Motor 2812 Pro 1100KV", 
        "Qty": 4, 
        "Approx Price": "€44.00", 
        "Notes": "Flight motors (4x). EMAX Pro Series or ECO II recommended.",
        "Link": "https://www.aliexpress.com/w/wholesale-emax-2812-1100kv.html"
    },
    {
        "Component": "Drive Motor", 
        "Description": "Brushless Motor 5010 360KV", 
        "Qty": 2, 
        "Approx Price": "€34.58", 
        "Notes": "Wheel drive motors. High torque pancake style.",
        "Link": "https://www.aliexpress.com/w/wholesale-5010-360kv-motor.html"
    },
    {
        "Component": "Linear Actuator", 
        "Description": "Micro Linear Actuator 12V 100mm Stroke 150N+", 
        "Qty": 2, 
        "Approx Price": "€35.40", 
        "Notes": "STRONGLY RECOMMEND 150N or 188N version (60N is too weak). Check specific dimensions.",
        "Link": "https://www.aliexpress.com/w/wholesale-12v-micro-linear-actuator-100mm-150n.html"
    },
    {
        "Component": "Actuator Driver", 
        "Description": "DRV8871 H-Bridge Driver", 
        "Qty": 2, 
        "Approx Price": "€6.30", 
        "Notes": "Controls linear actuators",
        "Link": "https://www.amazon.com/s?k=DRV8871+driver"
    },
    {
        "Component": "Propellers", 
        "Description": "8x4.5 or 9x5 Propellers (CW/CCW)", 
        "Qty": 4, 
        "Approx Price": "€3.68", 
        "Notes": "Get a mix of CW and CCW. Suggest 9 inch for better lift if fits.",
        "Link": "https://www.amazon.com/s?k=9050+propellers+cw+ccw"
    },
    {
        "Component": "Flight ESC", 
        "Description": "4-in-1 45A Brushless ESC Stack", 
        "Qty": 1, 
        "Approx Price": "€30.00", 
        "Notes": "30x30 or 20x20 mounting depending on your frame design",
        "Link": "https://www.getfpv.com/electronics/electronic-speed-controllers-esc/4-in-1-escs.html"
    },
    {
        "Component": "Drive ESC", 
        "Description": "35A Bidirectional ESC", 
        "Qty": 2, 
        "Approx Price": "€20.58", 
        "Notes": "Must support reverse for tank drive.",
        "Link": "https://www.amazon.com/s?k=bidirectional+brushless+esc+35a"
    },
    {
        "Component": "Bearings", 
        "Description": "12x8x3.5 mm Bearings (MR128-2RS)", 
        "Qty": 6, 
        "Approx Price": "€7.20", 
        "Notes": "Standard MR128 ball bearings",
        "Link": "https://www.amazon.com/s?k=mr128-2rs+bearing"
    },
    {
        "Component": "Chassis Plate", 
        "Description": "Carbon Fiber Plate 2mm 300x200mm", 
        "Qty": 2, 
        "Approx Price": "€160.00", 
        "Notes": "For CNC cutting custom frame parts",
        "Link": "https://www.amazon.com/s?k=carbon+fiber+sheet+2mm"
    },
    {
        "Component": "Battery", 
        "Description": "6S LiPo 2200mAh 100C+", 
        "Qty": 2, 
        "Approx Price": "€50.00", 
        "Notes": "High C-rating critical for heavy drone. XT60 connector.",
        "Link": "https://www.amazon.com/s?k=6s+lipo+2200mah+100c"
    },
    {
        "Component": "Controller", 
        "Description": "Teensy 4.0", 
        "Qty": 1, 
        "Approx Price": "€30.40", 
        "Notes": "For custom flight code",
        "Link": "https://www.pjrc.com/store/teensy40.html"
    },
    {
        "Component": "Connectors", 
        "Description": "XT60 Plugs Pairs", 
        "Qty": 10, 
        "Approx Price": "€20.00", 
        "Notes": "Power distribution",
        "Link": "https://www.amazon.com/s?k=xt60+connectors"
    }
]

df = pd.DataFrame(data)
output_path = r'd:\Repository\mech_421\Final_project\Build\BOM\Transformer_Robot_BOM_v2.xlsx'
df.to_excel(output_path, index=False)
print(f"BOM saved to {output_path}")
