namespace Aemula.Chips.Arm.Instructions
{
    /// <summary>
    /// A8.8.27 BX.
    /// Causes a branch to a target address and instruction set.
    /// </summary>
    internal abstract class BX : Instruction
    {
        protected BX(uint rawInstruction)
            : base(rawInstruction)
        {

        }

        public override ExecutionResult Execute(Arm cpu)
        {
            if (cpu.ConditionPassed(Condition))
            {
                EncodingSpecificOperations(cpu, out var m);

                cpu.BXWritePC(cpu.R[m]);

                return ExecutionResult.Branched;
            }

            return ExecutionResult.None;
        }

        protected abstract void EncodingSpecificOperations(
            Arm cpu,
            out uint m);

        public sealed class A1 : BX
        {
            public readonly uint Rm;

            public A1(uint rawInstruction)
                : base(rawInstruction)
            {
                Rm = rawInstruction & 0b1111;
            }

            protected override void EncodingSpecificOperations(
                Arm cpu,
                out uint m)
            {
                m = Rm;
            }
        }
    }
}
