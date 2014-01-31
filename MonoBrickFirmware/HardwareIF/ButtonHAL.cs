using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.HardwareIF
{
#if !Ev3Simu
    #region Real hardware
    public class ButtonHAL
    {

        #region SingletonStuff
        // ensures that only one instance of this class can exist, so everybody writes and reads the same variables
        // http://geekswithblogs.net/BlackRabbitCoder/archive/2010/05/19/c-system.lazylttgt-and-the-singleton-design-pattern.aspx
        // static holder for instance, need to use lambda to construct since constructor private (DotNet4 only)
        private static readonly Lazy<ButtonHAL> _instance = new Lazy<ButtonHAL>(() => new ButtonHAL());

        private ButtonHAL()  // hiding creator
        {
            dev = new UnixDevice("/dev/lms_ui");
            buttonMem = dev.MMap(Buttons.ButtonCount, 0);
        }
        // accessor for instance
        public static ButtonHAL Init { get { return _instance.Value; } }

        #endregion //SingletonStuff

        UnixDevice dev;
        MemoryArea buttonMem;

        public Buttons.ButtonStates ReadButtons()
        {
            Buttons.ButtonStates bs = Buttons.ButtonStates.None;
            byte[] buttonData = buttonMem.Read();
            int bitMask = 1;
            foreach (byte butState in buttonData)
            {
                if (butState != 0)
                    bs |= (Buttons.ButtonStates)bitMask;
                bitMask = bitMask << 1;
            }
            return bs;
        }

        public void LedPattern(int pattern)
        {
            byte[] cmd = new byte[2];
            cmd[0] = (byte)('0' + pattern);
            dev.Write(cmd);
        }

        public void Dispose()
        {
            dev.Dispose();
        }
    }
    #endregion // real hardware
#else
    #region Simulated hardware

    public class ButtonHAL
    {
    #region SingletonStuff
        // ensures that only one instance of this class can exist, so everybody writes and reads the same variables
        // http://geekswithblogs.net/BlackRabbitCoder/archive/2010/05/19/c-system.lazylttgt-and-the-singleton-design-pattern.aspx
        // static holder for instance, need to use lambda to construct since constructor private (DotNet4 only)
        private static readonly Lazy<ButtonHAL> _instance = new Lazy<ButtonHAL>(() => new ButtonHAL());

        private ButtonHAL()  // hiding creator
        {
            // nothing to do
        } 
                // accessor for instance
        public static ButtonHAL Init { get { return _instance.Value; } }
 
    #endregion //SingletonStuff

        // Button stuff
        private static Buttons.ButtonStates _CurrentButton;

        public Buttons.ButtonStates CurrentButton { set { _CurrentButton = value; } get { return _CurrentButton; } }

        public Buttons.ButtonStates ReadButtons() { return _CurrentButton; }

        // Lighting stuff
        public enum LedColor { Red, Orange, Green }
        public bool LedBlinking;
        public bool LedOn;
        public LedColor _CurrentLedColor;
        
        public void LedPattern(int pattern)
        {
            //tbd
        }

        public void Dispose()
        {
            // nothing to dispose
        }
    }
    #endregion // Simulated hardware
#endif

}
