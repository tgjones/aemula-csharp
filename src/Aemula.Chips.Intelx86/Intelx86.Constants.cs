namespace Aemula.Chips.Intelx86
{
    partial class Intelx86
    {
        private const byte RegAX = 0;
        private const byte RegCX = 1;
        private const byte RegDX = 2;
        private const byte RegBX = 3;
        private const byte RegSP = 4;
        private const byte RegBP = 5;
        private const byte RegSI = 6;
        private const byte RegDI = 7;

        private struct ModRM
        {
            public OperandMode Mod;
            public byte RegOrOpcodeExtension;
            public byte RM;
            public byte Scale;
            public byte Index;
            public byte Base;
            public ushort Disp;
        }

        private enum OperandMode : byte
        {
            MemoryModeNoDisplacement = 0b00,
            MemoryMode8BitDisplacement = 0b01,
            MemoryMode16BitDisplacement = 0b10,
            RegisterMode = 0b11
        }
    }
}
