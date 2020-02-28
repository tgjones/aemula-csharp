using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Aemula.Chips.Intelx86
{
    internal static class Alu
    {
        public static ushort Sub16(ushort dest, ushort src, ref ProcessorFlags flags)
        {
            var result = (ushort)(dest - src);

            //SetOverflowFlag16(dest, src, result);
            //SetSignFlag16(result);
            //SetZeroFlag16(result);
            //SetAuxiliaryCarryFlag16(dest, src, result);
            //SetParityFlag(result);
            //Flags.CF = dest < src;

            return result;
        }

        [Flags]
        private enum ChangedFlags : byte
        {
            AF,
            CF,
            OF,
            PF,
            SF,
            ZF
        }

        private static void UpdateFlags16(
            ushort operand1, 
            ushort operand2, 
            ushort result,
            ref ProcessorFlags flags,
            ChangedFlags changedFlags)
        {
            if (changedFlags.HasFlag(ChangedFlags.AF))
            {
                flags.AF = ((operand1 ^ operand2 ^ result) & 0x10) != 0;
            }

            if (changedFlags.HasFlag(ChangedFlags.CF))
            {
                // Check that:
                // - operands have the same sign

                flags.CF = ((operand1 ^ (operand1 ^ operand2) & (operand2 ^ result)) & 0x8000) != 0;
            }

            if (changedFlags.HasFlag(ChangedFlags.OF))
            {
                // There is overflow if the operands have the same sign,
                // and the operand sign is different from the result sign.
                flags.OF = ((operand1 ^ result) & (operand2 ^ result) & 0x8000) != 0;
            }

            if (changedFlags.HasFlag(ChangedFlags.PF))
            {
                flags.PF = Parity(result);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Parity(uint value)
        {
            if (Popcnt.IsSupported)
            {
                return (Popcnt.PopCount(value & 0xFF) & 1) == 1;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        //private void SetSignFlag16(ushort value)
        //{
        //    Flags.SF = (value & 0x8000) != 0;
        //}

        //private void SetZeroFlag16(ushort value)
        //{
        //    Flags.ZF = value == 0;
        //}

        //private void SetAuxiliaryCarryFlag16(ushort operand1, ushort operand2, ushort result)
        //{
            
        //}

        //private void SetOverflowFlag16(ushort operand1, ushort operand2, ushort result)
        //{
        //    // There is overflow if the operands do not have different signs,
        //    // and the input sign is different from the output sign.
        //    Flags.OF = ((operand1 ^ result) & (operand2 ^ result) & 0x8000) != 0;
        //}
    }
}
