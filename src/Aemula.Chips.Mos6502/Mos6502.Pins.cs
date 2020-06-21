namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        public SplitUInt16 Address;

        [Pin(PinDirection.Bidirectional)]
        private byte _data;

        [Pin(PinDirection.In)]
        private bool _rdy;

        [Pin(PinDirection.In)]
        private bool _irq;

        [Pin(PinDirection.In)]
        private bool _nmi;

        [Pin(PinDirection.Out)]
        private bool _sync;

        [Pin(PinDirection.In)]
        [Handle(ChangeType.Always)]
        private bool _res;

        [Pin(PinDirection.In)]
        [Handle(ChangeType.TransitionedLoToHi, ChangeType.TransitionedHiToLo)]
        private bool _phi0;

        /// <summary>
        /// When PHI1 is high, external devices can read from the address bus or data bus.
        /// </summary>
        [Pin(PinDirection.Out)]
        private bool _phi1;

        /// <summary>
        /// When PHI2 is high, external devices can write to the data bus.
        /// </summary>
        [Pin(PinDirection.Out)]
        private bool _phi2;

        /// <summary>
        /// Read/write (read = true, write = false)
        /// </summary>
        [Pin(PinDirection.Out)]
        private bool _rw;
    }
}
