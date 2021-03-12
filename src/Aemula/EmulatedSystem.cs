using System;
using System.Collections.Generic;
using System.Linq;
using Aemula.UI;

namespace Aemula
{
    public abstract class EmulatedSystem
    {
        public event EventHandler ProgramLoaded;

        protected void RaiseProgramLoaded()
        {
            ProgramLoaded?.Invoke(this, EventArgs.Empty);
        }

        public abstract ushort LastPC { get; }

        public virtual void Reset() { }

        public abstract void LoadProgram(string filePath);

        public abstract void RunForDuration(TimeSpan duration);
        public abstract void StepInstruction();
        public abstract void StepCpuCycle();

        public abstract SortedDictionary<ushort, DisassembledInstruction> Disassemble();

        public virtual IEnumerable<DebuggerWindow> CreateDebuggerWindows()
        {
            return Enumerable.Empty<DebuggerWindow>();
        }
    }
}
