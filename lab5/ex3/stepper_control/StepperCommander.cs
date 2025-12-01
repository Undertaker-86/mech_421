using System;
using System.IO.Ports;

namespace StepperControl
{
    /// <summary>
    /// Helper for building and sending stepper control packets and tracking commanded angle.
    /// </summary>
    public class StepperCommander
    {
        private const byte StartByte = 255;
        private const int TimerClockHz = 1_000_000;
        public const int StepsPerRevolution = 400; // half-step resolution

        private readonly SerialPort _port;

        /// <summary>
        /// Current commanded angle in degrees (0-360), updated by MoveByAngle/MoveToAngle/CalibrateZero.
        /// </summary>
        public double CurrentAngleDeg { get; private set; }

        /// <summary>
        /// Create a commander that writes packets to the provided SerialPort.
        /// </summary>
        /// <param name="port">Open SerialPort to write to.</param>
        public StepperCommander(SerialPort port)
        {
            _port = port ?? throw new ArgumentNullException(nameof(port));
        }

        /// <summary>
        /// Convert a desired continuous speed in RPM (sign = direction) to a packet for dirByte 1/2.
        /// Returns the packet and outputs dirByte and TA1CCR0 used.
        /// </summary>
        public byte[] BuildContinuousPacketFromRpm(double rpm, out ushort ccr0, out byte dirByte)
        {
            dirByte = rpm >= 0 ? (byte)2 : (byte)1;
            double stepsPerSecond = Math.Abs(rpm) * StepsPerRevolution / 60.0;

            // Avoid divide-by-zero; clamp CCR0 into [1, 65535].
            if (stepsPerSecond < 1e-6)
            {
                stepsPerSecond = 1e-6;
            }

            double rawCcr0 = (TimerClockHz / stepsPerSecond) - 1.0;
            rawCcr0 = Math.Max(1.0, Math.Min(65535.0, rawCcr0));
            ccr0 = (ushort)Math.Round(rawCcr0);

            return BuildPacket(dirByte, ccr0);
        }

        /// <summary>
        /// Build a single-step packet (dirByte 3 for CCW, 4 for CW).
        /// CCR0 is zero and escape is zero for single steps.
        /// </summary>
        public byte[] BuildSingleStepPacket(bool clockwise)
        {
            byte dir = clockwise ? (byte)4 : (byte)3;
            return new byte[] { StartByte, dir, 0, 0, 0 };
        }

        /// <summary>
        /// Set the commanded zero reference angle to 0 degrees.
        /// </summary>
        public void CalibrateZero()
        {
            CurrentAngleDeg = 0;
        }

        /// <summary>
        /// Move by a signed angle (deg). Positive = CW, negative = CCW.
        /// Rounds to nearest half-step, sends that many single-step packets, and wraps angle to [0, 360).
        /// </summary>
        public void MoveByAngle(double deltaDeg)
        {
            int steps = (int)Math.Round(deltaDeg * StepsPerRevolution / 360.0);
            if (steps == 0)
            {
                return;
            }

            bool clockwise = steps > 0;
            int count = Math.Abs(steps);
            byte[] packet = BuildSingleStepPacket(clockwise);

            for (int i = 0; i < count; i++)
            {
                SendPacket(packet);
            }

            double executedDeg = steps * (360.0 / StepsPerRevolution);
            CurrentAngleDeg = NormalizeAngle(CurrentAngleDeg + executedDeg);
        }

        /// <summary>
        /// Move to an absolute target angle (deg). Uses the smallest signed delta (-180, 180] then calls MoveByAngle.
        /// </summary>
        public void MoveToAngle(double targetDeg)
        {
            double targetNorm = NormalizeAngle(targetDeg);
            double diff = targetNorm - CurrentAngleDeg;
            diff = NormalizeSigned180(diff);
            MoveByAngle(diff);
        }

        /// <summary>
        /// Send a packet over the configured SerialPort. Throws if the port is not open.
        /// </summary>
        public void SendPacket(byte[] packet)
        {
            if (packet == null) throw new ArgumentNullException(nameof(packet));
            if (_port == null || !_port.IsOpen)
            {
                throw new InvalidOperationException("Serial port is not open.");
            }
            _port.Write(packet, 0, packet.Length);
        }

        private static double NormalizeAngle(double deg)
        {
            double wrapped = deg % 360.0;
            if (wrapped < 0) wrapped += 360.0;
            return wrapped;
        }

        private static double NormalizeSigned180(double deg)
        {
            double wrapped = deg % 360.0;
            if (wrapped <= -180.0) wrapped += 360.0;
            if (wrapped > 180.0) wrapped -= 360.0;
            return wrapped;
        }

        private static byte[] BuildPacket(byte dirByte, ushort ccr0)
        {
            byte high = (byte)(ccr0 >> 8);
            byte low = (byte)(ccr0 & 0xFF);
            byte escape = 0;

            if (high == 255)
            {
                high = 0;
                escape |= 0b10;
            }

            if (low == 255)
            {
                low = 0;
                escape |= 0b01;
            }

            return new[] { StartByte, dirByte, high, low, escape };
        }
    }
}
