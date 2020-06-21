namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void AddressingModeZeroPageIndexedCycle0()
        {
            Address = PC;
            PC += 1;
        }

        private void AddressingModeZeroPageIndexedCycle1()
        {
            _ad.Hi = 0;
            _ad.Lo = _data;
            Address = _ad;
        }

        private void AddressingModeZeroPageIndexedCycle2(byte indexRegisterValue)
        {
            Address.Hi = 0;
            Address.Lo = (byte)(_ad.Lo + indexRegisterValue);
        }

        private void AddressingModeZeroPageXCycle2() => AddressingModeZeroPageIndexedCycle2(X);

        private void AddressingModeZeroPageYCycle2() => AddressingModeZeroPageIndexedCycle2(Y);
    }
}
