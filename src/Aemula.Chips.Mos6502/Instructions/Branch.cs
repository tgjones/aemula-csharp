namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void Branch0(bool flag, bool value)
        {
            Address = PC;
            _ad = PC + (sbyte)_data;
            // If branch was not taken, fetch next instruction.
            if (flag != value)
            {
                FetchNextInstruction();
            }
        }

        private void Branch0Bcc() => Branch0(P.C, false);

        private void Branch0Bcs() => Branch0(P.C, true);

        private void Branch0Beq() => Branch0(P.Z, true);

        private void Branch0Bmi() => Branch0(P.N, true);

        private void Branch0Bne() => Branch0(P.Z, false);

        private void Branch0Bpl() => Branch0(P.N, false);

        private void Branch0Bvc() => Branch0(P.V, false);

        private void Branch0Bvs() => Branch0(P.V, true);

        /// <summary>
        /// Executed if branch was taken.
        /// </summary>
        private void Branch1()
        {
            Address.Lo = _ad.Lo;

            // Are we branching to the same page?
            if (_ad.Hi == PC.Hi)
            {
                PC = _ad;
                FetchNextInstruction();
            }
        }

        /// <summary>
        /// Only executed if page was crossed.
        /// </summary>
        private void Branch2()
        {
            PC = _ad;
        }

        private void Jmp()
        {
            PC = Address;
        }
    }
}
