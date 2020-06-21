namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void AddressingModeIndirectIndexedYCycle0()
        {
            Address = PC;
            PC += 1;
        }

        private void AddressingModeIndirectIndexedYCycle1()
        {
            _ad.Hi = 0;
            _ad.Lo = _data;
            Address = _ad;
        }

        private void AddressingModeIndirectIndexedYCycle2()
        {
            Address.Lo = (byte)(_ad.Lo + 1);
            _ad.Lo = _data;
        }

        private void AddressingModeIndirectIndexedYCycle3()
        {
            _ad.Hi = _data;
            Address.Hi = _ad.Hi;
            Address.Lo = (byte)(_ad.Lo + Y);
        }

        private void AddressingModeIndirectIndexedYCycle3Read()
        {
            var withoutCarry = _ad.Hi;
            var withCarry = (_ad + Y).Hi;
            if (withoutCarry == withCarry)
            {
                _tr += 1;
            }
        }

        /// <summary>
        /// This cycle can be skipped for read access if page boundary is not crossed.
        /// </summary>
        private void AddressingModeIndirectIndexedYCycle4()
        {
            Address = _ad + Y;
        }
    }
}
