using System;
using Aemula.Debugging;

namespace Aemula
{
    public abstract class EmulatedSystem
    {
        public event EventHandler ProgramLoaded;

        protected void RaiseProgramLoaded()
        {
            ProgramLoaded?.Invoke(this, EventArgs.Empty);
        }

        public abstract ulong CyclesPerSecond { get; }

        public virtual void Reset() { }

        public abstract void LoadProgram(string filePath);

        public void RunForDuration(TimeSpan duration)
        {
            var clocks = duration.ToSystemTicks(CyclesPerSecond);

            for (var i = 0; i < clocks; i++)
            {
                Tick();
            }
        }

        public abstract void Tick();

        public virtual Debugger CreateDebugger() => null;
    }
}
