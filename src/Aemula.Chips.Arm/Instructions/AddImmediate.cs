using System;

namespace Aemula.Chips.Arm.Instructions
{
    /// <summary>
    /// A8.8.5 ADD (immediate, ARM).
    /// Adds an immediate value to a register value, 
    /// and writes the result to the destination register.
    /// </summary>
    internal abstract class AddImmediate : Instruction
    {
        protected AddImmediate(uint rawInstruction)
            : base(rawInstruction)
        {

        }

        public override ExecutionResult Execute(Arm cpu)
        {
            if (cpu.ConditionPassed(Condition))
            {
                EncodingSpecificOperations(cpu, out var imm32, out var d, out var n, out var setFlags);

                var (result, carry, overflow) = BuiltInFunctions.AddWithCarry(cpu.R[n], imm32, false);

                if (d == 15)
                {
                    cpu.ALUWritePC(result);
                }
                else
                {
                    cpu.R[d] = result;
                    if (setFlags)
                    {
                        cpu.Apsr.N = result.GetBit(31) == 1;
                        cpu.Apsr.Z = BuiltInFunctions.IsZero(result);
                        cpu.Apsr.C = carry;
                        cpu.Apsr.V = overflow;
                    }
                }
            }

            return ExecutionResult.None;
        }

        protected abstract void EncodingSpecificOperations(
            Arm cpu,
            out uint imm32,
            out uint d,
            out uint n,
            out bool setFlags);

        public sealed class A1 : AddImmediate
        {
            public readonly bool S;
            public readonly uint Rn;
            public readonly uint Rd;
            public readonly uint Imm12;

            public A1(uint rawInstruction, uint rn)
                : base(rawInstruction)
            {
                S = ((rawInstruction >> 20) & 0b1) == 1;
                Rn = rn;
                Rd = (rawInstruction >> 12) & 0b1111;
                Imm12 = rawInstruction & 0xFFF;
            }

            protected override void EncodingSpecificOperations(
                Arm cpu,
                out uint imm32,
                out uint d,
                out uint n,
                out bool setFlags)
            {
                if (Rn == 0b1111 && !S)
                {
                    // SEE ADR.
                    throw new NotImplementedException();
                }

                if (Rd == 0b1111 && S)
                {
                    // SEE SUBS PC, LR and related instructions.
                    throw new NotImplementedException();
                }

                d = Rd;
                n = Rn;
                setFlags = S;
                imm32 = BuiltInFunctions.ARMExpandImm(Imm12);
            }
        }
    }
}
