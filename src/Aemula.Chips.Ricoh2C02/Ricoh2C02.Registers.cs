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

            public bool SlaveMode => Data.Get(6, 1) == 1;

            public bool EnableNmi => Data.Get(7, 1) == 1;
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

            public bool Grayscale => Data.Get(0, 1) == 1;

            public bool RenderBackgroundLeft => Data.Get(1, 1) == 1;

            public bool RenderSpritesLeft => Data.Get(2, 1) == 1;

            public bool RenderBackground => Data.Get(3, 1) == 1;

            public bool RenderSprites => Data.Get(4, 1) == 1;

            public bool EmphasizeRed => Data.Get(5, 1) == 1;

            public bool EmphasizeGreen => Data.Get(6, 1) == 1;

            public bool EmphasizeBlue => Data.Get(7, 1) == 1;
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
