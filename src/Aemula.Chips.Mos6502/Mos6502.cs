using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Aemula.Bus;
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

        public ulong Cycles;
        public Action OnFetchingInstruction { get; set; }
        public Action<ushort, byte> OnMemoryRead;
        public Action<ushort, byte> OnMemoryWrite;

        //public bool Rdy = false;

        public bool Resetting { get; internal set; }

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

        //public void Reset()
        //{
        //    _currentInstructionImplementation = _resetImplementation;
        //    _currentInstructionImplementation.Reset();

        //    _cycleState = CycleState.ExecuteInstruction;

        //    Resetting = true;
        //}

        //public void Irq()
        //{
        //    // Don't proceed if interrupts are disabled.
        //    if (P.I)
        //    {
        //        return;
        //    }

        //    _irqPending = true;
        //}

        //public void Nmi()
        //{
        //    _nmiPending = true;
        //}

        //public Mos6502CycleResult Cycle()
        //{
        //    Mos6502CycleResult cycleResult;

        //    switch (_cycleState)
        //    {
        //        case CycleState.FetchInstruction:
        //            if (_nmiPending)
        //            {
        //                _currentInstructionImplementation = _nmiImplementation;
        //                _nmiPending = false;
        //            }
        //            else if (_irqPending)
        //            {
        //                _currentInstructionImplementation = _irqImplementation;
        //                _irqPending = false;
        //            }
        //            else
        //            {
        //                OnFetchingInstruction?.Invoke();
        //                var opcode = ReadMemory(PC);
        //                if (Rdy)
        //                {
        //                    return Mos6502CycleResult.Paused;
        //                }
        //                if (!_instructionImplementations.TryGetValue(opcode, out var instructionImplementation))
        //                {
        //                    throw new NotImplementedException($"Opcode ${opcode:X2} is not implemented.");
        //                }
        //                PC++;
        //                _currentInstructionImplementation = instructionImplementation;
        //            }
        //            _currentInstructionImplementation.Reset();
        //            _cycleState = CycleState.ExecuteInstruction;
        //            cycleResult = Mos6502CycleResult.ExecutingInstruction;
        //            break;

        //        case CycleState.ExecuteInstruction:
        //            cycleResult = Mos6502CycleResult.ExecutingInstruction;
        //            if (_currentInstructionImplementation.Cycle(this) == InstructionCycleResult.Finished)
        //            {
        //                _currentInstructionImplementation = null;
        //                _cycleState = CycleState.FetchInstruction;
        //                cycleResult = Mos6502CycleResult.FinishedInstruction;
        //            }
        //            break;

        //        default:
        //            throw new InvalidOperationException();
        //    }

        //    Cycles++;

        //    //var formattedInstruction = instructionDefinition.Format(Address, Data);
        //    //_log.Enqueue($"{formattedInstruction,-15} (A = {A:X2}, X = {X:X2}, Y = {Y:X2}, PC = {PC:X4}) ${Address:X4} = {Memory[Address]:X2}");
        //    //while (_log.Count > 20)
        //    //{
        //    //    _log.Dequeue();
        //    //}

        //    return cycleResult;
        //}

        [Flags]
        private enum BrkFlags
        {
            None  = 0,
            Irq   = 1,
            Nmi   = 2,
            Reset = 4,
        }

        //private enum CycleState
        //{
        //    FetchInstruction,
        //    ExecuteInstruction
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private byte ReadMemory(ushort address)
        //{
        //    var result = _bus.Read(address);
        //    OnMemoryRead?.Invoke(address, result);
        //    return result;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private void WriteMemory(ushort address, byte data)
        //{
        //    _bus.Write(address, data);
        //    OnMemoryWrite?.Invoke(address, data);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private byte SetZeroNegativeFlags(int value)
        //{
        //    value &= 0xFF;

        //    P.Z = value == 0;
        //    P.N = (value & 0x80) == 0x80;

        //    return (byte)value;
        //}

        //private void Push(byte data, bool writeToMemory = true)
        //{
        //    if (writeToMemory)
        //    {
        //        WriteMemory((ushort)(0x0100 | SP), data);
        //    }
        //    SP--;
        //}


        //private sealed class Branch : InstructionImplementation
        //{
        //    private readonly Func<bool> _getFlag;
        //    private readonly bool _comparand;
        //    private int _state;
        //    private sbyte _offset;
        //    private bool _branchTaken;
        //    private ushort _oldPC;

        //    public override byte InstructionSizeInBytes => 2;

        //    public Branch(string mnemonic, Func<bool> getFlag, bool comparand)
        //        : base(mnemonic)
        //    {
        //        _getFlag = getFlag;
        //        _comparand = comparand;
        //    }

        //    protected override string GetDisassembledOperands(Mos6502 cpu, ushort address)
        //    {
        //        var offset = (sbyte)cpu.ReadMemory((ushort)(address + 1));
        //        var newAddress = address + 2 + offset;
        //        return $"${newAddress:X4}";
        //    }

        //    public override InstructionCycleResult Cycle(Mos6502 cpu)
        //    {
        //        switch (_state)
        //        {
        //            case 0:
        //                _offset = (sbyte)cpu.ReadMemory(cpu.PC);
        //                cpu.PC++;
        //                _branchTaken = _getFlag() == _comparand;
        //                _state = 1;
        //                return _branchTaken
        //                    ? InstructionCycleResult.NotFinished
        //                    : InstructionCycleResult.Finished;

        //            case 1:
        //                cpu.ReadMemory(cpu.PC); // Spurious read of next instruction.
        //                _oldPC = cpu.PC;
        //                cpu.PC = (ushort)(cpu.PC + _offset);
        //                var samePage = AreSamePage(_oldPC, cpu.PC);
        //                _state = 2;
        //                return samePage
        //                    ? InstructionCycleResult.Finished
        //                    : InstructionCycleResult.NotFinished;

        //            case 2:
        //                cpu.ReadMemory((ushort)((_oldPC & 0xFF00) | (cpu.PC & 0x00FF))); // Spurious read of invalid address.
        //                return InstructionCycleResult.Finished;

        //            default:
        //                throw new InvalidOperationException();
        //        }
        //    }

        //    public override void Reset()
        //    {
        //        _state = 0;
        //    }
        //}

        //private sealed class Simple : InstructionImplementation
        //{
        //    private readonly Action _func;

        //    public Simple(string mnemonic, Action func)
        //        : base(mnemonic)
        //    {
        //        _func = func;
        //    }

        //    public override InstructionCycleResult Cycle(Mos6502 cpu)
        //    {
        //        cpu.ReadMemory(cpu.PC); // Spurious read.
        //        _func();
        //        return InstructionCycleResult.Finished;
        //    }

        //    public override void Reset() { }
        //}

        public IEnumerable<DebuggerWindow> CreateDebuggerWindows()
        {
            yield return new CpuStateWindow(this);
            //yield return new DisassemblyWindow(this);
        }
    }

    public enum Mos6502CycleResult
    {
        ExecutingInstruction,
        FinishedInstruction,
        Paused,
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