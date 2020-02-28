namespace Aemula.Chips.Arm.Instructions
{
    /// <summary>
    /// A8.8.38 CMP (register).
    /// Subtracts a register value from a register value
    /// </summary>
    internal abstract class CmpRegister : Instruction
    {
        protected CmpRegister(uint rawInstruction)
            : base(rawInstruction)
        {

        }

        public override ExecutionResult Execute(Arm cpu)
        {
            if (cpu.ConditionPassed(Condition))
            {
                EncodingSpecificOperations(cpu, out var n, out var m, out var shiftType, out var shift);

                var shifted = BuiltInFunctions.Shift(cpu.R[m], shiftType, (int)shift, cpu.Apsr.C);

                var (result, carry, overflow) = BuiltInFunctions.AddWithCarry(cpu.R[n], ~shifted, true);

                cpu.Apsr.N = result.GetBit(31) == 1;
                cpu.Apsr.Z = BuiltInFunctions.IsZero(result);
                cpu.Apsr.C = carry;
                cpu.Apsr.V = overflow;
            }

            return ExecutionResult.None;
        }

        protected abstract void EncodingSpecificOperations(
            Arm cpu,
            out uint n,
            out uint m,
            out ShiftRotateType shiftType,
            out uint shift);

        public sealed class A1 : CmpRegister
        {
            public readonly uint Rn;
            public readonly uint Imm5;
            public readonly uint Type;
            public readonly uint Rm;

            public A1(uint rawInstruction)
                : base(rawInstruction)
            {
                Rn = (rawInstruction >> 16) & 0b1111;
                Type = (rawInstruction >> 5) & 0b11;
                Imm5 = (rawInstruction >> 7) & 0b11111;
                Rm = rawInstruction & 0b1111;
            }

            protected override void EncodingSpecificOperations(
                Arm cpu,
                out uint n,
                out uint m,
                out ShiftRotateType shiftType,
                out uint shift)
            {
                n = Rn;
                m = Rm;
                (shiftType, shift) = BuiltInFunctions.DecodeImmShift(Type, Imm5);
            }
        }
    }
}
