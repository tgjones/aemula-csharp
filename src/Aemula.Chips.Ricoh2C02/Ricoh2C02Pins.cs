namespace Aemula.Chips.Ricoh2C02
{
    public struct Ricoh2C02Pins
    {
        public bool CpuRW;
        public byte CpuAddress;
        public byte CpuData;

        /// <summary>
        /// True if the PPU is reading from VRAM, false if it's writing.
        /// </summary>
        public bool PpuRW;

        // In hardware, these two overlap in pins AD0..AD7
        public ushort PpuAddress;
        public byte PpuData;
    }
}