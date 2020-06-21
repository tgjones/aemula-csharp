namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void Pha()
        {
            Address.Hi = 0x01;
            Address.Lo = SP;
            SP -= 1;
            _data = A;
            _rw = false;
        }

        private void Php()
        {
            Address.Hi = 0x01;
            Address.Lo = SP;
            SP -= 1;
            _data = P.AsByte(true);
            _rw = false;
        }

        private void Pla0()
        {
            Address.Hi = 0x01;
            Address.Lo = SP;
            SP += 1;
        }

        private void Pla1()
        {
            Address.Hi = 0x01;
            Address.Lo = SP;
        }

        private void Pla2()
        {
            A = P.SetZeroNegativeFlags(_data);
        }

        private void Plp0()
        {
            Address.Hi = 0x01;
            Address.Lo = SP;
            SP += 1;
        }

        private void Plp1()
        {
            Address.Hi = 0x01;
            Address.Lo = SP;
        }

        private void Plp2()
        {
            P.SetFromByte(P.SetZeroNegativeFlags(_data));
        }
    }
}
