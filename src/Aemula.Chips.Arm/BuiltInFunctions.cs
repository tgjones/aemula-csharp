using System;

namespace Aemula.Chips.Arm
{
    internal static class BuiltInFunctions
    {
        public const uint Unknown = 0xDEADBEEF;

        public static byte BitCount(uint value)
        {
            var result = (byte)0;
            for (var i = 1u; i != 0; i <<= 1)
            {
                if ((value & i) != 0)
                {
                    result++;
                }
            }
            return result;
        }

        public static byte LowestSetBit(uint value)
        {
            var result = (byte)0;
            for (var i = 1u; i != 0; i <<= 1, result++)
            {
                if ((value & i) == 1)
                {
                    return result;
                }
            }
            return 33;
        }

        // P.5.3
        public static bool IsZero(uint value)
        {
            return value == 0;
        }

        // A5.2.4
        public static uint ARMExpandImm(uint imm12)
        {
            var (imm32, _) = ARMExpandImm_C(imm12, false);
            return imm32;
        }

        // A5.2.4
        public static (uint imm32, bool carryOut) ARMExpandImm_C(uint imm12, bool carryIn)
        {
            var unrotatedValue = ZeroExtend(imm12.GetBits(0, 7));
            return Shift_C(unrotatedValue, ShiftRotateType.Ror, (int)(2 * imm12.GetBits(8, 11)), carryIn);
        }

        // A8.4.3
        public static (ShiftRotateType type, uint shift) DecodeImmShift(uint type, uint imm5)
        {
            switch (type)
            {
                case 0b00:
                    return (ShiftRotateType.Lsl, imm5);

                case 0b01:
                    return (ShiftRotateType.Lsr, imm5 == 0 ? 32 : imm5);

                case 0b10:
                    return (ShiftRotateType.Asr, imm5 == 0 ? 32 : imm5);

                case 0b11:
                    return (imm5 == 0)
                        ? (ShiftRotateType.Rrx, 1)
                        : (ShiftRotateType.Ror, imm5);

                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        // A8.4.3
        public static uint Shift(uint value, ShiftRotateType type, int amount, bool carryIn)
        {
            var (result, _) = Shift_C(value, type, amount, carryIn);
            return result;
        }

        // A8.4.3
        public static (uint result, bool carryOut) Shift_C(uint value, ShiftRotateType type, int amount, bool carryIn)
        {
            if (type == ShiftRotateType.Rrx && amount != 1)
            {
                throw new InvalidOperationException();
            }

            if (amount == 0)
            {
                return (value, carryIn);
            }

            switch (type)
            {
                case ShiftRotateType.Lsl:
                    return LSL_C(value, amount);

                case ShiftRotateType.Lsr:
                    return LSR_C(value, amount);

                case ShiftRotateType.Asr:
                    return ASR_C(value, amount);

                case ShiftRotateType.Ror:
                    return ROR_C(value, amount);

                case ShiftRotateType.Rrx:
                    return RRX_C(value, carryIn);

                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        // A2.2.1
        public static (uint result, bool carryOut) LSL_C(uint x, int shift)
        {
            if (shift <= 0)
            {
                throw new InvalidOperationException();
            }

            var result = x << shift;
            var carryOut = ((x >> (32 - shift)) & 0b1) == 1;

            return (result, carryOut);
        }

        // A2.2.1
        public static uint LSL(uint x, int shift)
        {
            if (shift < 0)
            {
                throw new InvalidOperationException();
            }

            return x << shift;
        }

        // A2.2.1
        public static (uint result, bool carryOut) LSR_C(uint x, int shift)
        {
            if (shift <= 0)
            {
                throw new InvalidOperationException();
            }

            var result = x >> shift;
            var carryOut = ((x >> (shift - 1)) & 0b1) == 1;

            return (result, carryOut);
        }

        // A2.2.1
        public static uint LSR(uint x, int shift)
        {
            if (shift < 0)
            {
                throw new InvalidOperationException();
            }

            return x >> shift;
        }

        // A2.2.1
        public static (uint result, bool carryOut) ASR_C(uint x, int shift)
        {
            if (shift <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            var result = (uint)((int)x >> shift);
            var carryOut = ((x >> (shift - 1)) & 0b1) == 1;

            return (result, carryOut);
        }

        // A2.2.1
        public static (uint result, bool carryOut) ROR_C(uint x, int shift)
        {
            if (shift == 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            var m = shift % 32;
            var result = LSR(x, m) | LSL(x, 32 - m);
            var carryOut = result.GetBit(31) == 1;

            return (result, carryOut);
        }

        // A2.2.1
        public static (uint result, bool carryOut) RRX_C(uint x, bool carryIn)
        {
            var result = ((carryIn ? 1u : 0) << 31) | (x >> 1);
            var carryOut = x.GetBit(0) == 1;
            return (result, carryOut);
        }

        // A2.2.1
        public static (uint result, bool carryOut, bool overflow) AddWithCarry(uint x, uint y, bool carryIn)
        {
            var unsignedSum = (ulong)x + (ulong)y + (carryIn ? 1u : 0);
            var signedSum = (long)x + (long)y + (carryIn ? 1u : 0);
            var result = (uint)(unsignedSum & 0xFFFFFFFF);
            var carryOut = (unsignedSum >> 32) == 1;
            var overflow = (long)result != signedSum;
            return (result, carryOut, overflow);
        }

        // P.5.3
        public static uint ZeroExtend(uint value)
        {
            return value;
        }

        // P.5.3
        public static uint SignExtend(uint value, int existingNumBits)
        {
            if ((value & (1 << (existingNumBits - 1))) == 1)
            {
                return (0xFFFFFFFF << existingNumBits) | value;
            }
            return value;
        }
    }

    // A8.4.3
    internal enum ShiftRotateType : byte
    {
        /// <summary>
        /// Logical shift left
        /// </summary>
        Lsl,

        /// <summary>
        /// Logical shift right
        /// </summary>
        Lsr,

        /// <summary>
        /// Arithmetic shift right
        /// </summary>
        Asr,

        /// <summary>
        /// Rotate right
        /// </summary>
        Ror,

        // Rotate right with extend
        Rrx
    }
}
