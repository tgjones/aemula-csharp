namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        /// <summary>
        /// LAX - Load A and X Registers (undocumented)
        /// </summary>
        private void Lax()
        {
            Lda();
            Ldx();
        }

        /// <summary>
        /// LDA - Load Accumulator
        /// </summary>
        private void Lda()
        {
            A = P.SetZeroNegativeFlags(_data);
        }

        /// <summary>
        /// LDX - Load X Register
        /// </summary>
        private void Ldx()
        {
            X = P.SetZeroNegativeFlags(_data);
        }

        /// <summary>
        /// LDY - Load Y Register
        /// </summary>
        private void Ldy()
        {
            Y = P.SetZeroNegativeFlags(_data);
        }
    }
}
