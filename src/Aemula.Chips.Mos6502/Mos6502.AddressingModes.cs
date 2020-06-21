//using System;

//namespace Aemula.Chips.Mos6502
//{
//    partial class Mos6502
//    {
//        private sealed class Immediate : InstructionImplementation
//        {
//            private readonly Action<byte> _func;

//            public override byte InstructionSizeInBytes => 2;

//            public Immediate(string mnemonic, Action<byte> func)
//                : base(mnemonic)
//            {
//                _func = func;
//            }

//            protected override string GetDisassembledOperands(Mos6502 cpu, ushort address)
//            {
//                return $"#${cpu.ReadMemory((ushort)(address + 1)):X2}";
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                // T1
//                var value = cpu.ReadMemory(cpu.PC);
//                cpu.PC++;
//                _func(value);
//                return InstructionCycleResult.Finished;
//            }

//            public override void Reset() { }
//        }

//        private abstract class ZeroPage : InstructionImplementation
//        {
//            public override byte InstructionSizeInBytes => 2;

//            public ZeroPage(string mnemonic)
//                : base(mnemonic)
//            {
//            }

//            protected override string GetDisassembledOperands(Mos6502 cpu, ushort address)
//            {
//                return $"${cpu.ReadMemory((ushort)(address + 1)):X2}";
//            }
//        }

//        private sealed class ZeroPageRead : ZeroPage
//        {
//            private readonly Action<byte> _func;
//            private int _state;
//            private byte _address;

//            public ZeroPageRead(string mnemonic, Action<byte> func)
//                : base(mnemonic)
//            {
//                _func = func;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0: // T1
//                        _address = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1: // T2
//                        var value = cpu.ReadMemory(_address);
//                        _func(value);
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

//        private sealed class ZeroPageWrite : ZeroPage
//        {
//            private readonly Func<byte> _func;
//            private int _state;
//            private byte _address;

//            public ZeroPageWrite(string mnemonic, Func<byte> func)
//                : base(mnemonic)
//            {
//                _func = func;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0:
//                        _address = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1:
//                        cpu.WriteMemory(_address, _func());
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

//        private sealed class ZeroPageReadModifyWrite : ZeroPage
//        {
//            private readonly Func<byte, byte> _func;
//            private int _state;
//            private byte _address;
//            private byte _value;

//            public ZeroPageReadModifyWrite(string mnemonic, Func<byte, byte> func)
//                : base(mnemonic)
//            {
//                _func = func;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0:
//                        _address = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1:
//                        _value = cpu.ReadMemory(_address);
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2:
//                        cpu.WriteMemory(_address, _value); // Write original value.
//                        _state = 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3:
//                        cpu.WriteMemory(_address, _func(_value));
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

//        private abstract class ZeroPageIndexed : InstructionImplementation
//        {
//            private readonly string _indexRegisterName;

//            public override byte InstructionSizeInBytes => 2;

//            public ZeroPageIndexed(string mnemonic, string indexRegisterName)
//                : base(mnemonic)
//            {
//                _indexRegisterName = indexRegisterName;
//            }

//            protected override string GetDisassembledOperands(Mos6502 cpu, ushort address)
//            {
//                return $"${cpu.ReadMemory((ushort)(address + 1)):X2},{_indexRegisterName}";
//            }
//        }

//        private abstract class ZeroPageIndexedRead : ZeroPageIndexed
//        {
//            private readonly Action<byte> _func;
//            private readonly Func<Mos6502, byte> _getIndexValue;
//            private int _state;
//            private byte _address;

//            protected ZeroPageIndexedRead(string mnemonic, Action<byte> func, Func<Mos6502, byte> getIndexValue, string indexRegisterName)
//                : base(mnemonic, indexRegisterName)
//            {
//                _func = func;
//                _getIndexValue = getIndexValue;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0: // T1
//                        _address = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1: // T2
//                        cpu.ReadMemory(_address); // Read at wrong address
//                        _address += _getIndexValue(cpu);
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2: // T3
//                        var value = cpu.ReadMemory(_address);
//                        _func(value);
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

