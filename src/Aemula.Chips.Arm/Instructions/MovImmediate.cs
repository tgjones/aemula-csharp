using System;

namespace Aemula.Chips.Arm.Instructions
{
    /// <summary>
    /// A8.8.102 MOV (immediate).
    /// Writes an immediate value to the destination register.
    /// </summary>
    internal abstract class MovImmediate : Instruction
    {
        protected MovImmediate(uint rawInstruction)
            : base(rawInstruction)
        {

        }

        public override ExecutionResult Execute(Arm cpu)
        {
            if (cpu.ConditionPassed(Condition))
            {
                EncodingSpecificOperations(cpu, out var imm32, out var d, out var setFlags, out var carry);

                var result = imm32;
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
                    }
                }
            }

            return ExecutionResult.None;
        }

        protected abstract void EncodingSpecificOperations(
            Arm cpu,
            out uint imm32, 
            out uint d, 
            out bool setFlags, 
            out bool carry);

        public sealed class A1 : MovImmediate
        {
            public readonly bool S;
            public readonly uint Rd;
            public readonly uint Imm12;

            public A1(uint rawInstruction)
                : base(rawInstruction)
            {
                S = ((rawInstruction >> 20) & 0b1) == 1;
                Rd = (rawInstruction >> 12) & 0b1111;
                Imm12 = rawInstruction & 0xFFF;
            }

            protected override void EncodingSpecificOperations(
                Arm cpu,
                out uint imm32,
                out uint d,
                out bool setFlags,
                out bool carry)
            {
                if (Rd == 15 && S)
                {
                    // SEE SUBS PC, LR and related instructions.
                    throw new NotImplementedException();
                }

                d = Rd;
                setFlags = S;
                (imm32, carry) = BuiltInFunctions.ARMExpandImm_C(Imm12, cpu.Apsr.C);
            }
        }
    }
}
