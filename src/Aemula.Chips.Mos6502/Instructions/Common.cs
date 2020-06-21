namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void RmwCycle()
        {
            _ad.Lo = _data;
            _rw = false;
        }

        /// <summary>
        /// NOP
        /// </summary>
        private void Nop() { }
    }
}
