namespace Aemula.Chips.Arm.Instructions
{
    /// <summary>
    /// A8.8.132 POP (ARM).
    /// Loads multiple registers from the stack.
    /// </summary>
    internal abstract class Pop : Instruction
    {
        protected Pop(uint rawInstruction)
            : base(rawInstruction)
        {

        }

        public override ExecutionResult Execute(Arm cpu)
        {
            if (cpu.ConditionPassed(Condition))
            {
                EncodingSpecificOperations(cpu, out var registers);

                var address = cpu.SP;

                for (var i = (byte)0; i <= 15; i++)
                {
                    if (registers.GetBit(i) == 1)
                    {
                        cpu.R[i] = cpu.ReadMemory(address);

                        address += 4;
                    }
                }

                if (registers.GetBitAsBool(13))
                {
                    cpu.SP = BuiltInFunctions.Unknown;
                }
                else
                {
                    cpu.SP += 4u * BuiltInFunctions.BitCount(registers);
                }
            }

            return ExecutionResult.None;
        }

        protected abstract void EncodingSpecificOperations(
            Arm cpu,
            out uint registers);

        public sealed class A1 : Pop
        {
            public readonly uint RegisterList;

            public A1(uint rawInstruction)
                : base(rawInstruction)
            {
                RegisterList = rawInstruction & 0xFFFF;
            }

            protected override void EncodingSpecificOperations(
                Arm cpu,
                out uint registers)
            {
                registers = RegisterList;
            }
        }
    }
}
