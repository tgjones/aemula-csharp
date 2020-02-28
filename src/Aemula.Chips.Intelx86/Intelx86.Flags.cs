namespace Aemula.Chips.Intelx86
{
    partial class Intelx86
    {
        private void SetParityFlag(uint value)
        {
            Flags.PF = Parity(value);
        }

        private void SetSignFlag16(ushort value)
        {
            Flags.SF = (value & 0x8000) != 0;
        }

        private void SetZeroFlag16(ushort value)
        {
            Flags.ZF = value == 0;
        }

        private void SetAuxiliaryCarryFlag16(ushort operand1, ushort operand2, ushort result)
        {
            Flags.AF = ((operand1 ^ operand2 ^ result) & 0x10) != 0;
        }

        private void SetOverflowFlag16(ushort operand1, ushort operand2, ushort result)
        {
            // There is overflow if the operands do not have different signs,
            // and the input sign is different from the output sign.
            Flags.OF = ((operand1 ^ result) & (operand2 ^ result) & 0x8000) != 0;
        }
    }
}
