namespace Aemula.Chips.INS8154
{
    public sealed class INS8154
    {
        private readonly byte[] _ram = new byte[128];

        [Pin(PinDirection.In)]
        private byte _address;

        [Pin(PinDirection.Bidirectional)]
        private byte _data;

        /// <summary>
        /// Chip Select 0. Must be low to select the chip.
        /// </summary>
        [Pin(PinDirection.In)]
        private bool _cs0;

        /// <summary>
        /// Chip Select 1. Must be high to select the chip.
        /// </summary>
        [Pin(PinDirection.In)]
        private bool _cs1;

        /// <summary>
        /// Read strobe.
        /// </summary>
        [Pin(PinDirection.In)]
        [Handle(ChangeType.TransitionedHiToLo)]
        private bool _nrds = true;

        /// <summary>
        /// Write strobe.
        /// </summary>
        [Pin(PinDirection.In)]
        private bool _nwds = true;

        private bool IsChipActive() => !_cs0 && _cs1;

        private void OnNrdsTransitionedHiToLo()
        {
            if (!IsChipActive())
            {
                return;
            }

            _data = _ram[_address];
        }

        private void OnNwdsTransitionedHiToLo()
        {
            if (!IsChipActive())
            {
                return;
            }

            _ram[_address] = _data;
        }
    }
}
