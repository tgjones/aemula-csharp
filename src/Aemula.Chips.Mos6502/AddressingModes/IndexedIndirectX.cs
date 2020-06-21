namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void AddressingModeIndexedIndirectXCycle0()
        {
            Address = PC;
            PC += 1;
        }

        private void AddressingModeIndexedIndirectXCycle1()
        {
            _ad.Hi = 0;
            _ad.Lo = _data;
            Address = _ad;
        }

        private void AddressingModeIndexedIndirectXCycle2()
        {
            _ad.Lo += X;
            Address = _ad;
        }

        private void AddressingModeIndexedIndirectXCycle3()
        {
            Address.Lo = (byte)(_ad.Lo + 1);
            _ad.Lo = _data;
        }

        private void AddressingModeIndexedIndirectXCycle4()
        {
            Address.Hi = _data;
            Address.Lo = _ad.Lo;
        }
    }
}
