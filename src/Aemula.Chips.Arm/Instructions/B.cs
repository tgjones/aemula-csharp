namespace Aemula.Chips.Arm.Instructions
{
    /// <summary>
    /// A8.8.18 B.
    /// Causes a branch to a target address.
    /// </summary>
    internal abstract class B : Instruction
    {
        protected B(uint rawInstruction)
            : base(rawInstruction)
        {

        }

        public override ExecutionResult Execute(Arm cpu)
        {
            if (cpu.ConditionPassed(Condition))
            {
                EncodingSpecificOperations(cpu, out var imm32);

                cpu.BranchWritePC(cpu.PC + imm32);

                return ExecutionResult.Branched;
            }

            return ExecutionResult.None;
        }

        protected abstract void EncodingSpecificOperations(
            Arm cpu,
            out uint imm32);

        public sealed class A1 : B
        {
            public readonly uint Imm24;

            public A1(uint rawInstruction)
                : base(rawInstruction)
            {
                Imm24 = rawInstruction & 0xFFFFFF;
            }

            protected override void EncodingSpecificOperations(
                Arm cpu,
                out uint imm32)
            {
                imm32 = BuiltInFunctions.SignExtend(Imm24 << 2, 26);
            }
        }
    }
}
