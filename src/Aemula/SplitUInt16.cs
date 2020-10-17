using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Aemula
{
    [StructLayout(LayoutKind.Explicit, Size = 2)]
    public struct SplitUInt16
    {
        [FieldOffset(0)]
        public byte Lo;

        [FieldOffset(1)]
        public byte Hi;

        [FieldOffset(0)]
        public ushort Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SplitUInt16 operator +(SplitUInt16 lhs, byte rhs)
        {
            return new SplitUInt16
            {
                Value = (ushort)(lhs.Value + rhs)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SplitUInt16 operator +(SplitUInt16 lhs, sbyte rhs)
        {
            return new SplitUInt16
            {
                Value = (ushort)(lhs.Value + rhs)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static SplitUInt16 operator ++(SplitUInt16 lhs)
        {
            return new SplitUInt16
            {
                Value = (ushort)(lhs.Value + 1)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator SplitUInt16(byte value)
        {
            return new SplitUInt16
            {
                Value = value
            };
        }
    }
}
