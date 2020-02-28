using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Aemula.Chips.Intelx86
{
    public sealed partial class Intelx86
    {
        private readonly Bus.Bus<uint, byte> _bus;
        private readonly Dictionary<byte, Action> _instructions;

        private ModRM _modRM;

        public readonly GeneralRegister[] Registers;

        public ushort SP
        {
            get => Registers[RegSP].Word0;
            set => Registers[RegSP].Word0 = value;
        }

        public ushort IP;

        public ushort CS;
        public ushort DS;
        public ushort ES;
        public ushort SS;

        public ProcessorFlags Flags;

        public Intelx86(Bus.Bus<uint, byte> bus)
        {
            _bus = bus;

            _instructions = CreateInstructions();

            Registers = new GeneralRegister[8];
        }

        public void Reset()
        {
            IP = 0xFFF0;
        }

        public void Run()
        {
            while (true) // TODO
            {
                var opcode = Fetch8();

                if (!_instructions.TryGetValue(opcode, out var implementation))
                {
                    throw new NotSupportedException($"Opcode {opcode} has not yet been implemented.");
                }

                implementation();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte Fetch8()
        {
            var result = _bus.Read(IP);
            IP++;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private sbyte FetchI8()
        {
            return (sbyte)Fetch8();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort Fetch16()
        {
            var result = (ushort)(_bus.Read(IP) | (_bus.Read((uint)(IP + 1)) << 8));
            IP += 2;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private short FetchI16()
        {
            return (short)Fetch16();
        }

        private void FetchModRM()
        {
            var value = Fetch8();

            _modRM = new ModRM
            {
                Mod = (OperandMode)((value >> 6) & 0b11),
                RegOrOpcodeExtension = (byte)((value >> 3) & 0b111),
                RM = (byte)(value & 0b111),
                // TODO
            };

            switch (_modRM.Mod)
            {
                case OperandMode.MemoryMode8BitDisplacement:
                    _modRM.Disp = Fetch8();
                    break;

                case OperandMode.MemoryMode16BitDisplacement:
                    _modRM.Disp = Fetch16();
                    break;

                case OperandMode.MemoryModeNoDisplacement when _modRM.RM == 6:
                    _modRM.Disp = Fetch16();
                    break;
            }
        }

        private ushort ReadEffective16()
        {
            switch (_modRM.Mod)
            {
                case OperandMode.MemoryModeNoDisplacement:
                    switch (_modRM.RM)
                    {
                        case 6: return _bus.Read(_modRM.Disp);
                        default: throw new NotImplementedException();
                    }

                case OperandMode.RegisterMode:
                    return Registers[_modRM.RM].Word0;

                default:
                    throw new NotImplementedException();
            }
        }

        private void WriteEffective8(byte value)
        {
            switch (_modRM.Mod)
            {
                case OperandMode.MemoryModeNoDisplacement:
                    switch (_modRM.RM)
                    {
                        case 1:
                            _bus.Write((uint)(Registers[RegBX].Word0 + Registers[RegDI].Word0), value);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void WriteEffective16(ushort value)
        {
            switch (_modRM.Mod)
            {
                case OperandMode.MemoryModeNoDisplacement:
                    switch (_modRM.RM)
                    {
                        case 6:
                            throw new NotImplementedException();
                            //_bus.Write16(_modRM.Disp, value);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                    break;

                case OperandMode.RegisterMode:
                    Registers[_modRM.RM].Word0 = value;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private byte ReadRegister8()
        {
            return Registers[_modRM.RegOrOpcodeExtension].Byte0;
        }

        private ushort ReadRegister16()
        {
            return Registers[_modRM.RegOrOpcodeExtension].Word0;
        }

        private void WriteRegister16(ushort value)
        {
            Registers[_modRM.RegOrOpcodeExtension].Word0 = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Parity(uint value)
        {
            if (Popcnt.IsSupported)
            {
                return (Popcnt.PopCount(value & 0xFF) & 1) == 1;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort SignExtend8to16(byte value)
        {
            return unchecked((ushort)(value << 24 >> 24));
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct GeneralRegister
    {
        [FieldOffset(0)]
        public byte Byte0;

        [FieldOffset(1)]
        public byte Byte1;

        [FieldOffset(0)]
        public ushort Word0;
    }

    public struct ProcessorFlags
    {
        /// <summary>
        /// Carry flag.
        /// </summary>
        public bool CF;

        /// <summary>
        /// Parity flag.
        /// </summary>
        public bool PF;

        /// <summary>
        /// Auxiliary carry flag.
        /// </summary>
        public bool AF;

        /// <summary>
        /// Zero flag.
        /// </summary>
        public bool ZF;

        /// <summary>
        /// Sign flag.
        /// </summary>
        public bool SF;

        /// <summary>
        /// Trap flag.
        /// </summary>
        public bool TF;

        /// <summary>
        /// Interrupt flag.
        /// </summary>
        public bool IF;

        /// <summary>
        /// Direction flag.
        /// </summary>
        public bool DF;

        /// <summary>
        /// Overflow flag.
        /// </summary>
        public bool OF;
    }
}
