namespace Aemula
{
    public struct PackedByte
    {
        private static readonly byte[] BitMasks =
        {
            0x00,
            0x01,
            0x03,
            0x07,
            0x0F,
            0x1F,
            0x3F,
            0x7F,
            0xFF,
        };

        public byte Value;

        public PackedByte(byte value)
        {
            Value = value;
        }

        public byte Get(int start, int count)
        {
            var mask = BitMasks[count];
            return (byte)(Value & (mask << start));
        }

        public void Set(int start, int count, byte data)
        {
            var mask = BitMasks[count];
            Value = (byte)((Value & (~(mask << start))) | (data << start));
        }
    }
}
