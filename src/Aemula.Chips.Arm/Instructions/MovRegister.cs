using System;

namespace Aemula.Chips.Arm.Instructions
{
    /// <summary>
    /// A8.8.104 MOV (register, ARM).
    /// Copies a value from a register to the destination register.
    /// </summary>
    internal abstract class MovRegister : Instruction
    {
        protected MovRegister(uint rawInstruction)
            : base(rawInstruction)
        {

        }

        public override ExecutionResult Execute(Arm cpu)
        {
            if (cpu.ConditionPassed(Condition))
            {
                EncodingSpecificOperations(cpu, out var d, out var m, out var setFlags);

                var result = cpu.R[m];
                if (d == 15)
                {
                    cpu.ALUWritePC(result);
                }
                else
                {
                    cpu.R[d] = result;
                    if (setFlags)
                    {
                        cpu.Apsr.N = result.GetBitAsBool(31);
                        cpu.Apsr.Z = BuiltInFunctions.IsZero(result);
                    }
                }
            }

            return ExecutionResult.None;
        }

        protected abstract void EncodingSpecificOperations(
            Arm cpu,
            out uint d,
            out uint m,
            out bool setFlags);

        public sealed class A1 : MovRegister
        {
            public readonly bool S;
            public readonly uint Rd;
            public readonly uint Rm;

            public A1(uint rawInstruction)
                : base(rawInstruction)
            {
                S = rawInstruction.GetBitAsBool(20);
                Rd = rawInstruction.GetBits(12, 15);
                Rm = rawInstruction & 0b1111;
            }

            protected override void EncodingSpecificOperations(
                Arm cpu,
                out uint d,
                out uint m,
                out bool setFlags)
            {
                if (Rd == 15 && S)
                {
                    // SEE SUBS PC, LR and related instructions.
                    throw new NotImplementedException();
                }

                d = Rd;
                m = Rm;
                setFlags = S;
            }
        }
    }
}
