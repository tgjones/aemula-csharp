using System.Collections.Generic;
using Aemula.Bus;
using Aemula.Clock;
using Aemula.UI;

namespace Aemula.Chips.Ricoh2A03
{
    public sealed partial class Ricoh2A03 : IClockable
    {
        private const ushort OamDmaAddress = 0x4014;
        private readonly DmaUnit _dmaUnit;

        public Mos6502.Mos6502 CpuCore { get; }

        public Ricoh2A03(IBus<ushort, byte> bus)
        {
            var wrappedBus = new WrappedBus<ushort, byte>(
                bus,
                address => address >= 0x4000 & address <= 0x401F,
                Read,
                Write);

            CpuCore = new Mos6502.Mos6502(wrappedBus);

            _dmaUnit = new DmaUnit(bus, CpuCore);
        }

        public void Cycle()
        {
            // TODO: APU stuff.

            _dmaUnit.Cycle();

            if (_dmaUnit.DmaState != DmaState.Inactive)
            {
                return;
            }

            var cycleResult = CpuCore.Cycle();

            // Did CPU become paused on this cycle? If so it means we previously requested it
            // to pause so that we can start a DMA transfer.
            if (cycleResult == Mos6502.Mos6502CycleResult.Paused)
            {
                _dmaUnit.DmaState = DmaState.Pending;
            }
        }

        private byte Read(ushort address)
        {
            switch (address)
            {
                case OamDmaAddress: // Write-only
                    return 0;

                default:
                    // TODO
                    return 0;
                    //throw new ArgumentOutOfRangeException(nameof(address));
            }
        }

        private void Write(ushort address, byte data)
        {
            switch (address)
            {
                case OamDmaAddress:
                    _dmaUnit.SetHiByte(data);

                    // Tell CPU we want to pause it at the next read cycle.
                    CpuCore.Rdy = false;

                    break;

                default:
                    // TODO
                    //throw new ArgumentOutOfRangeException(nameof(address));
                    break;
            }
        }

        public void Reset()
        {
            CpuCore.Reset();
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
