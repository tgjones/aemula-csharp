using System.Collections.Generic;
using Aemula.Clock.UI;
using Aemula.UI;

namespace Aemula.Clock
{
    public sealed class Clock
    {
        private readonly ulong _masterClockSpeed;

        private readonly ClockableDevice[] _attachedDevices;
        private readonly ulong[] _nextUpdates;

        private ulong _masterClock;

        public ulong MasterClock => _masterClock;

        public bool IsRunning;

        public Clock(ulong masterClockSpeed, params ClockableDevice[] clockableDevices)
        {
            _masterClockSpeed = masterClockSpeed;

            _attachedDevices = clockableDevices;
            _nextUpdates = new ulong[clockableDevices.Length];

            IsRunning = true;
        }

        public void Update(EmulatorTime time)
        {
            if (!IsRunning)
            {
                return;
            }

            // Figure out how many (whole number) clock cycles we can execute in this time.
            var clockCycles = (ulong)(_masterClockSpeed * time.ElapsedTime.TotalSeconds);
            var endClock = _masterClock + clockCycles;

            while (_masterClock < endClock)
            {
                Cycle();
            }
        }

        public void Cycle()
        {
            for (int i = 0; i < _attachedDevices.Length; i++)
            {
                ref var clockableDevice = ref _attachedDevices[i];
                if (_masterClock == _nextUpdates[i])
                {
                    clockableDevice.Device.Cycle();
                    _nextUpdates[i] += clockableDevice.ClockSpeedDivider;
                }
            }

            _masterClock++;
        }

        public IEnumerable<DebuggerWindow> CreateDebuggerWindows()
        {
            yield return new ClockWindow(this);
        }
    }
}
