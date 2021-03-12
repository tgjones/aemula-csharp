using System;
using System.Collections.Generic;
using Aemula.Chips.Mos6502.UI;
using Aemula.UI;

namespace Aemula.Chips.Mos6502
{
    public partial class Mos6502
    {
        public Mos6502Pins Pins;

        // Registers
        public byte A;
        public byte X;
        public byte Y;

        // Program counter
        public ushort PC;

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

        private ushort _ad;

        private readonly bool _bcdEnabled;

        public Mos6502(Mos6502Options options)
        {
            _bcdEnabled = options.BcdEnabled;

            Pins = new Mos6502Pins
            {
                Sync = true,
                Res = true,
                RW = true
            };
        }

        public void Tick()
        {
            ref var pins = ref Pins;

            if (pins.Sync | pins.Irq | pins.Nmi | pins.Rdy | pins.Res)
            {
                // TODO: Interrupt stuff.

                // Check RDY pin, but only during a read cycle.
                if (pins.Rdy & pins.RW)
                {
                    return;
                }

                if (pins.Sync)
                {
                    _ir = pins.Data;
                    _tr = 0;
                    pins.Sync = false;

                    if (pins.Res)
                    {
                        _brkFlags = BrkFlags.Reset;
                    }

                    if (_brkFlags != BrkFlags.None)
                    {
                        _ir = 0;
                        pins.Res = false;
                    }
                    else
                    {
                        PC++;
                    }
                }
            }

            // Assume we're going to read.
            pins.RW = true;

            ExecuteInstruction(ref pins);

            // Increment timing register.
            _tr += 1;
        }

        [Flags]
        private enum BrkFlags
        {
            None  = 0,
            Irq   = 1,
            Nmi   = 2,
            Reset = 4,
        }

        public IEnumerable<DebuggerWindow> CreateDebuggerWindows()
        {
            yield return new CpuStateWindow(this);
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