//        private sealed class ZeroPageXRead : ZeroPageIndexedRead
//        {
//            public ZeroPageXRead(string mnemonic, Action<byte> func)
//                : base(mnemonic, func, cpu => cpu.X, "X")
//            {

//            }
//        }

//        private sealed class ZeroPageYRead : ZeroPageIndexedRead
//        {
//            public ZeroPageYRead(string mnemonic, Action<byte> func)
//                : base(mnemonic, func, cpu => cpu.Y, "Y")
//            {

//            }
//        }

//        private abstract class ZeroPageIndexedWrite : ZeroPageIndexed
//        {
//            private readonly Func<byte> _func;
//            private readonly Func<Mos6502, byte> _getIndexValue;
//            private int _state;
//            private byte _address;
//            private byte _addressWithIndex;

//            protected ZeroPageIndexedWrite(string mnemonic, Func<byte> func, Func<Mos6502, byte> getIndexValue, string indexRegisterName)
//                : base(mnemonic, indexRegisterName)
//            {
//                _func = func;
//                _getIndexValue = getIndexValue;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0:
//                        _address = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1:
//                        cpu.ReadMemory(_address);
//                        _addressWithIndex = (byte)(_address + _getIndexValue(cpu));
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2:
//                        cpu.WriteMemory(_addressWithIndex, _func());
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

//        private sealed class ZeroPageXWrite : ZeroPageIndexedWrite
//        {
//            public ZeroPageXWrite(string mnemonic, Func<byte> func)
//                : base(mnemonic, func, cpu => cpu.X, "X")
//            {

//            }
//        }

//        private sealed class ZeroPageYWrite : ZeroPageIndexedWrite
//        {
//            public ZeroPageYWrite(string mnemonic, Func<byte> func)
//                : base(mnemonic, func, cpu => cpu.Y, "Y")
//            {

//            }
//        }

//        private sealed class ZeroPageXReadModifyWrite : ZeroPageIndexed
//        {
//            private readonly Func<byte, byte> _func;
//            private int _state;
//            private byte _address;
//            private byte _value;

//            public ZeroPageXReadModifyWrite(string mnemonic, Func<byte, byte> func)
//                : base(mnemonic, "X")
//            {
//                _func = func;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0:
//                        _address = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1:
//                        cpu.ReadMemory(_address); // Read from wrong address.
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2:
//                        _address += cpu.X;
//                        _value = cpu.ReadMemory(_address);
//                        _state = 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3:
//                        cpu.WriteMemory(_address, _value); // Write original value.
//                        _state = 4;
//                        return InstructionCycleResult.NotFinished;

//                    case 4:
//                        cpu.WriteMemory(_address, _func(_value));
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

//        private abstract class Absolute : InstructionImplementation
//        {
//            public override byte InstructionSizeInBytes => 3;

//            public Absolute(string mnemonic)
//                : base(mnemonic)
//            {
//            }

//            protected override string GetDisassembledOperands(Mos6502 cpu, ushort address)
//            {
//                var actualAddress = cpu.ReadMemory((ushort)(address + 1)) | (cpu.ReadMemory((ushort)(address + 2)) << 8);
//                return $"${actualAddress:X4}";
//            }
//        }

//        private sealed class AbsoluteRead : Absolute
//        {
//            private readonly Action<byte> _func;
//            private int _state;
//            private ushort _address;

//            public AbsoluteRead(string mnemonic, Action<byte> func)
//                : base(mnemonic)
//            {
//                _func = func;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0: // T1
//                        _address = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1: // T2
//                        _address |= (ushort)(cpu.ReadMemory(cpu.PC) << 8);
//                        cpu.PC++;
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2: // T3
//                        var value = cpu.ReadMemory(_address);
//                        _func(value);
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

//        private sealed class AbsoluteWrite : Absolute
//        {
//            private readonly Func<byte> _func;
//            private int _state;
//            private ushort _address;

