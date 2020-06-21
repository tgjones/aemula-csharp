namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void Brk0()
        {
            if ((_brkFlags & (BrkFlags.Irq | BrkFlags.Nmi)) != 0)
            {
                PC += 1;
            }
            Address.Hi = 0x01;
            Address.Lo = SP;
            SP -= 1;
            _data = PC.Hi;
            if ((_brkFlags & BrkFlags.Reset) == 0)
            {
                _rw = false;
            }
        }

        private void Brk1()
        {
            Address.Hi = 0x01;
            Address.Lo = SP;
            SP -= 1;
            _data = PC.Lo;
            if ((_brkFlags & BrkFlags.Reset) == 0)
            {
                _rw = false;
            }
        }

        private void Brk2()
        {
            Address.Hi = 0x01;
            Address.Lo = SP;
            SP -= 1;
            _data = P.AsByte(_brkFlags == BrkFlags.None);
            _ad.Hi = 0xFF;
            if ((_brkFlags & BrkFlags.Reset) != 0)
            {
                _ad.Lo = 0xFC;
            }
            else
            {
                _rw = false;
                if ((_brkFlags & BrkFlags.Nmi) != 0)
                {
                    _ad.Lo = 0xFA;
                }
                else
                {
                    _ad.Lo = 0xFE;
                }
            }
        }

        private void Brk3()
        {
            Address = _ad;
            _ad.Lo += 1;
            P.I = true;
            _brkFlags = BrkFlags.None;
        }

        private void Brk4()
        {
            Address.Lo = _ad.Lo;
            _ad.Lo = _data;
        }

        private void Brk5()
        {
            PC.Hi = _data;
            PC.Lo = _ad.Lo;
        }
    }
}
