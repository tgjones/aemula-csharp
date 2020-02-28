namespace Aemula.Chips.Intelx86
{
    partial class Intelx86
    {


        private ushort Add16(ushort dest, ushort src)
        {
            return (ushort)(dest + src);
        }

        private void Cmp16(ushort dest, ushort src)
        {
            Sub16(dest, src);
        }

        private void Sub16(ushort dest, ushort src)
        {
            var comparison = (ushort)(dest - src);

            SetOverflowFlag16(dest, src, comparison);
            SetSignFlag16(comparison);
            SetZeroFlag16(comparison);
            SetAuxiliaryCarryFlag16(dest, src, comparison);
            SetParityFlag(comparison);
            Flags.CF = dest < src;
        }

        private ushort Inc16(ushort value)
        {
            var result = (ushort)(value + 1);

            Flags.OF = result == 0x7FFF;
            Flags.SF = (result & 0x8000) != 0;
            Flags.ZF = result == 0;
            Flags.AF = (result & 0xF) == 0xF;
            Flags.PF = Parity(result);

            return result;
        }

        private ushort Dec16(ushort value)
        {
            var result = (ushort)(value - 1);

            Flags.OF = result == 0x7FFF;
            Flags.SF = (result & 0x8000) != 0;
            Flags.ZF = result == 0;
            Flags.AF = (result & 0xF) == 0xF;
            Flags.PF = Parity(result);

            return result;
        }

        private ushort Xor16(ushort dest, ushort src)
        {
            var result = (ushort)(dest ^ src);

            Flags.OF = result == 0x7FFF;
            Flags.SF = (result & 0x8000) != 0;
            Flags.ZF = result == 0;
            Flags.AF = (result & 0xF) == 0xF;
            Flags.PF = Parity(result);
            // CF

            return result;
        }
    }
}
