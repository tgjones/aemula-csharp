namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void Clc() => P.C = false;

        private void Cld() => P.D = false;

        private void Cli() => P.I = false;

        private void Clv() => P.V = false;

        private void Sed() => P.D = true;

        private void Sei() => P.I = true;

        private void Slc() => P.C = true;
    }
}
