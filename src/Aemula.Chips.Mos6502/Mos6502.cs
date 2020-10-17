using System;
using System.Collections.Generic;
using Aemula.Chips.Mos6502.UI;
using Aemula.UI;

namespace Aemula.Chips.Mos6502
{
    public sealed partial class Mos6502
    {
        public static (Mos6502, Mos6502Pins) Create(Mos6502Options options)
        {
            var mos6502 = new Mos6502(options);

            var pins = new Mos6502Pins
            {
                Sync = true,
                Res = true,
                RW = true
            };

            return (mos6502, pins);
        }

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

        private Mos6502(Mos6502Options options)
        {
            _bcdEnabled = options.BcdEnabled;
            _brkFlags = BrkFlags.Reset;
        }

        public void Tick(ref Mos6502Pins pins)
        {
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