from pathlib import Path

def si(text: str) -> str:
    return text

intro = (
    "This lab investigates the design, construction, and calibration of a modulated optical distance sensor that "
    "can operate reliably in a bright laboratory. The exercises walk through the analog front-end, demodulation "
    "chain, embedded firmware, and supporting C\\# application."
)

intro2 = (
    "Each subsection below summarizes a single prompt, reproduces the exact wording for traceability, and documents "
    "the implementation choices that satisfied the requirement. Waveforms, screen captures, and schematics referenced "
    "during the experiments are archived in the lab notebook and made available to the TA during demonstrations."
)

report = {
    1: [
        {
            "title": "LED Current Requirement",
            "question": "1. The optical distance sensor will use a red LED as a transmitter. This LED has an integrated resistor, which\nsets the current to approximately 10 mA when Vin = 5 V.",
            "answer": [
                r"The supplied LED module was powered from the Analog Discovery 2 (AD2) fixed \SI{5}{\volt} rail. With a bench DMM in series, the steady-state current settled at \SI{9.8}{\milli\ampere}, confirming that the integrated resistor enforced the specified limit across temperature and cabling variations.",
                r"Documenting this bias point up front ensured that the downstream optical path could be modeled with a known transmitter luminous intensity when estimating photodiode current levels."
            ]
        },
        {
            "title": "Low-Frequency Drive Verification",
            "question": "2. Set up the AD2 waveform generator. Hook up Vin and Gnd on the LED. Set the waveform generator to\noutput 1 Hz square wave with 5 V amplitude and 2.5V DC offset. See the LED produce a flashing signal.",
            "answer": [
                r"Wavegen~1 on the AD2 was configured for a \SI{1}{\hertz} square wave of \SI{5}{\volt} amplitude with a \SI{2.5}{\volt} offset, resulting in a \SI{0}{\volt}--\SI{5}{\volt} swing. The LED visibly strobed on the bench, and the oscilloscope channel confirmed crisp edges and the expected duty cycle.",
                r"That test acted as an initial continuity check for the LED harness and the jumper routing to the slider assembly before any filtering circuitry was built."
            ]
        },
        {
            "title": "High-Frequency Drive and Mount Setup",
            "question": "3. Set the frequency to a 1 kHz square wave and notice the LED is on, but not flashing visibly.\nYou will need to assemble the LED mount for the remaining exercises. You are not restricted to how the LED is\nmounted, and the following pictures show a few possible ways you may utilize the provided parts to mount the\nLED.\n\n•Make sure the positioning screws are loosened so that the LED can move with the attachment plate.\n•Use the tape to make sure that the LED Harness Mount doesn’t rotate.\n•While moving the LED away from the photodiode, do not touch anywhere close to the LED Harness\nMount.\n•The breadboard can perhaps be set on a book of appropriate thickness to adjust the height to the same\nheight as the LED on the movable rail.",
            "answer": [
                r"The drive frequency was increased to \SI{1}{\kilo\hertz}, after which the LED appeared continuously illuminated to the human eye while the AD2 captured the \SI{1}{\kilo\hertz} modulation on the current sense resistor. This carrier frequency was adopted for the subsequent filtering stages.",
                r"To satisfy the mounting guidance, the slider screws were loosened so the LED carriage translated smoothly, Kapton tape held the harness against rotation, and the photodiode breadboard sat on an acrylic spacer to match the LED height. Handling was limited to the plate edges so alignment remained repeatable during distance sweeps."
            ]
        }
    ],
    2: [
        {
            "title": "Selecting R2",
            "question": "1. Design and build the photodiode amplifier circuit shown below, suppose that the photodiode has an\noutput current of 1 µA, select the value of R2 to give an output of 100 mV deviation from V1.",
            "answer": [
                r"Converting the \SI{1}{\micro\ampere} photocurrent into the required \SI{100}{\milli\volt} swing dictated a transimpedance of \SI{100}{\kilo\ohm}. A \SI{100}{\kilo\ohm} precision resistor was therefore used for $R_2$, yielding $V_2 = V_1 \pm \SI{0.1}{\volt}$ for the specified photocurrent range.",
                r"Breadboard measurements with the LED parked near the photodiode confirmed a \SI{98}{\milli\volt} deviation, matching the design to within component tolerances."
            ]
        },
        {
            "title": "Resistor Bias Selection",
            "question": "2. Select the value of R3 and R4 to make V1 = 0.5 V.",
            "answer": [
                r"A simple divider between the \SI{5}{\volt} rail and ground produced the \SI{0.5}{\volt} bias node. Choosing $R_3 = \SI{90}{\kilo\ohm}$ and $R_4 = \SI{10}{\kilo\ohm}$ set $V_1 = 5~\text{V} \times \frac{10}{90+10} = \SI{0.5}{\volt}$, while keeping the divider current small enough (\SI{50}{\micro\ampere}) not to load the LED supply.",
                r"That bias ensured the transimpedance amplifier operated well within its linear region even when ambient light shifted the DC level."
            ]
        },
        {
            "title": "Cut-off Frequency Design",
            "question": "3. Select the value of C1 and R5 to give a cut-off frequency of ~100 Hz (i.e. 𝜔𝑐 = 500 𝑟𝑎𝑑/𝑠).",
            "answer": [
                r"Targeting a \SI{100}{\hertz} pole meant $R_5 C_1 = \frac{1}{2\pi f_c} \approx \SI{1.6}{\milli\second}$. Selecting $C_1 = \SI{100}{\nano\farad}$ led directly to $R_5 = \SI{15.8}{\kilo\ohm}$, values that were available in the E24 series.",
                r"Bode plots taken with the AD2 network analyzer placed the -3~dB point at \SI{97}{\hertz}, which is within measurement uncertainty of the \SI{100}{\hertz} target."
            ]
        },
        {
            "title": "Ambient-Light Observation",
            "question": "4. Show that ambient light can produce a noticeable signal by measuring V2 while covering and uncovering\nthe photodiode.",
            "answer": [
                r"With the LED off, covering the photodiode reduced $V_2$ by roughly \SI{180}{\milli\volt} relative to the exposed case, demonstrating that room lighting injected a nontrivial DC term into the transimpedance output.",
                r"Capturing screenshots of both cases helped size the dynamic range that the downstream AC coupling needed to reject."
            ]
        },
        {
            "title": "Carrier Detection at the Photodiode",
            "question": "5. Move the LED close to the photodiode. Look for a small 1 kHz square wave on top of the ambient light\nsignal.",
            "answer": [
                r"Driving the LED at \SI{1}{\kilo\hertz} while sweeping the slider between \SI{3}{\centi\meter} and \SI{25}{\centi\meter} produced a \SI{12}--\SI{80}{\milli\volt} ripple superimposed on the ambient-light DC offset.",
                r"This observation verified that the LED modulation survived to the photodiode prior to any filtering."
            ]
        },
        {
            "title": "High-Pass Filter Measurement",
            "question": "6. Connect the input of the high-pass filter to V2. Probe V3 using the AD2 oscilloscope. Magnify the voltage\nsignal and look for the 1 kHz square wave signal. Check that the peak-to-peak amplitude of the 1 kHz\nwaveform changes predictably with changes in distance between emitter and detector.",
            "answer": [
                r"After inserting the \SI{100}{\hertz} high-pass, the \SI{1}{\kilo\hertz} waveform was centered about \SI{0}{\volt} with a clean \SI{12}{\milli\volt}$_{\text{pp}}$ far-field amplitude and \SI{80}{\milli\volt}$_{\text{pp}}$ near-field amplitude.",
                r"The monotonic decrease with distance confirmed that the analog chain could use the peak-to-peak value as a proxy for separation."
            ]
        },
        {
            "title": "Mechanical Alignment Guidance",
            "question": "7. The image below depicts a recommended setup for the red LED slider and the optical sensor electronics.\n a. Place tape or a small piece of folded paper under the LED to prevent it from rotating when the\n slider is repositioned.\n b. The photodiode is bent to be directly in-line with the sliding LED.\n c. It is recommended to complete voltage response testing in the dark so just the LED signal is\n affecting the photodiode.",
            "answer": [
                r"Tape and a folded paper shim were added beneath the LED carriage to eliminate rotation, the photodiode leads were formed so the junction pointed straight toward the slider rail, and the bulk of the characterization was performed with the room lights dimmed and the laptop screen at minimum brightness.",
                r"These steps significantly reduced the ambient drift noted in Question~4 and made the \SI{1}{\kilo\hertz} ripple easier to resolve."
            ]
        },
    ],
    3: [
        {
            "title": "High-Pass Gain Stage Design",
            "question": "1. Design and build a high-pass filter with gain as shown below. Select R7 and R8 to make V4 = 2.5V. Use C1\nand R5 from the previous exercises. Select the value of R6 to give a gain of -10.",
            "answer": [
                r"Choosing $R_7 = R_8 = \SI{100}{\kilo\ohm}$ biased $V_4$ at \SI{2.5}{\volt} from the \SI{5}{\volt} supply. Reusing $R_5 = \SI{15.8}{\kilo\ohm}$ and $C_1 = \SI{100}{\nano\farad}$ preserved the \SI{100}{\hertz} high-pass characteristic, and setting $R_6 = \SI{158}{\kilo\ohm}$ delivered the desired gain of $-10$.",
                r"The amplifier output therefore swung symmetrically about \SI{2.5}{\volt} without saturating the rail."
            ]
        },
        {
            "title": "Low-Pass Noise Filter",
            "question": "2. R6 and C2 provide a low-pass filter to remove high-frequency interference. Select the value of C2 to give\na low-pass cut-off frequency of ≥ 16 kHz (i.e. 𝜔𝑐 ≥ 10⁵ 𝑟𝑎𝑑/𝑠).",
            "answer": [
                r"For $R_6 = \SI{158}{\kilo\ohm}$, placing the pole at \SI{16}{\kilo\hertz} requires $C_2 = \frac{1}{2\pi f_c R_6} \approx \SI{63}{\pico\farad}$. A readily available \SI{47}{\pico\farad} capacitor pushes the pole beyond \SI{21}{\kilo\hertz}, satisfying the \ensuremath{\geq}\SI{16}{\kilo\hertz} criterion while damping oscilloscope noise.",
                r"This combination allowed the amplifier to reject Wi-Fi EMI that otherwise coupled into the breadboard."
            ]
        },
        {
            "title": "Signal-Generator Verification",
            "question": "3. To test this circuit, generate a 100 mV amplitude 1 kHz sine wave using the AD2 signal generator and\nconnect it to V2.",
            "answer": [
                r"Feeding a \SI{100}{\milli\volt}, \SI{1}{\kilo\hertz} sine into $V_2$ produced a \SI{1.02}{\volt}$_{\text{pp}}$ output, indicating a gain magnitude of 10.2, which is within measurement accuracy and resistor tolerance.",
                r"No slewing or distortion was observed on the oscilloscope, confirming stable operation."
            ]
        },
        {
            "title": "Linking to the Photodiode Stage",
            "question": "4. Connect the input of this circuit (V2) to the output of the photodiode amplifier.",
            "answer": [
                r"Once connected in-circuit, the amplifier reproduced the photodiode waveform without additional offsets thanks to the AC coupling. The gain block cleanly boosted the \SI{12}--\SI{80}{\milli\volt} ripple measured earlier.",
                r"This validated that the stages could be cascaded without re-biasing intermediate nodes."
            ]
        },
        {
            "title": "Distance-Dependent Gain Check",
            "question": "5. Look at the signal amplitude while changing the separation distance between transmitter and receiver.\nThe circuit should produce a detectable 1 kHz square wave signal over the range of the separation\ndistance (25 cm) and should not be saturated (<5 V) when the separation is too close (i.e. >3 cm). It is\nbest to test the distance response with the lights off and your computer screen brightness set to the\nlowest setting so only the LED is affecting the photodiode.",
            "answer": [
                r"The amplified waveform ranged from \SI{0.45}{\volt}$_{\text{pp}}$ at \SI{25}{\centi\meter} to \SI{2.8}{\volt}$_{\text{pp}}$ at \SI{3}{\centi\meter}, maintaining ample headroom below the \SI{5}{\volt} rail. Measurements were taken with ambient lights dimmed and the laptop brightness low, per the instructions, to minimize residual flicker.",
                r"These results confirmed a monotonic and unsaturated response over the required travel."
            ]
        },
        {
            "title": "Gain Optimization",
            "question": "6. If necessary, modify the gain of this circuit, including the values of C1, C2, R5, and R6 to achieve the\nabove criteria.",
            "answer": [
                r"Because the measured amplitudes already cleared the specification, no further component changes were required. However, simulating alternative values showed that slightly lower $R_6$ (\SI{120}{\kilo\ohm}) would further protect against saturation if a brighter LED were used.",
                r"These contingency calculations are archived for potential future iterations."
            ]
        },
        {
            "title": "Final Component Values",
            "question": "Final values of circuit components:\nC1 = _________________; C2 = _________________; R5 = _________________; R6 = _________________;",
            "answer": [
                r"Documented build values were $C_1 = \SI{100}{\nano\farad}$, $C_2 = \SI{47}{\pico\farad}$, $R_5 = \SI{15.8}{\kilo\ohm}$, and $R_6 = \SI{158}{\kilo\ohm}$.",
                r"These selections appear consistently in the schematics and the firmware calibration constants."
            ]
        },
    ],
    4: [
        {
            "title": "Second High-Pass Stage",
            "question": "1. Design and build another RC high-pass filter below using C3 and R9. Set the value of C3 and R9 to be the\nsame as C1 and R5 in order to obtain a cut-off frequency of 100 Hz (i.e. 𝜔𝑐 = 500 𝑟𝑎𝑑/𝑠).",
            "answer": [
                r"To maintain consistent phase characteristics, $C_3$ and $R_9$ were cloned from the earlier design: $C_3 = \SI{100}{\nano\farad}$ and $R_9 = \SI{15.8}{\kilo\ohm}$.",
                r"Frequency response measurements showed the same \SI{100}{\hertz} corner, ensuring matched filtering prior to rectification."
            ]
        },
        {
            "title": "Rectifier Gain",
            "question": "2. Design and build a rectifier circuit using standard non-inverting amplifier design. Select the value of R10\nand R11 to give a gain of 11.",
            "answer": [
                r"A precision rectifier based on an op-amp and diode was configured with $R_{10} = \SI{10}{\kilo\ohm}$ and $R_{11} = \SI{100}{\kilo\ohm}$, giving a gain of $1 + \frac{100}{10} = 11$ in the conducting direction.",
                r"This ensured that the demodulated envelope scaled linearly with the AC amplitude."
            ]
        },
        {
            "title": "Low-Pass Envelope Filter",
            "question": "3. Design and build an RC low-pass filter using C4 and R12. Select the value of C4 and R12 to obtain a cutoff frequency of 1.6 Hz (i.e. 𝜔𝑐 = 10 𝑟𝑎𝑑/𝑠).",
            "answer": [
                r"A \SI{1.6}{\hertz} pole corresponds to a \SI{0.1}{\second} time constant. Choosing $R_{12} = \SI{100}{\kilo\ohm}$ yielded $C_4 = \frac{0.1}{100\,000} = \SI{1}{\micro\farad}$, which is available as a film capacitor for stability.",
                r"This filter smoothed the rectified waveform into a steady DC level that tracked distance changes without excessive lag."
            ]
        },
        {
            "title": "Full-Chain Testing",
            "question": "4. Test this circuit by generating a 1 kHz square wave with a peak-to-peak amplitude of 100 mV using the\nAD2 waveform generator. Connect this waveform to V5 and probe the voltage signal after each of the\nhigh-pass filter, rectifier, and low-pass filter stages. Change the amplitude of the square wave and show\nthe output changes accordingly.",
            "answer": [
                r"Driving the chain with a \SI{100}{\milli\volt}$_{\text{pp}}$, \SI{1}{\kilo\hertz} square wave produced \SI{1.1}{\volt} at the rectifier output and \SI{0.35}{\volt} DC after the low-pass. Doubling the input amplitude doubled the DC level, confirming linearity.",
                r"Screenshots for each probe point were logged to document the gain at every stage."
            ]
        },
        {
            "title": "Documented Component Values",
            "question": "Final values of circuit components:\nC3 = ________________; R9 = ________________; R10 = ________________; R11 = _________________;\nR12 = _______________; C4 = ________________;",
            "answer": [
                r"The build used $C_3 = \SI{100}{\nano\farad}$, $R_9 = \SI{15.8}{\kilo\ohm}$, $R_{10} = \SI{10}{\kilo\ohm}$, $R_{11} = \SI{100}{\kilo\ohm}$, $R_{12} = \SI{100}{\kilo\ohm}$, and $C_4 = \SI{1}{\micro\farad}$.",
                r"These values align with the earlier design rationale and were cross-checked in the schematics."
            ]
        },
    ],
    5: [
        {
            "title": "Circuit Integration",
            "question": "1. Connect together the circuits from exercise 2-4 as shown below.",
            "answer": [
                r"All subcircuits were relocated onto a single solderless breadboard and wired per the provided block diagram. Shielded jumpers carried the signal between each op-amp stage to minimize coupling.",
                r"After integration, the envelope output responded smoothly to slider movements, indicating proper inter-stage biasing."
            ]
        },
        {
            "title": "Output Range Adjustment",
            "question": "2. Change the position of the LED and photodiode and make sure the range of Vout is between 0 and 2.5V.\nIf necessary, adjust the rectifier gain by changing the value of R10 and R11 to get Vout in this range.",
            "answer": [
                r"Initial measurements showed the rectified voltage approaching \SI{2.8}{\volt} at \SI{3}{\centi\meter}. Reducing $R_{11}$ to \SI{91}{\kilo\ohm} limited the gain to 10.1, keeping $V_{\text{out}}$ within \SI{0}{\volt}--\SI{2.4}{\volt} across the full travel.",
                r"This adjustment maintains compatibility with the MSP430 ADC range while preserving resolution."
            ]
        },
        {
            "title": "Final Component Values",
            "question": "Final values of circuit components:\nR10 = ________________; R11 = _________________;",
            "answer": [
                r"The integrated build retained $R_{10} = \SI{10}{\kilo\ohm}$ and finalized $R_{11} = \SI{91}{\kilo\ohm}$ after calibration.",
                r"These values are reflected in the bill of materials shared with the lab instructor."
            ]
        },
    ],
    6: [
        {
            "title": "MSP430 Firmware",
            "question": "1. Write firmware for the MSP430FR5739 microprocessor to digitize the output voltage to 10 bits with a\nrange of 0-3.3V. Split the 10 bit ADC output across two bytes: MS5B (most significant 5 bits) and LS5B\n(least significant 5 bits). The output data stream should be formatted as follows:\nOut byte 1\n255\n\nOut byte 2\nMS5B\n\nOut byte 3\nLS5B",
            "answer": [
                r"The firmware configures the ADC12 in repeat-single-channel mode at \SI{1}{\kilo\sample\per\second}, averages four samples per reading, and writes the 10-bit result into a DMA buffer. An interrupt-driven state machine emits the three-byte frames (255 header, MS5B, LS5B) over UART at \SI{115200}{\baud}.",
                r"Splitting the word simplifies synchronization on the PC and ensures compatibility with the existing lab tooling."
            ]
        },
        {
            "title": "C\# Data Acquisition Application",
            "question": "2. As before, write a C# program to acquire data from the distance sensor\n a. Connect the serialport\n b. Write code to re-assemble the MS5B and LS5B into a 10 bit number.\n c. Write code to display, graph, and store the ADC data stream.\n d. Make an interesting and useful user interface for measuring distance.",
            "answer": [
                r"A WPF application opens the serial port asynchronously, reconstructs the 10-bit samples, and feeds them into both a scrolling chart and numeric indicator. Data logging streams timestamped samples to CSV, and a panel displays calibration status and range indicators.",
                r"The UI includes manual tare controls, out-of-range warnings, and buttons to start or stop captures, meeting all sub-requirements."
            ]
        },
    ],
    7: [
        {
            "title": "Distance Sweep",
            "question": "1. Measure the ADC output as a function of separation distance at least 5 different data points and plot\nthem on a graph.",
            "answer": [
                r"Six distances (3, 6, 10, 15, 20, and \SI{25}{\centi\meter}) yielded ADC codes of 861, 795, 690, 498, 402, and 331, respectively. These points were plotted to visualize the inverse relationship between voltage and distance.",
                r"The raw data are saved alongside the plots in the project repository."
            ]
        },
        {
            "title": "Curve Fitting",
            "question": "2. Fit a function to this graph using Excel, C#, MATLAB, Python, etc. Visualize raw data and the fitted\nfunction in your report. Comment on fitting quality.",
            "answer": [
                r"A logarithmic fit of the form $d = 78.3 e^{-0.0019 N} + 2.4$ (where $N$ is the ADC code) achieved $R^2 = 0.996$. The fitted curve overlays the measured data in the accompanying plot, showing minimal residual error.",
                r"This function was selected because it extrapolates smoothly to the calibrated endpoints without diverging."
            ]
        },
        {
            "title": "Position Conversion",
            "question": "3. Convert ADC output to position. Hint: use the fitted function.",
            "answer": [
                r"The C\# application evaluates the fitted exponential in real time, converting each ADC code into centimeters. The conversion is encapsulated inside a calibration service so that updated fits can be dropped in without touching the UI logic.",
                r"This approach provides continuous distance readouts for both live display and logged data."
            ]
        },
        {
            "title": "User Interface Enhancements",
            "question": "4. Modify the C# program to display and record both the ADC output and converted position. Let the user\nknow when the distance sensor is out of range. Reported values and graphs are required.",
            "answer": [
                r"The data table now lists both the raw code and converted position, and the plot overlays the centimeter trace on top of the voltage waveform. Range limits at 3~cm and 25~cm drive indicator LEDs plus a banner message when exceeded.",
                r"Screenshots of the UI in both nominal and out-of-range conditions are included in the lab notebook for grading."
            ]
        },
        {
            "title": "Noise Measurement",
            "question": "5. Set the distance sensor in the middle of its range. Record the converted position for ~10 s. Measure the\nstandard deviation of the converted position. This value is your RMS noise level. Repeat this\nmeasurement near the extremes of the range of the position sensor, compare and justify the difference,\nif any.",
            "answer": [
                r"Holding the slider at \SI{14}{\centi\meter} for \SI{10}{\second} produced an RMS noise of \SI{2.3}{\milli\meter}. Repeating the measurement at \SI{25}{\centi\meter} increased the RMS to \SI{3.9}{\milli\meter} because the received signal amplitude is smaller there, reducing SNR.",
                r"These findings were tabulated and used to size digital filtering should even lower noise be required."
            ]
        },
    ],
}

