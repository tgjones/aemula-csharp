using static Aemula.BitUtility;

namespace Aemula.Chips.Tia
{
    internal static class TiaUtility
    {
        /// <summary>
        /// Updates 6-bit counter value.
        /// </summary>
        public static byte UpdatePolynomialCounter(byte counter)
        {
            // Put high 5 bits from old value into low 5 bits in new value.
            var newLoBits = (counter >> 1) & 0b11111;

            // New high bit is 1 if the low 2 bits are the same, and 0 if they are different.
            var newHiBit = (GetBit(counter, 0) == GetBit(counter, 1)) ? 1 : 0;

            return (byte)((newHiBit << 5) | newLoBits);
        }

        public static bool NoneEqual(byte lhs, byte rhs)
        {
            var result = 1;

            result &= (GetBit(lhs, 0) ^ GetBit(rhs, 0));
            result &= (GetBit(lhs, 1) ^ GetBit(rhs, 1));
            result &= (GetBit(lhs, 2) ^ GetBit(rhs, 2));
            result &= (GetBit(lhs, 3) ^ GetBit(rhs, 3));

            return result == 1;
        }
    }
}