//            public AbsoluteWrite(string mnemonic, Func<byte> func)
//                : base(mnemonic)
//            {
//                _func = func;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0: // T1
//                        _address = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1: // T2
//                        _address |= (ushort)(cpu.ReadMemory(cpu.PC) << 8);
//                        cpu.PC++;
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2: // T3
//                        cpu.WriteMemory(_address, _func());
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

//        private sealed class AbsoluteReadModifyWrite : Absolute
//        {
//            private readonly Func<byte, byte> _func;
//            private int _state;
//            private ushort _address;
//            private byte _value;

//            public AbsoluteReadModifyWrite(string mnemonic, Func<byte, byte> func)
//                : base(mnemonic)
//            {
//                _func = func;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0: // Read low byte of address.
//                        _address = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1: // Read high byte of address.
//                        _address |= (ushort)(cpu.ReadMemory(cpu.PC) << 8);
//                        cpu.PC++;
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2: // Read value at effective address.
//                        _value = cpu.ReadMemory(_address);
//                        _state = 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3: // Spurious write.
//                        cpu.WriteMemory(_address, _value);
//                        _state = 4;
//                        return InstructionCycleResult.NotFinished;

//                    case 4: // Perform operation and write result.
//                        cpu.WriteMemory(_address, _func(_value));
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

//        private abstract class AbsoluteIndexed : InstructionImplementation
//        {
//            private readonly string _indexRegisterName;

//            public override byte InstructionSizeInBytes => 3;

//            public AbsoluteIndexed(string mnemonic, string indexRegisterName)
//                : base(mnemonic)
//            {
//                _indexRegisterName = indexRegisterName;
//            }

//            protected override string GetDisassembledOperands(Mos6502 cpu, ushort address)
//            {
//                var actualAddress = cpu.ReadMemory((ushort)(address + 1)) | (cpu.ReadMemory((ushort)(address + 2)) << 8);
//                return $"${actualAddress:X4},{_indexRegisterName}";
//            }
//        }

//        private abstract class AbsoluteIndexedRead : AbsoluteIndexed
//        {
//            private readonly Action<byte> _func;
//            private readonly Func<Mos6502, byte> _getIndexValue;
//            private int _state;
//            private ushort _address;
//            private ushort _addressWithIndex;

//            protected AbsoluteIndexedRead(string mnemonic, Action<byte> func, Func<Mos6502, byte> getIndexValue, string indexRegisterName)
//                : base(mnemonic, indexRegisterName)
//            {
//                _func = func;
//                _getIndexValue = getIndexValue;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0: // T1
//                        _address = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1: // T2
//                        _address |= (ushort)(cpu.ReadMemory(cpu.PC) << 8);
//                        _addressWithIndex = (ushort)(_address + _getIndexValue(cpu));
//                        cpu.PC++;
//                        _state = AreSamePage(_address, _addressWithIndex) ? 3 : 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2: // T3
//                        cpu.ReadMemory((ushort)(_addressWithIndex - 0x100)); // Read at wrong address.
//                        _state = 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3: // T4
//                        var value = cpu.ReadMemory(_addressWithIndex);
//                        _func(value);
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

//        private sealed class AbsoluteXRead : AbsoluteIndexedRead
//        {
//            public AbsoluteXRead(string mnemonic, Action<byte> func)
//                : base(mnemonic, func, cpu => cpu.X, "X")
//            {
//            }
//        }

//        private sealed class AbsoluteYRead : AbsoluteIndexedRead
//        {
//            public AbsoluteYRead(string mnemonic, Action<byte> func)
//                : base(mnemonic, func, cpu => cpu.Y, "Y")
//            {
//            }
//        }

//        private abstract class AbsoluteIndexedWrite : AbsoluteIndexed
//        {
//            private readonly Func<byte> _func;
//            private readonly Func<Mos6502, byte> _getIndexValue;
//            private int _state;
//            private ushort _address;
//            private ushort _addressWithIndex;

