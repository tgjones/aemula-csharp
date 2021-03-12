using System;
using System.Collections.Generic;
using Aemula.UI;

namespace Aemula
{
    public sealed class Emulator
    {
        private TimeSpan _lastTime;

        public readonly EmulatedSystem System;

        public bool Running;

        public Emulator(EmulatedSystem system)
        {
            System = system;

            system.Reset();

            Running = true;
        }

        public void Update(EmulatorTime time)
        {
            if (_lastTime == TimeSpan.Zero)
            {
                _lastTime = time.TotalTime;
            }

            var deltaTime = time.TotalTime - _lastTime;

            _lastTime = time.TotalTime;

            if (!Running)
            {
                return;
            }

            System.RunForDuration(deltaTime);
        }

        public void Draw(EmulatorTime time)
        {
            // TODO: Draw actual screen.
        }

        public IEnumerable<DebuggerWindow> CreateDebuggerWindows()
        {
            foreach (var debuggerWindow in System.CreateDebuggerWindows())
            {
                yield return debuggerWindow;
            }

            yield return new DisassemblyWindow(this);
        }
    }
}
