namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        /// <summary>
        /// Read low byte of target address.
        /// </summary>
        private void Jsr0()
        {
            Address = PC;
            PC += 1;
        }

        /// <summary>
        /// Put SP on address bus.
        /// </summary>
        private void Jsr1()
        {
            Address.Hi = 0x01;
            Address.Lo = SP;
            _ad.Lo = _data;
        }

        /// <summary>
        /// Write PC high byte to stack.
        /// </summary>
        private void Jsr2()
        {
            Address.Hi = 0x01;
            Address.Lo = SP;
            SP -= 1;
            _data = PC.Hi;
            _rw = false;
        }

        /// <summary>
        /// Write PC low byte to stack.
        /// </summary>
        private void Jsr3()
        {
            Address.Hi = 0x01;
            Address.Lo = SP;
            SP -= 1;
            _data = PC.Lo;
            _rw = false;
        }

        /// <summary>
        /// Read high byte of target address.
        /// </summary>
        private void Jsr4()
        {
            Address = PC;
        }

        private void Jsr5()
        {
            PC.Hi = _data;
            PC.Lo = _ad.Lo;
        }

        private void Rti0()
        {
            Address.Hi = 0x01;
            Address.Lo = SP;
            SP += 1;
        }

        private void Rti1()
        {
            Address.Lo = SP;
            SP += 1;
        }

        private void Rti2()
        {
            Address.Lo = SP;
            SP += 1;
            P.SetFromByte(_data);
        }

        private void Rti3()
        {
            Address.Lo = SP;
            _ad.Lo = _data;
        }

        private void Rti4()
        {
            PC.Hi = _data;
            PC.Lo = _ad.Lo;
        }

        private void Rts0()
        {
            Address.Hi = 0x01;
            Address.Lo = SP;
            SP += 1;
        }

        private void Rts1()
        {
            Address.Lo = SP;
            SP += 1;
        }

        private void Rts2()
        {
            Address.Lo = SP;
            _ad.Lo = _data;
        }

        private void Rts3()
        {
            PC.Hi = _data;
            PC.Lo = _ad.Lo;
            Address = PC;
            PC += 1;
        }
    }
}
