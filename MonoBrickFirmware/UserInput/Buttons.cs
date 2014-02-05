using System;
using MonoBrickFirmware.Tools;
using System.Threading;
using MonoBrickFirmware.HardwareIF;

namespace MonoBrickFirmware.UserInput
{
    public class Buttons : IDisposable
    {
        [Flags]
        public enum ButtonStates
        {
            None = 0x00,
            Up = 0x01,
            Enter = 0x02,
            Down = 0x04,
            Right = 0x08,
            Left = 0x10,
            Escape = 0x20,
            All = 0xff,
        }

        public const int ButtonCount = 6;
        const int debounceTime = 10;
        const int pollTime = 50;
        private ButtonHAL _ButtonHAL;

        public Buttons()
        {
            _ButtonHAL = ButtonHAL.Init;
        }

        ButtonStates ReadButtons()
        {
            return _ButtonHAL.ReadButtons();
        }

        internal ButtonStates GetDebounced()
        {

            ButtonStates s2 = ButtonStates.None;
            for (; ; )
            {
                ButtonStates s1 = ReadButtons();
                if (s1 == s2)
                    return s1;
                Thread.Sleep(debounceTime);
                s2 = ReadButtons();
            }
        }

        public ButtonStates GetButtonStates()
        {
            return GetDebounced();
        }

        public void WaitForKeyRelease(CancellationToken token)
        {
            ButtonStates bs = GetDebounced();
            do
            {
                Thread.Sleep(pollTime);
                bs = GetDebounced();
            } while (bs != ButtonStates.None && !token.IsCancellationRequested);
        }

        public void WaitForKeyRelease()
        {
            WaitForKeyRelease(new CancellationToken(false));
        }

        public ButtonStates GetKeypress(CancellationToken token)
        {
            ButtonStates bs = GetDebounced();
            while (bs != ButtonStates.None && !token.IsCancellationRequested)
            {
                bs = GetDebounced();
            }
            do
            {
                Thread.Sleep(pollTime);
                bs = GetDebounced();
            } while (bs == ButtonStates.None && !token.IsCancellationRequested);
            return bs;
        }

        public ButtonStates GetKeypress()
        {
            return GetKeypress(new CancellationToken(false));
        }


        public void LedPattern(int pattern)
        {
            _ButtonHAL.LedPattern(pattern);
        }

        #region IDisposable implementation

        void IDisposable.Dispose()
        {
            _ButtonHAL.Dispose();
        }

        #endregion

    }

}
