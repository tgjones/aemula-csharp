namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private byte AslHelper(byte value)
        {
            P.C = (value & 0x80) == 0x80;
            return P.SetZeroNegativeFlags((byte)(value << 1));
        }

        /// <summary>
        /// ASL - Arithmetic Shift Left
        /// </summary>
        private void Asl()
        {
            _data = AslHelper(_ad.Lo);
            _rw = false;
        }

        /// <summary>
        /// ASL - Arithmetic Shift Left (accumulator addressing)
        /// </summary>
        private void Asla()
        {
            A = AslHelper(A);
        }

        private byte LsrHelper(byte value)
        {
            P.C = (value & 0x1) == 0x1;
            return P.SetZeroNegativeFlags((byte)(value >> 1));
        }

        /// <summary>
        /// LSR - Logical Shift Right
        /// </summary>
        private void Lsr()
        {
            _data = LsrHelper(_ad.Lo);
            _rw = false;
        }

        /// <summary>
        /// LSR - Logical Shift Right (accumulator addressing)
        /// </summary>
        private void Lsra()
        {
            A = LsrHelper(A);
        }

        private byte RolHelper(byte value)
        {
            var temp = (byte)(P.C ? 1 : 0);
            P.C = (value & 0x80) == 0x80;
            return P.SetZeroNegativeFlags((byte)((value << 1) | temp));
        }

        private byte RorHelper(byte value)
        {
            var temp = (byte)((P.C ? 1 : 0) << 7);
            P.C = (value & 0x1) == 0x1;
            return P.SetZeroNegativeFlags((byte)((value >> 1) | temp));
        }

        /// <summary>
        /// ROR - Rotate Right
        /// </summary>
        private void Ror()
        {
            _data = RorHelper(_ad.Lo);
            _rw = false;
        }

        /// <summary>
        /// ROR - Rotate Right (accumulator addressing)
        /// </summary>
        private void Rora()
        {
            A = RorHelper(A);
        }
    }
}
