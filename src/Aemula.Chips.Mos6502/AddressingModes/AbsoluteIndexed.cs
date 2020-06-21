namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        /// <summary>
        /// Set address bus to PC (to fetch BAL, low byte of base address), increment PC.
        /// </summary>
        private void AddressingModeAbsoluteIndexedCycle0()
        {
            Address = PC;
            PC += 1;
        }

        /// <summary>
        /// Set address bus to PC (to fetch BAH, high byte of base address), increment PC.
        /// </summary>
        private void AddressingModeAbsoluteIndexedCycle1()
        {
            Address = PC;
            PC += 1;
            _ad.Lo = _data;
        }

        /// <summary>
        /// Set address bus to BAH,BAL+index
        /// </summary>
        private void AddressingModeAbsoluteIndexedCycle2(byte indexRegisterValue)
        {
            _ad.Hi = _data;
            Address.Hi = _ad.Hi;
            Address.Lo = (byte)(_ad.Lo + indexRegisterValue);
        }

        /// <summary>
        /// If, when the index register is added to BAL (the low byte of the base address),
        /// the resulting address is on the same page, then we skip the next cycle.
        /// 
        /// Otherwise if it's on the next page, then we execute an extra cycle
        /// to add the carry value to BAH (the high byte of the base address).
        /// 
        /// This conditional check only happens for instructions that read memory.
        /// For instructions that write to memory, we always execute the extra cycle.
        /// </summary>
        private void AddressingModeAbsoluteIndexedCycle2Read(byte indexRegisterValue)
        {
            var withoutCarry = _ad.Hi;
            var withCarry = (_ad + indexRegisterValue).Hi;
            if (withoutCarry == withCarry)
            {
                _tr += 1;
            }
        }

        private void AddressingModeAbsoluteIndexedCycle3(byte indexRegisterValue)
        {
            Address = _ad + indexRegisterValue;
        }
    }
}
