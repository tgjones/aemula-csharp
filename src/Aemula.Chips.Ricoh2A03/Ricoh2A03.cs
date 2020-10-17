﻿using System.Collections.Generic;
using Aemula.Chips.Mos6502;
using Aemula.UI;

namespace Aemula.Chips.Ricoh2A03
{
    public sealed partial class Ricoh2A03
    {
        public static (Ricoh2A03, Mos6502Pins) Create()
        {
            var (mos6502, pins) = Mos6502.Mos6502.Create(Mos6502Options.Default);

            var ricoh2A03 = new Ricoh2A03(mos6502);

            return (ricoh2A03, pins);
        }

        private const ushort OamDmaAddress = 0x4014;
        private readonly DmaUnit _dmaUnit;

        public readonly Mos6502.Mos6502 CpuCore;

        private Ricoh2A03(Mos6502.Mos6502 cpuCore)
        {
            CpuCore = cpuCore;

            _dmaUnit = new DmaUnit();
        }

        public void Cycle(ref Mos6502Pins pins)
        {
            // TODO: APU stuff.

            _dmaUnit.Cycle(ref pins);

            if (_dmaUnit.DmaState != DmaState.Inactive)
            {
                return;
            }

            CpuCore.Tick(ref pins);

            var address = pins.Address.Value;

            // Is the address in the DMA range?
            if (address >= 0x4000 && address <= 0x401F)
            {
                if (pins.RW)
                {
                    pins.Data = address switch
                    {
                        // Write-only
                        OamDmaAddress => 0,

                        // TODO
                        _ => 0
                    };
                }
                else
                {
                    switch (address)
                    {
                        case OamDmaAddress:
                            _dmaUnit.SetHiByte(pins.Data);

                            // Tell CPU we want to pause it at the next read cycle.
                            pins.Rdy = true;

                            break;

                        default:
                            // TODO
                            //throw new ArgumentOutOfRangeException(nameof(address));
                            break;
                    }
                }
            }

            // Did CPU become paused on this cycle? If so it means we previously requested it
            // to pause so that we can start a DMA transfer.
            if (pins.RW && pins.Rdy)
            {
                _dmaUnit.DmaState = DmaState.Pending;
            }
        }

        public IEnumerable<DebuggerWindow> CreateDebuggerWindows()
        {
            foreach (var debuggerWindow in CpuCore.CreateDebuggerWindows())
            {
                yield return debuggerWindow;
            }
        }
    }
}
