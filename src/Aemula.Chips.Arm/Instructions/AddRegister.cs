using System;

namespace Aemula.Chips.Arm.Instructions
{
    /// <summary>
    /// A8.8.7 ADD (register, ARM).
    /// Adds a register value to a register value, 
    /// and writes the result to the destination register.
    /// </summary>
    internal abstract class AddRegister : Instruction
    {
        protected AddRegister(uint rawInstruction)
            : base(rawInstruction)
        {

        }

        public override ExecutionResult Execute(Arm cpu)
        {
            if (cpu.ConditionPassed(Condition))
            {
                EncodingSpecificOperations(cpu, out var d, out var n, out var m, out var setFlags, out var shiftType, out var shift);

                var shifted = BuiltInFunctions.Shift(cpu.R[m], shiftType, (int)shift, cpu.Apsr.C);
                var (result, carry, overflow) = BuiltInFunctions.AddWithCarry(cpu.R[n], shifted, false);

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
                        cpu.Apsr.C = carry;
                        cpu.Apsr.V = overflow;
                    }
                }
            }

            return ExecutionResult.None;
        }

        protected abstract void EncodingSpecificOperations(
            Arm cpu,
            out uint d,
            out uint n,
            out uint m,
            out bool setFlags,
            out ShiftRotateType shiftType,
            out uint shift);

        public sealed class A1 : AddRegister
        {
            public readonly bool S;
            public readonly uint Rn;
            public readonly uint Rd;
            public readonly uint Imm5;
            public readonly uint Type;
            public readonly uint Rm;

            public A1(uint rawInstruction)
                : base(rawInstruction)
            {
                S = rawInstruction.GetBitAsBool(20);
                Rn = rawInstruction.GetBits(16, 19);
                Rd = rawInstruction.GetBits(12, 15);
                Imm5 = rawInstruction.GetBits(7, 11);
                Type = rawInstruction.GetBits(5, 6);
                Rm = rawInstruction.GetBits(0, 3);
            }

            protected override void EncodingSpecificOperations(
                Arm cpu,
                out uint d,
                out uint n,
                out uint m,
                out bool setFlags,
                out ShiftRotateType shiftType,
                out uint shift)
            {
                if (Rd == 0b1111 && S)
                {
                    // SEE  SEE SUBS PC, LR and related instructions.
                    throw new NotImplementedException();
                }

                d = Rd;
                n = Rn;
                m = Rm;
                setFlags = S;
                (shiftType, shift) = BuiltInFunctions.DecodeImmShift(Type, Imm5);
            }
        }
    }
}
