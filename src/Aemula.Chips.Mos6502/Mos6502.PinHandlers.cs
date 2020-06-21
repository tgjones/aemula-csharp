namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void OnResSet()
        {
            if (!_res)
            {
                _sync = true;
                _brkFlags = BrkFlags.Reset;
            }
        }

        // How this is actually supposed to work is:
        // - PHI1 is when address lines change.
        // - PHI2 is when the data is transferred.

        private void OnPhi0TransitionedLoToHi()
        {
            // TODO: Write to the data bus.

            _phi2 = _phi0;
            _phi1 = !_phi0;
        }

        private void OnPhi0TransitionedHiToLo()
        {
            // TODO: Read previously requested data from bus.
            // TODO: Put address onto the bus.
            // TODO: Set RW pin.
            // TODO: Set SYNC pin for an opcode fetch.

            // A low RDY pin, combined with a read cycle, pauses the CPU.
            if (_rdy || _rw)
            {
                // If SYNC pin is set, this is the start of a new instruction.
                // We will have the new opcode in the DATA pins.
                if (_sync)
                {
                    _ir = _data;
                    _tr = 0;
                    _sync = false;

                    if (_brkFlags != BrkFlags.None)
                    {
                        _ir = 0;
                        _res = false;
                    }
                    else
                    {
                        PC += 1;
                    }
                }

                // Assume we're going to read.
                _rw = true;

                // Call generated function with actual instruction dispatch.
                ExecuteInstruction();

                // Increment timing register.
                _tr += 1;
            }

            _phi2 = _phi0;
            _phi1 = !_phi0;
        }
    }
}
