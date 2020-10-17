using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Aemula.Bus;
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
        private static readonly TimeSpan TimePerCpuTick = TimeSpan.FromSeconds(1.0f / CpuFrequency);

        private readonly byte[] _ram;
        private readonly byte[] _vram;

        private Mos6502Pins _cpuPins;
        private Ricoh2C02Pins _ppuPins;

        private TimeSpan _nextUpdateTime = TimeSpan.Zero;

        private Cartridge _cartridge;

        public readonly Ricoh2A03 Cpu;

        public readonly Bus<ushort, byte> PpuBus;
        public readonly Ricoh2C02 Ppu;

        public Nes()
        {
            (Cpu, _cpuPins) = Ricoh2A03.Create();

            PpuBus = new Bus<ushort, byte>();
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
            Cpu.Cycle(ref _cpuPins);

            //if (_cpuPins.Sync)
            //{
            //    var cpu = Cpu.CpuCore;
            //    var cycles = 0;
            //    Debug.WriteLine($"{cpu.PC.Value:X4}  A:{cpu.A:X2} X:{cpu.X:X2} Y:{cpu.Y:X2} P:{cpu.P.AsByte(false):X2} SP:{cpu.SP:X2} CPUC:{cycles - 7}");
            //}

            var address = _cpuPins.Address.Value;

            // The 3 high bits dictate which chips are selected.
            var a13_a15 = address >> 13;

            switch (a13_a15)
            {
                case 0b000: // Internal RAM. Only address pins A0..A10 are connected.
                    if (_cpuPins.RW)
                    {
                        _cpuPins.Data = _ram[address & 0x7FF];
                    }
                    else
                    {
                        _ram[address & 0x7FF] = _cpuPins.Data;
                    }
                    break;

                case 0b001: // PPU ports. Only address pins A0..A2 are connected.
                    _ppuPins.CpuRW = _cpuPins.RW;
                    _ppuPins.CpuAddress = (byte)(address & 0x7);
                    _ppuPins.CpuData = _cpuPins.Data;
                    Ppu.CpuCycle(ref _ppuPins);

                    // Now handle reads / writes on PPU data bus.
                    var pa13 = (_ppuPins.PpuAddress >> 13) & 1;
                    if (_ppuPins.PpuRW)
                    {
                        if (pa13 == 1)
                        {
                            _ppuPins.PpuData = _vram[_ppuPins.PpuAddress & 0x7FF];
                        }
                        else
                        {
                            // TODO: Use mapper.
                            _ppuPins.PpuData = _cartridge?.ChrRom[_ppuPins.PpuAddress] ?? 0;
                        }
                    }
                    else
                    {
                        if (pa13 == 1)
                        {
                            _vram[_ppuPins.PpuAddress & 0x7FF] = _ppuPins.PpuData;
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
                    if (_cpuPins.RW)
                    {
                        // TODO: Mapper implementations.
                        // What follows is NROM-128, mapper 0.
                        _cpuPins.Data = _cartridge?.PrgRom[address & 0x3FFF] ?? 0;
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
            _cpuPins.Res = true;
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
