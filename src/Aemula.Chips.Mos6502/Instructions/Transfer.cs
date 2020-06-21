namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        /// <summary>
        /// TAX - Transfer Accumulator to X
        /// </summary>
        private void Tax()
        {
            X = P.SetZeroNegativeFlags(A);
        }

        /// <summary>
        /// TAY - Transfer Accumulator to Y
        /// </summary>
        private void Tay()
        {
            Y = P.SetZeroNegativeFlags(A);
        }

        /// <summary>
        /// TSX - Transfer Stack Pointer to X
        /// </summary>
        private void Tsx()
        {
            X = P.SetZeroNegativeFlags(SP);
        }

        /// <summary>
        /// TXA - Transfer X to Accumulator
        /// </summary>
        private void Txa()
        {
            A = P.SetZeroNegativeFlags(X);
        }

        /// <summary>
        /// TXS - Transfer X to Stack Pointer
        /// </summary>
        private void Txs()
        {
            SP = X;
        }

        /// <summary>
        /// TYA - Transfer Y to Accumulator
        /// </summary>
        private void Tya()
        {
            A = P.SetZeroNegativeFlags(Y);
        }
    }
}
