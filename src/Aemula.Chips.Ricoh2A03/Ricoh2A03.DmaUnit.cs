using Aemula.Bus;

namespace Aemula.Chips.Ricoh2A03
{
    partial class Ricoh2A03
    {
        // When 4014 is written to, pause CPU and do 256 * read/write cycles from specified page to 2004
        // https://forums.nesdev.com/viewtopic.php?f=3&t=14120
        private sealed class DmaUnit
        {
            private readonly IBus<ushort, byte> _bus;
            private readonly Mos6502.Mos6502 _cpu;

            public DmaUnit(IBus<ushort, byte> bus, Mos6502.Mos6502 cpu)
            {
                _bus = bus;
                _cpu = cpu;
            }

            public DmaState DmaState;

            private bool _isDmaReadCycle;
            private byte _dmaHiByte;
            private byte _dmaLoByte;
            private byte _dmaValue;

            public void Cycle()
            {
                // DMA transfer can only become active on a read cycle.
                if (DmaState == DmaState.Pending && _isDmaReadCycle)
                {
                    DmaState = DmaState.Active;
                }

                // DMA flips between read / write cycles. This is happening all the time,
                // even if a DMA transfer is not in progress.

                if (_isDmaReadCycle)
                {
                    // Only do a read if a DMA transfer is actually active.
                    if (DmaState == DmaState.Active)
                    {
                        _dmaValue = _bus.Read((ushort)((_dmaHiByte << 8) | _dmaLoByte));
                    }
                    _isDmaReadCycle = false;
                }
                else
                {
                    // Only do a write if a DMA transfer is actually active.
                    if (DmaState == DmaState.Active)
                    {
                        _bus.Write(0x2004, _dmaValue);

                        // Check if we have finished the DMA transfer.
                        if (_dmaLoByte == 0xFF)
                        {
                            DmaState = DmaState.Inactive;

                            // Let CPU continue on next clock cycle.
                            _cpu.Rdy = true;
                        }
                        else
                        {
                            _dmaLoByte++;
                        }
                    }

                    _isDmaReadCycle = true;
                }
            }

            public void SetHiByte(byte value)
            {
                _dmaHiByte = value;
                _dmaLoByte = 0;
            }
        }

        private enum DmaState
        {
            Inactive,
            Pending,
            Active,
        }
    }
}
