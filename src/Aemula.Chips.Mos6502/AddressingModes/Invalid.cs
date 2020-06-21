namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void AddressingModeInvalidCycle0()
        {
            Address = PC;
        }

        private void AddressingModeInvalidCycle1()
        {
            Address.Hi = 0xFF;
            Address.Lo = 0xFF;
            _data = 0xFF;
            _ir -= 1;
        }
    }
}
