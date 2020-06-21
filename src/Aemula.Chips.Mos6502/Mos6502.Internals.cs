//using System;

//namespace Aemula.Chips.Mos6502
//{
//    partial class Mos6502
//    {
//        private const ushort NmiVector = 0xFFFA;
//        private const ushort ResetVector = 0xFFFC;
//        private const ushort IrqVector = 0xFFFE;

//        private abstract class InstructionImplementationBase
//        {
//            public abstract InstructionCycleResult Cycle(Mos6502 cpu);

//            public abstract void Reset();
//        }

//        private abstract class InstructionImplementation : InstructionImplementationBase
//        {
//            private readonly string _mnemonic;

//            public InstructionImplementation(string mnemonic)
//            {
//                _mnemonic = mnemonic;
//            }

//            public string GetDisassembly(Mos6502 cpu, ushort address)
//            {
//                var result = _mnemonic;

//                var operands = GetDisassembledOperands(cpu, address);
//                if (operands.Length > 0)
//                {
//                    result += " " + operands;
//                }

//                return result;
//            }

//            protected virtual string GetDisassembledOperands(Mos6502 cpu, ushort address) => string.Empty;

//            public virtual byte InstructionSizeInBytes => 1;
//        }

//        private enum InstructionCycleResult
//        {
//            Finished,
//            NotFinished
//        }

//        private static bool AreSamePage(ushort v1, ushort v2)
//        {
//            return (v1 & 0xFF00) == (v2 & 0xFF00);
//        }

//        // Almost the same as BRK, but clears the B flag in the pushed status byte.
//        private sealed class IrqImplementation : InstructionImplementationBase
//        {
//            private int _state;
//            private ushort _address;

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0:
//                        cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1:
//                        cpu.Push((byte)(cpu.PC >> 8));
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2:
//                        cpu.Push((byte)(cpu.PC & 0xFF));
//                        _state = 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3:
//                        cpu.Push(cpu.P.AsByte(false));
//                        cpu.P.I = true; // Disable interrupts
//                        _state = 4;
//                        return InstructionCycleResult.NotFinished;

//                    case 4:
//                        _address = cpu.ReadMemory(IrqVector);
//                        _state = 5;
//                        return InstructionCycleResult.NotFinished;

//                    case 5:
//                        _address |= (ushort)(cpu.ReadMemory(IrqVector + 1) << 8);
//                        cpu.PC = _address;
//                        return InstructionCycleResult.Finished;

//                    default:
//                        throw new InvalidOperationException();
//                }
//            }

//            public override void Reset()
//            {
//                _state = 0;
//            }
//        }

//        // Almost the same as IRQ, but gets the address from $FFFA/$FFFB.
//        private sealed class NmiImplementation : InstructionImplementationBase
//        {
//            private int _state;
//            private ushort _address;

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0:
//                        cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1:
//                        cpu.Push((byte)(cpu.PC >> 8));
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2:
//                        cpu.Push((byte)(cpu.PC & 0xFF));
//                        _state = 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3:
//                        cpu.Push(cpu.P.AsByte(false));
//                        cpu.P.I = true; // Disable interrupts
//                        _state = 4;
//                        return InstructionCycleResult.NotFinished;

//                    case 4:
//                        _address = cpu.ReadMemory(NmiVector);
//                        _state = 5;
//                        return InstructionCycleResult.NotFinished;

//                    case 5:
//                        _address |= (ushort)(cpu.ReadMemory(NmiVector + 1) << 8);
//                        cpu.PC = _address;
//                        return InstructionCycleResult.Finished;

//                    default:
//                        throw new InvalidOperationException();
//                }
//            }

//            public override void Reset()
//            {
//                _state = 0;
//            }
//        }

//        private sealed class ResetImplementation : InstructionImplementationBase
//        {
//            private int _state;
//            private ushort _address;

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0:
//                        cpu.A = 0;
//                        cpu.X = 0;
//                        cpu.Y = 0;
//                        cpu.SP = 0;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1:
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2:
//                        cpu.Push((byte)(cpu.PC >> 8), false);
//                        _state = 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3:
//                        cpu.Push((byte)(cpu.PC & 0xFF), false);
//                        _state = 4;
//                        return InstructionCycleResult.NotFinished;

//                    case 4:
//                        cpu.Push(cpu.P.AsByte(false), false);
//                        cpu.P.SetFromByte(0);
//                        cpu.P.I = true; // Disable interrupts
//                        _state = 5;
//                        return InstructionCycleResult.NotFinished;

//                    case 5:
//                        _address = cpu.ReadMemory(ResetVector);
//                        _state = 6;
//                        return InstructionCycleResult.NotFinished;

//                    case 6:
//                        _address |= (ushort)(cpu.ReadMemory(ResetVector + 1) << 8);
//                        cpu.PC = _address;
//                        cpu.Resetting = false;
//                        return InstructionCycleResult.Finished;

//                    default:
//                        throw new InvalidOperationException();
//                }
//            }

//            public override void Reset()
//            {
//                _state = 0;
//            }
//        }
//    }
//}