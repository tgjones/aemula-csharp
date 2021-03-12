using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Aemula.Chips.Mos6502;
using Aemula.Chips.Ricoh2A03;
using Aemula.Chips.Ricoh2C02;
using Aemula.Consoles.Nes.UI;
using Aemula.UI;

namespace Aemula.Consoles.Nes
{
    public sealed class Nes : EmulatedSystem
    {
        private const int CpuFrequency = 1789773;
        //private const int CpuFrequency = 20;
        private static readonly double ClocksPerMillisecond = CpuFrequency / 1000.0;

        private readonly byte[] _ram;
        private readonly byte[] _vram;

        private Cartridge _cartridge;

        private ushort _lastPC;

        public readonly Ricoh2A03 Cpu;

        public readonly Ricoh2C02 Ppu;

        public Nes()
        {
            Cpu = new Ricoh2A03();

            Ppu = new Ricoh2C02();

            _ram = new byte[0x0800];
            _vram = new byte[0x0800];
        }

        public override void RunForDuration(TimeSpan duration)
        {
            var clocks = (int)Math.Round(ClocksPerMillisecond * duration.TotalMilliseconds);

            for (var i = 0; i < clocks; i++)
            {
                Tick();
            }
        }

        public override void StepInstruction()
        {
            ref readonly var pins = ref Cpu.CpuCore.Pins;

            var lastRdy = pins.Rdy;

            while (true)
            {
                Tick();

                if ((pins.Sync || lastRdy) && !pins.Rdy)
                {
                    break;
                }

                lastRdy = pins.Rdy;
            }
        }

        public override void StepCpuCycle()
        {
            Tick();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Tick()
        {
            DoCpuCycle();

            for (var i = 0; i < 3; i++)
            {
                DoPpuCycle();
            }
        }

        private void DoCpuCycle()
        {
            Cpu.Cycle();

            ref var cpuPins = ref Cpu.CpuCore.Pins;
            ref var ppuPins = ref Ppu.Pins;

            var address = cpuPins.Address;

            if (cpuPins.Sync)
            {
                _lastPC = address;

                //var cpu = Cpu.CpuCore;
                //var cycles = 0;
                //Debug.WriteLine($"{cpu.PC.Value:X4}  A:{cpu.A:X2} X:{cpu.X:X2} Y:{cpu.Y:X2} P:{cpu.P.AsByte(false):X2} SP:{cpu.SP:X2} CPUC:{cycles - 7}");
            }

            // The 3 high bits dictate which chips are selected.
            var a13_a15 = address >> 13;

            switch (a13_a15)
            {
                case 0b000: // Internal RAM. Only address pins A0..A10 are connected.
                    if (cpuPins.RW)
                    {
                        cpuPins.Data = _ram[address & 0x7FF];
                    }
                    else
                    {
                        _ram[address & 0x7FF] = cpuPins.Data;
                    }
                    break;

                case 0b001: // PPU ports. Only address pins A0..A2 are connected.
                    ppuPins.CpuRW = cpuPins.RW;
                    ppuPins.CpuAddress = (byte)(address & 0x7);
                    ppuPins.CpuData = cpuPins.Data;
                    Ppu.CpuCycle();
                    cpuPins.Data = ppuPins.CpuData;

                    // Now handle reads / writes on PPU data bus.
                    var pa13 = (ppuPins.PpuAddress >> 13) & 1;
                    if (ppuPins.PpuRW)
                    {
                        if (pa13 == 1)
                        {
                            ppuPins.PpuData = _vram[ppuPins.PpuAddress & 0x7FF];
                        }
                        else
                        {
                            // TODO: Use mapper.
                            ppuPins.PpuData = _cartridge?.ChrRom[ppuPins.PpuAddress] ?? 0;
                        }
                    }
                    else
                    {
                        if (pa13 == 1)
                        {
                            _vram[ppuPins.PpuAddress & 0x7FF] = ppuPins.PpuData;
                        }
                        else
                        {
                            // Can't write to CHR ROM, maybe?
                        }
                    }

                    break;

                // $4000-$401F is mapped internally on 2A03 chip.

                case 0b100: // ROMSEL. Only address pins A0..A14 are connected.
                case 0b101:
                case 0b110:
                case 0b111:
                    // This is ROM - can't write to it.
                    if (cpuPins.RW)
                    {
                        // TODO: Mapper implementations.
                        // What follows is NROM-128, mapper 0.
                        cpuPins.Data = _cartridge?.PrgRom[address & 0x3FFF] ?? 0;
                    }
                    break;
            }
        }

        public override ushort LastPC => _lastPC;

        public override SortedDictionary<ushort, DisassembledInstruction> Disassemble()
        {
            return Mos6502.Disassemble(ReadByteDebug);
        }

        private byte ReadByteDebug(ushort address)
        {
            // The 3 high bits dictate which chips are selected.
            var a13_a15 = address >> 13;

            switch (a13_a15)
            {
                case 0b000: // Internal RAM. Only address pins A0..A10 are connected.
                    return  _ram[address & 0x7FF];

                case 0b100: // ROMSEL. Only address pins A0..A14 are connected.
                case 0b101:
                case 0b110:
                case 0b111:
                    // TODO: Mapper implementations.
                    // What follows is NROM-128, mapper 0.
                    return _cartridge?.PrgRom[address & 0x3FFF] ?? 0;

                default:
                    // TODO: Read from PPU registers etc.
                    return 0;
            }
        }

        private void DoPpuCycle()
        {
            Ppu.Cycle();
        }

        public override void LoadProgram(string filePath)
        {
            var cartridge = Cartridge.FromFile(filePath);
            InsertCartridge(cartridge);

            Reset();
        }

        private void InsertCartridge(Cartridge cartridge)
        {
            _cartridge = cartridge;

            RaiseProgramLoaded();
        }

        public override void Reset()
        {
            Cpu.CpuCore.Pins.Res = true;
        }

        internal byte ReadChrRom(ushort address)
        {
            // TODO: Use mapper.
            return _cartridge?.ChrRom[address] ?? 0;
        }

        public override IEnumerable<DebuggerWindow> CreateDebuggerWindows()
        {
            foreach (var debuggerWindow in Cpu.CreateDebuggerWindows())
            {
                yield return debuggerWindow;
            }

            foreach (var debuggerWindow in Ppu.CreateDebuggerWindows())
            {
                yield return debuggerWindow;
            }

            yield return new PatternTableWindow(this);
        }
    }
}