//            protected AbsoluteIndexedWrite(string mnemonic, Func<byte> func, Func<Mos6502, byte> getIndexValue, string indexRegisterName)
//                : base(mnemonic, indexRegisterName)
//            {
//                _func = func;
//                _getIndexValue = getIndexValue;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0: // T1
//                        _address = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1: // T2
//                        _address |= (ushort)(cpu.ReadMemory(cpu.PC) << 8);
//                        _addressWithIndex = (ushort)(_address + _getIndexValue(cpu));
//                        cpu.PC++;
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2: // T3
//                        var wrongAddress = !AreSamePage(_address, _addressWithIndex)
//                            ? _addressWithIndex - 0x100
//                            : _addressWithIndex;
//                        cpu.ReadMemory((ushort)wrongAddress); // Read at wrong address.
//                        _state = 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3: // T4
//                        cpu.WriteMemory(_addressWithIndex, _func());
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

//        private sealed class AbsoluteXWrite : AbsoluteIndexedWrite
//        {
//            public AbsoluteXWrite(string mnemonic, Func<byte> func)
//                : base(mnemonic, func, cpu => cpu.X, "X")
//            {
//            }
//        }

//        private sealed class AbsoluteYWrite : AbsoluteIndexedWrite
//        {
//            public AbsoluteYWrite(string mnemonic, Func<byte> func)
//                : base(mnemonic, func, cpu => cpu.Y, "Y")
//            {
//            }
//        }

//        private abstract class AbsoluteIndexedReadModifyWrite : AbsoluteIndexed
//        {
//            private readonly Func<byte, byte> _func;
//            private readonly Func<Mos6502, byte> _getIndexValue;
//            private int _state;
//            private ushort _address;
//            private ushort _addressWithIndexNoCarry;
//            private ushort _addressWithIndex;
//            private byte _value;

//            public AbsoluteIndexedReadModifyWrite(string mnemonic, Func<byte, byte> func, Func<Mos6502, byte> getIndexValue, string indexRegisterName)
//                : base(mnemonic, indexRegisterName)
//            {
//                _func = func;
//                _getIndexValue = getIndexValue;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0: // Read low byte of address.
//                        _address = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1: // Read high byte of address.
//                        _address |= (ushort)(cpu.ReadMemory(cpu.PC) << 8);
//                        _addressWithIndexNoCarry = (ushort)((_address & 0xFF00) | (((_address & 0xFF) + _getIndexValue(cpu)) % 256));
//                        cpu.PC++;
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2: // Read value at effective address. Fix high byte of effective address.
//                        _value = cpu.ReadMemory(_addressWithIndexNoCarry);
//                        _addressWithIndex = (ushort)(_address + _getIndexValue(cpu));
//                        _state = 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3: // Spurious read.
//                        _value = cpu.ReadMemory(_addressWithIndex);
//                        _state = 4;
//                        return InstructionCycleResult.NotFinished;

//                    case 4: // Spurious write.
//                        cpu.WriteMemory(_addressWithIndex, _value);
//                        _state = 5;
//                        return InstructionCycleResult.NotFinished;

//                    case 5: // Perform operation and write result.
//                        cpu.WriteMemory(_addressWithIndex, _func(_value));
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

//        private sealed class AbsoluteXReadModifyWrite : AbsoluteIndexedReadModifyWrite
//        {
//            public AbsoluteXReadModifyWrite(string mnemonic, Func<byte, byte> func)
//                : base(mnemonic, func, cpu => cpu.X, "X")
//            {
//            }
//        }

//        private sealed class AbsoluteYReadModifyWrite : AbsoluteIndexedReadModifyWrite
//        {
//            public AbsoluteYReadModifyWrite(string mnemonic, Func<byte, byte> func)
//                : base(mnemonic, func, cpu => cpu.Y, "Y")
//            {
//            }
//        }

//        private abstract class IndexedIndirectX : InstructionImplementation
//        {
//            public override byte InstructionSizeInBytes => 2;

//            protected IndexedIndirectX(string mnemonic)
//                : base(mnemonic)
//            {
//            }

//            protected override string GetDisassembledOperands(Mos6502 cpu, ushort address)
//            {
//                return $"(${cpu.ReadMemory((ushort)(address + 1)):X2},X)";
//            }
//        }

//        private sealed class IndexedIndirectXRead : IndexedIndirectX
//        {
//            private readonly Action<byte> _func;
//            private int _state;
//            private ushort _indirectAddress;
//            private ushort _address;

