namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void DoAdcBinary(byte value)
        {
            var temp = (ushort)(A + value + (P.C ? 1 : 0));
            P.V = ((A ^ temp) & (value ^ temp) & 0x80) == 0x80;
            P.C = temp > 0xFF;
            A = (byte)temp;
            P.SetZeroNegativeFlags(A);
        }

        private void DoAdcDecimal(byte value)
        {
            var temp = (byte)((A + value + (P.C ? 1 : 0)) & 0xFF);
            P.Z = temp == 0;

            var ah = 0;
            var al = (A & 0xF) + (value & 0xF) + (P.C ? 1 : 0);
            if (al > 9)
            {
                al -= 10;
                al &= 0xF;
                ah = 1;
            }

            ah += (A >> 4) + (value >> 4);
            P.N = (ah & 8) == 8;
            P.V = ((A ^ value) & 0x80) == 0 && ((A ^ (ah << 4)) & 0x80) == 0x80;
            P.C = false;

            if (ah > 9)
            {
                P.C = true;
                ah -= 10;
                ah &= 0xF;
            }

            A = (byte)(((al & 0xF) | (ah << 4)) & 0xFF);
        }

        /// <summary>
        /// ADC - Add with Carry
        /// </summary>
        private void Adc()
        {
            if (!P.D || !_bcdEnabled)
            {
                DoAdcBinary(_data);
            }
            else
            {
                DoAdcDecimal(_data);
            }
        }

        private void DoSbcDecimal(byte value)
        {
            var carry = P.C ? 0 : 1;
            var al = (A & 0xF) - (value & 0xF) - carry;
            var ah = (A >> 4) - (value >> 4);

            if ((al & 0x10) == 0x10)
            {
                al = (al - 6) & 0xF;
                ah--;
            }

            if ((ah & 0x10) == 0x10)
            {
                ah = (ah - 6) & 0xF;
            }

            var result = A - value - carry;
            P.N = (result & 0x80) == 0x80;
            P.Z = (result & 0xFF) == 0;
            P.V = ((A ^ result) & (value ^ A) & 0x80) == 0x80;
            P.C = (result & 0x100) == 0;
            A = (byte)(al | (ah << 4));
        }

        /// <summary>
        /// SBC - Subtract with Carry
        /// </summary>
        private void Sbc()
        {
            if (!P.D || !_bcdEnabled)
            {
                DoAdcBinary((byte)~_data);
            }
            else
            {
                DoSbcDecimal(_data);
            }
        }

        /// <summary>
        /// DEC - Decrement Memory
        /// </summary>
        private void Dec()
        {
            _data = P.SetZeroNegativeFlags((byte)(_ad.Lo - 1));
            _rw = false;
        }

        /// <summary>
        /// DEX - Decrement X Register
        /// </summary>
        private void Dex()
        {
            X = P.SetZeroNegativeFlags((byte)(X - 1));
        }

        /// <summary>
        /// DEY - Decrement Y Register
        /// </summary>
        private void Dey()
        {
            Y = P.SetZeroNegativeFlags((byte)(Y - 1));
        }

        /// <summary>
        /// INC - Increment Memory
        /// </summary>
        private void Inc()
        {
            _data = P.SetZeroNegativeFlags((byte)(_ad.Lo + 1));
            _rw = false;
        }

        /// <summary>
        /// INX - Increment X Register
        /// </summary>
        private void Inx()
        {
            X = P.SetZeroNegativeFlags((byte)(X + 1));
        }

        /// <summary>
        /// INY - Increment Y Register
        /// </summary>
        private void Iny()
        {
            Y = P.SetZeroNegativeFlags((byte)(Y + 1));
        }
    }
}
