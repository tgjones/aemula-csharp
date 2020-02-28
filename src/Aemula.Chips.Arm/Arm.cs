using System;
using System.Diagnostics;

namespace Aemula.Chips.Arm
{
    public sealed partial class Arm
    {
        private readonly Bus.Bus<uint, byte> _bus;

        public Registers R;

        // From http://www.keil.com/support/man/docs/armasm/armasm_dom1359731138020.htm:
        // During execution, PC does not contain the address of the currently executing 
        // instruction. The address of the currently executing instruction is typically 
        // PC–8 for ARM, or PC–4 for Thumb.
        public uint PC
        {
            get => R[Registers.PC];
        }

        public uint SP
        {
            get => R[Registers.SP];
            set => R[Registers.SP] = value;
        }

        public uint LR
        {
            get => R[Registers.LR];
            set => R[Registers.LR] = value;
        }

        /// <summary>
        /// Application Program Status Register.
        /// </summary>
        public Apsr Apsr;

        public Arm(Bus.Bus<uint, byte> bus)
        {
            _bus = bus;

            R = new Registers();

            SP = 0xD0000000; // TODO
        }

        public void Step()
        {
            var rawInstruction = ReadMemory(R.CurrentInstructionAddress);

            var instructionAddress = R.CurrentInstructionAddress;
            var instruction = InstructionDecoder.Decode(rawInstruction);

            var result = instruction.Execute(this);

            if (result != Instructions.ExecutionResult.Branched)
            {
                R.CurrentInstructionAddress += 4;
            }

            Debug.WriteLine($"{instructionAddress:x} {instruction}");
        }

        // A8.3.1
        internal bool ConditionPassed(Condition condition)
        {
            var cond = (uint)condition;

            bool result;

            switch (cond >> 1)
            {
                case 0b000: // EQ or NE
                    result = Apsr.Z;
                    break;

                case 0b001: // CS or CC
                    result = Apsr.C;
                    break;

                case 0b010: // MI or PL
                    result = Apsr.N;
                    break;

                case 0b011: // VS or VC
                    result = Apsr.V;
                    break;

                case 0b100: // HI or LS
                    result = Apsr.C && !Apsr.Z;
                    break;

                case 0b101: // GE or LT
                    result = Apsr.N == Apsr.V;
                    break;

                case 0b110: // GT or LE
                    result = (Apsr.N == Apsr.V) && !Apsr.Z;
                    break;

                case 0b111: // AL
                    result = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (cond.GetBit(0) == 1 && cond != 0b1111)
            {
                result = !result;
            }

            return result;
        }

        // A2.3.2
        internal void BXWritePC(uint address)
        {
            // TODO: Full implementation including switching instruction sets.
            BranchTo(address);
        }

        // A2.3.2
        internal void ALUWritePC(uint address)
        {
            // TODO: Full implementation
            BXWritePC(address);
        }

        // A2.3.2
        internal void BranchWritePC(uint address)
        {
            // TODO: Full implementation including switching instruction sets.
            BranchTo(address & 0xFFFFFFFC);
        }

        // A2.3.2
        internal uint PCStoreValue()
        {
            return PC;
        }

        // B1.3.2
        internal void BranchTo(uint address)
        {
            R.CurrentInstructionAddress = address;
        }

        internal uint ReadMemory(uint address)
        {
            return (uint)(_bus.Read(address)
                | (_bus.Read(address + 1) << 8)
                | (_bus.Read(address + 2) << 16)
                | (_bus.Read(address + 3) << 24));
        }

        internal void WriteMemory(uint address, uint value)
        {
            _bus.Write(address, (byte)(value & 0xFF));
            _bus.Write(address + 1, (byte)((value >> 8) & 0xFF));
            _bus.Write(address + 2, (byte)((value >> 16) & 0xFF));
            _bus.Write(address + 3, (byte)((value >> 24) & 0xFF));
        }
    }

    public struct Apsr
    {
        public bool N;
        public bool Z;
        public bool C;
        public bool V;
        public bool Q;
        public bool GE;
    }
}