//            public IndexedIndirectXRead(string mnemonic, Action<byte> func)
//                : base(mnemonic)
//            {
//                _func = func;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0:
//                        _indirectAddress = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1:
//                        cpu.ReadMemory(_indirectAddress); // Read from wrong address.
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2:
//                        _address = cpu.ReadMemory((ushort)((_indirectAddress + cpu.X) % 256));
//                        _state = 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3:
//                        _address |= (ushort)(cpu.ReadMemory((ushort)((_indirectAddress + cpu.X + 1) % 256)) << 8);
//                        _state = 4;
//                        return InstructionCycleResult.NotFinished;

//                    case 4:
//                        var value = cpu.ReadMemory(_address);
//                        _func(value);
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

//        private sealed class IndexedIndirectXWrite : IndexedIndirectX
//        {
//            private readonly Func<byte> _func;
//            private int _state;
//            private ushort _indirectAddress;
//            private ushort _address;

//            public IndexedIndirectXWrite(string mnemonic, Func<byte> func)
//                : base(mnemonic)
//            {
//                _func = func;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0:
//                        _indirectAddress = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1:
//                        cpu.ReadMemory(_indirectAddress); // Read from wrong address.
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2:
//                        _address = cpu.ReadMemory((ushort)((_indirectAddress + cpu.X) % 256));
//                        _state = 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3:
//                        _address |= (ushort)(cpu.ReadMemory((ushort)((_indirectAddress + cpu.X + 1) % 256)) << 8);
//                        _state = 4;
//                        return InstructionCycleResult.NotFinished;

//                    case 4:
//                        cpu.WriteMemory(_address, _func());
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

//        private sealed class IndexedIndirectXReadModifyWrite : IndexedIndirectX
//        {
//            private readonly Func<byte, byte> _func;
//            private int _state;
//            private ushort _indirectAddress;
//            private ushort _address;
//            private byte _value;

//            public IndexedIndirectXReadModifyWrite(string mnemonic, Func<byte, byte> func)
//                : base(mnemonic)
//            {
//                _func = func;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0:
//                        _indirectAddress = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1:
//                        cpu.ReadMemory(_indirectAddress); // Read from wrong address.
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2:
//                        _address = cpu.ReadMemory((ushort)((_indirectAddress + cpu.X) % 256));
//                        _state = 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3:
//                        _address |= (ushort)(cpu.ReadMemory((ushort)((_indirectAddress + cpu.X + 1) % 256)) << 8);
//                        _state = 4;
//                        return InstructionCycleResult.NotFinished;

//                    case 4:
//                        _value = cpu.ReadMemory(_address);
//                        _state = 5;
//                        return InstructionCycleResult.NotFinished;

//                    case 5:
//                        cpu.WriteMemory(_address, _value);
//                        _value = _func(_value);
//                        _state = 6;
//                        return InstructionCycleResult.NotFinished;

//                    case 6:
//                        cpu.WriteMemory(_address, _value);
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

//        private abstract class IndirectIndexedY : InstructionImplementation
//        {
//            public override byte InstructionSizeInBytes => 2;

//            protected IndirectIndexedY(string mnemonic)
//                : base(mnemonic)
//            {
//            }

//            protected override string GetDisassembledOperands(Mos6502 cpu, ushort address)
//            {
//                return $"(${cpu.ReadMemory((ushort)(address + 1)):X2}),Y";
//            }
//        }

//        private sealed class IndirectIndexedYRead : IndirectIndexedY
//        {
//            private readonly Action<byte> _func;
//            private int _state;
//            private ushort _indirectAddress;
//            private ushort _address;
//            private ushort _addressWithY;

