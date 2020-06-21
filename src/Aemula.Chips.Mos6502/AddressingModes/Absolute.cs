namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void AddressingModeAbsoluteCycle0()
        {
            Address = PC;
            PC += 1;
        }

        private void AddressingModeAbsoluteCycle1()
        {
            Address = PC;
            PC += 1;
            _ad.Lo = _data;
        }

        private void AddressingModeAbsoluteCycle2()
        {
            Address.Hi = _data;
            Address.Lo = _ad.Lo;
        }
    }
}
