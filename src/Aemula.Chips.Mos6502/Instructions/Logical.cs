namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        /// <summary>
        /// AND - Logical AND
        /// </summary>
        private void And()
        {
            A = P.SetZeroNegativeFlags((byte)(A & _data));
        }

        /// <summary>
        /// EOR - Exclusive OR
        /// </summary>
        private void Eor()
        {
            A = P.SetZeroNegativeFlags((byte)(A ^ _data));
        }

        /// <summary>
        /// ORA - Logical Inclusive OR
        /// </summary>
        private void Ora()
        {
            A = P.SetZeroNegativeFlags((byte)(A | _data));
        }
    }
}
