using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        private static readonly TimeSpan TimePerCpuTick = TimeSpan.FromSeconds(1.0f / CpuFrequency);

        private readonly byte[] _ram;
        private readonly byte[] _vram;

        private TimeSpan _nextUpdateTime = TimeSpan.Zero;

        private Cartridge _cartridge;

        public readonly Ricoh2A03 Cpu;

        public readonly Ricoh2C02 Ppu;

        public Nes()
        {
            Cpu = new Ricoh2A03();

            Ppu = new Ricoh2C02();

            _ram = new byte[0x0800];
            _vram = new byte[0x0800];
        }

        public void Update(TimeSpan time)
        {
            if (_nextUpdateTime == TimeSpan.Zero)
            {
                _nextUpdateTime = time;
            }

            while (time > _nextUpdateTime)
            {
                Tick();
                _nextUpdateTime += TimePerCpuTick;
            }
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

            //if (_cpuPins.Sync)
            //{
            //    var cpu = Cpu.CpuCore;
            //    var cycles = 0;
            //    Debug.WriteLine($"{cpu.PC.Value:X4}  A:{cpu.A:X2} X:{cpu.X:X2} Y:{cpu.Y:X2} P:{cpu.P.AsByte(false):X2} SP:{cpu.SP:X2} CPUC:{cycles - 7}");
            //}

            ref var cpuPins = ref Cpu.CpuCore.Pins;
            ref var ppuPins = ref Ppu.Pins;

            var address = cpuPins.Address;

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

        private void DoPpuCycle()
        {

        }

        public void InsertCartridge(Cartridge cartridge)
        {
            _cartridge = cartridge;
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