//            public IndirectIndexedYRead(string mnemonic, Action<byte> func)
//                : base(mnemonic)
//            {
//                _func = func;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0:
//                        _indirectAddress = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1:
//                        _address = cpu.ReadMemory(_indirectAddress);
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2:
//                        _address |= (ushort)(cpu.ReadMemory((ushort)((_indirectAddress + 1) % 256)) << 8);
//                        _addressWithY = (ushort)(_address + cpu.Y);
//                        _state = AreSamePage(_address, _addressWithY) ? 4 : 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3:
//                        cpu.ReadMemory((ushort)((_address & 0xFF00) | (_addressWithY & 0x00FF))); // Read at wrong address.
//                        _state = 4;
//                        return InstructionCycleResult.NotFinished;

//                    case 4:
//                        var value = cpu.ReadMemory((ushort)(_address + cpu.Y));
//                        _func(value);
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

//        private sealed class IndirectIndexedYWrite : IndirectIndexedY
//        {
//            private readonly Func<byte> _func;
//            private int _state;
//            private ushort _indirectAddress;
//            private ushort _address;
//            private ushort _addressWithY;

//            public IndirectIndexedYWrite(string mnemonic, Func<byte> func)
//                : base(mnemonic)
//            {
//                _func = func;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0:
//                        _indirectAddress = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1:
//                        _address = cpu.ReadMemory(_indirectAddress);
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2:
//                        _address |= (ushort)(cpu.ReadMemory((ushort)((_indirectAddress + 1) % 256)) << 8);
//                        _addressWithY = (ushort)(_address + cpu.Y);
//                        _state = 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3:
//                        cpu.ReadMemory((ushort)((_address & 0xFF00) | (_addressWithY & 0x00FF))); // Read at wrong address.
//                        _state = 4;
//                        return InstructionCycleResult.NotFinished;

//                    case 4:
//                        cpu.WriteMemory((ushort)(_address + cpu.Y), _func());
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

//        private sealed class IndirectIndexedYReadModifyWrite : IndirectIndexedY
//        {
//            private readonly Func<byte, byte> _func;
//            private int _state;
//            private ushort _indirectAddress;
//            private ushort _address;
//            private ushort _addressWithY;
//            private byte _value;

//            public IndirectIndexedYReadModifyWrite(string mnemonic, Func<byte, byte> func)
//                : base(mnemonic)
//            {
//                _func = func;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                switch (_state)
//                {
//                    case 0:
//                        _indirectAddress = cpu.ReadMemory(cpu.PC);
//                        cpu.PC++;
//                        _state = 1;
//                        return InstructionCycleResult.NotFinished;

//                    case 1:
//                        _address = cpu.ReadMemory(_indirectAddress);
//                        _state = 2;
//                        return InstructionCycleResult.NotFinished;

//                    case 2:
//                        _address |= (ushort)(cpu.ReadMemory((ushort)((_indirectAddress + 1) % 256)) << 8);
//                        _addressWithY = (ushort)(_address + cpu.Y);
//                        _state = 3;
//                        return InstructionCycleResult.NotFinished;

//                    case 3:
//                        cpu.ReadMemory((ushort)((_address & 0xFF00) | (_addressWithY & 0x00FF))); // Read at wrong address.
//                        _state = 4;
//                        return InstructionCycleResult.NotFinished;

//                    case 4:
//                        _value = cpu.ReadMemory((ushort)(_address + cpu.Y));
//                        _state = 5;
//                        return InstructionCycleResult.NotFinished;

//                    case 5:
//                        cpu.WriteMemory((ushort)(_address + cpu.Y), _value);
//                        _value = _func(_value);
//                        _state = 6;
//                        return InstructionCycleResult.NotFinished;

//                    case 6:
//                        cpu.WriteMemory((ushort)(_address + cpu.Y), _value);
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

//        private sealed class AccumulatorReadModifyWrite : InstructionImplementation
//        {
//            private readonly Func<byte, byte> _func;

//            public AccumulatorReadModifyWrite(string mnemonic, Func<byte, byte> func)
//                : base(mnemonic)
//            {
//                _func = func;
//            }

//            public override InstructionCycleResult Cycle(Mos6502 cpu)
//            {
//                cpu.ReadMemory(cpu.PC); // Spurious read.
//                cpu.A = _func(cpu.A);
//                return InstructionCycleResult.Finished;
//            }

//            public override void Reset() { }
//        }
//    }
//}