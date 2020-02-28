namespace Aemula.Chips.Arm
{
    internal static class UInt32Extensions
    {
        public static uint GetBit(this uint value, byte bit)
        {
            return (value >> bit) & 0b1;
        }

        public static bool GetBitAsBool(this uint value, byte bit)
        {
            return value.GetBit(bit) == 1;
        }

        public static uint GetBits(this uint value, byte bitStart, byte bitEnd)
        {
            var numBits = bitEnd - bitStart + 1;
            var mask = (1u << numBits) - 1;

            return (value >> bitStart) & mask;
        }
    }
}
