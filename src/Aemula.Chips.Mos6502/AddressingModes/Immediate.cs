namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void AddressingModeImmediateCycle0()
        {
            Address = PC;
            PC += 1;
        }
    }
}
