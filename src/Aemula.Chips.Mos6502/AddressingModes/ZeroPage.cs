namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void AddressingModeZeroPageCycle0()
        {
            Address = PC;
            PC += 1;
        }

        private void AddressingModeZeroPageCycle1()
        {
            Address.Hi = 0;
            Address.Lo = _data;
        }
    }
}
