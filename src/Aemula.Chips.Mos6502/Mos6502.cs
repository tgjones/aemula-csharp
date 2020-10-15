using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Aemula.Chips.Mos6502.UI;
using Aemula.UI;

namespace Aemula.Chips.Mos6502
{
    public sealed partial class Mos6502
    {
        // Registers
        public byte A;
        public byte X;
        public byte Y;

        // Program counter
        public SplitUInt16 PC;

        // Stack pointer
        public byte SP;

        // Processor flags
        public ProcessorFlags P;

        /// <summary>
        /// Instruction register - stores opcode of instruction being executed.
        /// </summary>
        private byte _ir;

        /// <summary>
        /// Timing register - stores the progress through the current instruction, from 0 to 7.
        /// </summary>
        private byte _tr;

        private BrkFlags _brkFlags;

        private SplitUInt16 _ad;

        private readonly bool _bcdEnabled;

        public Mos6502()
            : this(new Mos6502Options { BcdEnabled = true })
        {
        }

        public Mos6502(Mos6502Options options)
        {
            _bcdEnabled = options.BcdEnabled;

            _rdy = true;
            _sync = true;
            _res = true;
            _rw = true;
        }

        private void FetchNextInstruction()
        {
            Address = PC;
            _sync = true;
        }

        [Flags]
        private enum BrkFlags
        {
            None  = 0,
            Irq   = 1,
            Nmi   = 2,
            Reset = 4,
        }

        /// <summary>
        /// Increments the timing register (which has the effect of skipping the next instruction cycle)
        /// if no page boundary is crossed when <paramref name="addend"/> is added to <see cref="_ad"/>.
        /// 
        /// This implementation goes to some effort to avoid branching.
        ///
        /// Thanks to Andre Weissflog's code at
        /// https://github.com/floooh/chips/blob/fdef73e0e65ebb5aa2bf33e226ba5b108a7a43fb/codegen/m6502_gen.py#L208
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncrementTimingRegisterIfNoPageCrossing(byte addend)
        {
            // Original high-byte
            var original = _ad.Hi;

            // High-byte after adding value
            var modified = (_ad + addend).Hi;

            // Delta will be either 0 (if it didn't cross page) or -1 (if it did).
            var delta = original - modified;

            // Increment will be either 1 (if it didn't cross page) or 0 (if it did).
            var timingRegisterIncrement = ~delta & 1;

            _tr += (byte)timingRegisterIncrement;
        }

        public IEnumerable<DebuggerWindow> CreateDebuggerWindows()
        {
            yield return new CpuStateWindow(this);
            //yield return new DisassemblyWindow(this);
        }
    }

    public readonly struct DecodedInstruction
    {
        public readonly ushort Address;
        public readonly string Disassembly;
        public readonly ushort InstructionSizeInBytes;

        internal DecodedInstruction(ushort address, string disassembly, ushort instructionSizeInBytes)
        {
            Address = address;
            Disassembly = disassembly;
            InstructionSizeInBytes = instructionSizeInBytes;
        }
    }
}