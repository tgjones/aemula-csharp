namespace Aemula.Chips.Ricoh2C02
{
    partial class Ricoh2C02
    {
        private struct PpuCtrlRegister
        {
            public PackedByte Data;

            public byte NameTableX => Data.Get(0, 1);

            public byte NameTableY => Data.Get(1, 1);

            public VRamAddressIncrementMode VRamAddressIncrementMode => (VRamAddressIncrementMode)Data.Get(2, 1);

            public byte SpritePatternTableAddress => Data.Get(3, 1);

            public byte BackgroundPatternTableAddress => Data.Get(4, 1);

            public SpriteSize SpriteSize => (SpriteSize)Data.Get(5, 1);

            public bool SlaveMode => Data.GetBit(6);

            public bool EnableNmi => Data.GetBit(7);
        }

        private enum VRamAddressIncrementMode
        {
            Add1,
            Add32,
        }

        private enum SpriteSize
        {
            Size8x8,
            Size8x16,
        }

        private struct PpuMaskRegister
        {
            public PackedByte Data;

            public bool Grayscale => Data.GetBit(0);

            public bool RenderBackgroundLeft => Data.GetBit(1);

            public bool RenderSpritesLeft => Data.GetBit(2);

            public bool RenderBackground => Data.GetBit(3);

            public bool RenderSprites => Data.GetBit(4);

            public bool EmphasizeRed => Data.GetBit(5);

            public bool EmphasizeGreen => Data.GetBit(6);

            public bool EmphasizeBlue => Data.GetBit(7);
        }

        private struct PpuStatusRegister
        {
            public PackedByte Data;

            public byte Unused
            {
                set => Data.Set(0, 5, value);
            }

            public bool SpriteOverflow
            {
                set => Data.Set(5, 1, (byte)(value ? 1 : 0));
            }

            public bool Sprite0Hit
            {
                set => Data.Set(6, 1, (byte)(value ? 1 : 0));
            }

            public bool VBlankStarted
            {
                set => Data.Set(7, 1, (byte)(value ? 1 : 0));
            }
        }
    }
}
