namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void Anc()
        {
            A &= _data;
            P.SetZeroNegativeFlags(A);
            P.C = (A & 0x80) != 0;
        }

        private void Ane()
        {
            A = (byte)((A | 0xEE) & X & _data);
            P.SetZeroNegativeFlags(A);
        }

        private void Arr()
        {
            And();

            // http://www.6502.org/users/andre/petindex/local/64doc.txt
            if (_bcdEnabled && P.D)
            {
                // Do ROR.
                var a = (byte)(A >> 1);

                // Add carry flag to MSB.
                if (P.C)
                {
                    a |= 0x80;
                }

                // Set zero and negative flags as normal.
                P.SetZeroNegativeFlags(a);

                // The V flag will be set if the bit 6 of the accumulator changed its state
                // between the AND and the ROR, cleared otherwise.
                P.V = ((A ^ a) & 0x40) != 0;

                // Now it gets weird: if low nibble is greater than 5, increment it by 6,
                // but if it carries, don't add it to high nibble.
                if ((A & 0xF) >= 5)
                {
                    a = (byte)(((a + 6) & 0xF) | (a & 0xF0));
                }

                // If high nibble is greater than 5, increment it by 6,
                // and set the carry flag.
                if ((A & 0xF0) >= 0x50)
                {
                    a += 0x60;
                    P.C = true;
                }
                else
                {
                    P.C = false;
                }

                A = a;
            }
            else
            {
                Rora();

                // The C flag is copied from bit 6 of the result.
                P.C = (A & 0x40) != 0;

                // The V flag is the result of an XOR operation between bit 6 and bit 5 of the result.
                P.V = (((A & 0x40) >> 6) ^ ((A & 0x20) >> 5)) == 1;
            }
        }

        private void Asr()
        {
            And();
            Lsra();
        }

        /// <summary>
        /// DCP - Decrement Memory then Compare (undocumented)
        /// </summary>
        private void Dcp()
        {
            Dec();
            Cmp();
        }

        /// <summary>
        /// ISB (also known as ISC) - Increment Memory then Subtract (undocumented)
        /// </summary>
        private void Isb()
        {
            Inc();
            Sbc();
        }

        private void Las()
        {
            A = (byte)(_data & SP);
            X = A;
            SP = A;
            P.SetZeroNegativeFlags(A);
        }

        private void Lxa()
        {
            A = (byte)((A | 0xEE) & _data);
            X = A;
            P.SetZeroNegativeFlags(A);
        }

        /// <summary>
        /// RLA - ROL + AND (undocumented)
        /// </summary>
        private void Rla()
        {
            Rol();
            And();
        }

        /// <summary>
        /// RRA - ROR + ADC (undocumented)
        /// </summary>
        private void Rra()
        {
            Ror();
            Adc();
        }

        private void Sbx()
        {
            var temp = (A & X) - _data;
            X = (byte)temp;
            P.C = temp >= 0;
            P.SetZeroNegativeFlags(X);
        }

        private void Sha()
        {
            _data = (byte)(A & X & (Address.Hi + 1));
            _rw = false;
        }

        private void Shs()
        {
            SP = (byte)(A & X);
            Sha();
        }

        private void Shx()
        {
            _data = (byte)(X & (Address.Hi + 1));
            _rw = false;
        }

        private void Shy()
        {
            _data = (byte)(Y & (Address.Hi + 1));
            _rw = false;
        }

        /// <summary>
        /// SLO - ASL + ORA (undocumented)
        /// </summary>
        private void Slo()
        {
            Asl();
            Ora();
        }

        /// <summary>
        /// SRE - LSR + EOR (undocumented)
        /// </summary>
        private void Sre()
        {
            Lsr();
            Eor();
        }
    }
}