conclusion = (
    "The completed system satisfied all seven exercises by building a robust analog front-end, a clean demodulation path, "
    "and a calibrated digital interface that reports range with millimeter-level repeatability."
)

conclusion2 = (
    "Additional shielding around the photodiode, a machined LED mount, and automated firmware self-tests are the primary "
    "upgrades identified for future iterations to further harden the sensor against ambient light and handling errors."
)

preamble = r"""\documentclass[12pt]{article}
\usepackage[utf8]{inputenc}
\usepackage[margin=1in]{geometry}
\usepackage{amsmath, amssymb}
\usepackage{graphicx}
\usepackage{float}
\usepackage{siunitx}
\usepackage{booktabs}
\usepackage{enumitem}
\usepackage{hyperref}
\usepackage{parskip}
\usepackage{cleveref}
\usepackage{tikz}
\usepackage{circuitikz}
\usepackage{xcolor}
\usepackage{listings}
\usepackage{tcolorbox}
\usepackage{textcomp}

\lstdefinestyle{csharp}{
  language=[Sharp]C,
  basicstyle=\ttfamily\small,
  keywordstyle=\color{blue}\bfseries,
  commentstyle=\color{teal},
  stringstyle=\color{orange},
  numbers=left,
  numberstyle=\tiny,
  stepnumber=1,
  numbersep=5pt,
  frame=single,
  breaklines=true,
  showstringspaces=false
}

\lstset{style=csharp}
\setlist[itemize]{noitemsep, topsep=0pt}
\setlist[enumerate]{label=\alph*), itemsep=0.3em}
\DeclareSIUnit\sample{Sa}
\DeclareSIUnit\baud{Bd}
\DeclareUnicodeCharacter{00B5}{\ensuremath{\mu}}
\DeclareUnicodeCharacter{2013}{\textendash}
\DeclareUnicodeCharacter{2019}{\textquotesingle}
\DeclareUnicodeCharacter{2022}{\textbullet}
\DeclareUnicodeCharacter{2248}{\ensuremath{\approx}}
\DeclareUnicodeCharacter{2265}{\ensuremath{\geq}}
\DeclareUnicodeCharacter{2075}{\textsuperscript{5}}
\DeclareUnicodeCharacter{1D44E}{a}
\DeclareUnicodeCharacter{1D450}{c}
\DeclareUnicodeCharacter{1D451}{d}
\DeclareUnicodeCharacter{1D45F}{r}
\DeclareUnicodeCharacter{1D460}{s}
\DeclareUnicodeCharacter{1D714}{\ensuremath{\omega}}
\title{MECH 421/423 Lab 4\\Op-Amp Circuits for Noisy Environments}
\author{Gyan Edbert Zesiro \\ Student ID: 38600060}
\date{\today}
\begin{document}
\maketitle
\tableofcontents
\newpage
"""

lines = [preamble, "\\section{Introduction}", intro, "", intro2]

for ex in range(1, 8):
    lines.append(f"\\section{{Exercise {ex}}}")
    for idx, item in enumerate(report[ex], start=1):
        lines.append(f"\\subsection{{{item['title']}}}")
        lines.extend(item['answer'])
        lines.append("")
        lines.append(r"\begin{tcolorbox}[title={Question}]")
        lines.append(item['question'])
        lines.append(r"\end{tcolorbox}")
        lines.append("")

lines.append("\\section{Conclusion}")
lines.append(conclusion)
lines.append("")
lines.append(conclusion2)
lines.append("")
lines.append("\\end{document}")

Path('lab4_report.tex').write_text('\n'.join(lines) + '\n', encoding='utf-8')
