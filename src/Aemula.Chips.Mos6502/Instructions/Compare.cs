namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        /// <summary>
        /// BIT - Bit Test
        /// </summary>
        private void Bit()
        {
            var value = _data;
            P.Z = (A & value) == 0;
            P.V = (value & 0x40) == 0x40;
            P.N = (value & 0x80) == 0x80;
        }

        private void Compare(byte register)
        {
            P.SetZeroNegativeFlags((byte)(register - _data));
            P.C = register >= _data;
        }

        /// <summary>
        /// CMP - Compare
        /// </summary>
        private void Cmp() => Compare(A);

        /// <summary>
        /// CPX - Compare X Register
        /// </summary>
        private void Cpx() => Compare(X);

        /// <summary>
        /// CPY - Compare Y Register
        /// </summary>
        private void Cpy() => Compare(Y);
    }
}
