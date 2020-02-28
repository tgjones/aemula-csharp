namespace Aemula.Chips.Arm.Instructions
{
    internal abstract class Instruction
    {
        public readonly uint RawInstruction;
        public readonly Condition Condition;

        protected Instruction(uint rawInstruction)
        {
            RawInstruction = rawInstruction;
            Condition = (Condition)rawInstruction.GetBits(28, 31);
        }

        public abstract ExecutionResult Execute(Arm cpu);
    }

    public enum ExecutionResult
    {
        None,
        Branched,
    }
}
