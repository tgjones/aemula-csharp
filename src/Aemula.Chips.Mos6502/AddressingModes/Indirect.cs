namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        /// <summary>
        /// Read low byte of target address.
        /// </summary>
        private void AddressingModeIndirectCycle0()
        {
            Address = PC;
            PC += 1;
        }

        /// <summary>
        /// Read high byte of target address.
        /// </summary>
        private void AddressingModeIndirectCycle1()
        {
            Address = PC;
            PC += 1;
            _ad.Lo = _data;
        }

        /// <summary>
        /// Read low byte of pointer stored at target address.
        /// </summary>
        private void AddressingModeIndirectCycle2()
        {
            _ad.Hi = _data;
            Address = _ad;
        }

        /// <summary>
        /// Read high byte of pointer stored at (target address + 1).
        /// </summary>
        private void AddressingModeIndirectCycle3()
        {
            Address.Lo = (byte)(_ad.Lo + 1);
            _ad.Lo = _data;
        }

        /// <summary>
        /// Read value at pointer.
        /// </summary>
        private void AddressingModeIndirectCycle4()
        {
            Address.Hi = _data;
            Address.Lo = _ad.Lo;
        }
    }
}
