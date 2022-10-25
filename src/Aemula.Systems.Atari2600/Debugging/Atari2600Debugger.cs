using Aemula.Chips.Mos6502.Debugging;
using Aemula.Debugging;
using Aemula.UI;
using System.Collections.Generic;

namespace Aemula.Systems.Atari2600.Debugging;

internal sealed class Atari2600Debugger : Debugger
{
    private static readonly Dictionary<ushort, string> Equates = new()
    {
        //{ 0x2000, "PPU_CTRL" },
        //{ 0x2001, "PPU_MASK" },
        //{ 0x2002, "PPU_STATUS" },
        //{ 0x2003, "OAM_ADDR" },
        //{ 0x2004, "OAM_DATA" },
        //{ 0x2005, "PPU_SCROLL" },
        //{ 0x2006, "PPU_ADDR" },
        //{ 0x2007, "PPU_DATA" },
    };

    private readonly Atari2600 _system;
    private readonly Mos6502Debugger _mos6502Debugger;

    public Atari2600Debugger(Atari2600 system)
        : base(system, CreateMemoryCallbacks(system))
    {
        _system = system;

        _mos6502Debugger = new Mos6502Debugger(system.Cpu);
        _mos6502Debugger.RegisterStepModes(this);

        StepModes.Add(new DebuggerStepMode("Step PPU Cycle", () => true));
    }

    private static DebuggerMemoryCallbacks CreateMemoryCallbacks(Atari2600 system)
    {
        return new DebuggerMemoryCallbacks(system.ReadByteDebug, system.WriteByteDebug);
    }

    protected override Disassembler CreateDisassembler()
    {
        return new Mos6502Disassembler(MemoryCallbacks, Equates, hasNmi: false, hasIrq: false);
    }

    protected override void TickSystem()
    {
        base.TickSystem();

        if (_system.Cpu.Pins.Sync)
        {
            OnAddressExecuting(_system.Cpu.Pins.Address);
        }
    }

    public override IEnumerable<DebuggerWindow> CreateDebuggerWindows()
    {
        foreach (var debuggerWindow in base.CreateDebuggerWindows())
        {
            yield return debuggerWindow;
        }

        foreach (var debuggerWindow in _system.Cpu.CreateDebuggerWindows())
        {
            yield return debuggerWindow;
        }

        yield return new BreakpointsWindow(this);

        yield return new ScreenDisplayWindow(_system.VideoOutput.DisplayBuffer);

        //yield return new MemoryEditor(1, _system.ReadByteDebug, _system.WriteByteDebug);
    }
}
