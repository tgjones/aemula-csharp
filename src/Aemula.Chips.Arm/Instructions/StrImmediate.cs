using System;

namespace Aemula.Chips.Arm.Instructions
{
    /// <summary>
    /// A8.8.204 STR (immediate, ARM).
    /// Stores a word from a register to memory.
    /// </summary>
    internal abstract class StrImmediate : Instruction
    {
        protected StrImmediate(uint rawInstruction)
            : base(rawInstruction)
        {

        }

        public override ExecutionResult Execute(Arm cpu)
        {
            if (cpu.ConditionPassed(Condition))
            {
                EncodingSpecificOperations(cpu, out var t, out var n, out var imm32, out var index, out var add, out var wback);

                var offsetAddress = add
                    ? cpu.R[n] + imm32
                    : cpu.R[n] - imm32;

                var address = index
                    ? offsetAddress
                    : cpu.R[n];

                cpu.WriteMemory(address, t == 15 ? cpu.PCStoreValue() : cpu.R[t]);
                
                if (wback)
                {
                    cpu.R[n] = offsetAddress;
                }
            }

            return ExecutionResult.None;
        }

        protected abstract void EncodingSpecificOperations(
            Arm cpu,
            out uint t,
            out uint n,
            out uint imm32,
            out bool index,
            out bool add,
            out bool wback);

        public sealed class A1 : StrImmediate
        {
            public readonly bool P;
            public readonly bool U;
            public readonly bool W;
            public readonly uint Rn;
            public readonly uint Rt;
            public readonly uint Imm12;

            public A1(uint rawInstruction, uint rn)
                : base(rawInstruction)
            {
                P = rawInstruction.GetBitAsBool(24);
                U = rawInstruction.GetBitAsBool(23);
                W = rawInstruction.GetBitAsBool(21);
                Rn = rn;
                Rt = rawInstruction.GetBits(12, 15);
                Imm12 = rawInstruction & 0xFFF;
            }

            protected override void EncodingSpecificOperations(
                Arm cpu,
                out uint t,
                out uint n,
                out uint imm32,
                out bool index,
                out bool add,
                out bool wback)
            {
                if (!P && W)
                {
                    // SEE STRT.
                    throw new NotImplementedException();
                }

                if (Rn == 0b1101 && P && !U && W && Imm12 == 0b100)
                {
                    // SEE PUSH.
                    throw new NotImplementedException();
                }

                t = Rt;
                n = Rn;
                imm32 = BuiltInFunctions.ZeroExtend(Imm12);
                index = P;
                add = U;
                wback = !P || W;

                if (wback && (n == 15 || n == t))
                {
                    // TODO: UNPREDICTABLE.
                }
            }
        }
    }
}
