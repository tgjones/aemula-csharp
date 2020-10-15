namespace Aemula.Chips.Mos6502
{
    public struct Mos6502Pins
    {
        public SplitUInt16 Address;
        public byte Data;
        public bool Rdy;
        public bool Irq;
        public bool Nmi;
        public bool Sync;
        public bool Res;
        public bool RW;
    }
}
