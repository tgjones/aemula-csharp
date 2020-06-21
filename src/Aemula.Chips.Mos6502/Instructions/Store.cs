namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        /// <summary>
        /// SAX - Store Accumulator and X (undocumented)
        /// </summary>
        private void Sax()
        {
            _data = (byte)(A & X);
            _rw = false;
        }

        /// <summary>
        /// STA - Store Accumulator
        /// </summary>
        private void Sta()
        {
            _data = A;
            _rw = false;
        }

        /// <summary>
        /// STX - Store X Register
        /// </summary>
        private void Stx()
        {
            _data = X;
            _rw = false;
        }

        /// <summary>
        /// STY - Store Y Register
        /// </summary>
        private void Sty()
        {
            _data = Y;
            _rw = false;
        }
    }
}
