namespace Aemula.Chips.Arm.Instructions
{
    /// <summary>
    /// A8.8.133 PUSH.
    /// Stores multiple registers to the stack.
    /// </summary>
    internal abstract class Push : Instruction
    {
        protected Push(uint rawInstruction)
            : base(rawInstruction)
        {

        }

        public override ExecutionResult Execute(Arm cpu)
        {
            if (cpu.ConditionPassed(Condition))
            {
                EncodingSpecificOperations(cpu, out var registers);

                var address = cpu.SP - 4u * BuiltInFunctions.BitCount(registers);

                for (var i = (byte)0; i <= 15; i++)
                {
                    if (registers.GetBit(i) == 1)
                    {
                        if (i == 13 && i != BuiltInFunctions.LowestSetBit(registers))
                        {
                            cpu.WriteMemory(address, BuiltInFunctions.Unknown);
                        }
                        else
                        {
                            cpu.WriteMemory(address, cpu.R[i]);
                        }

                        address += 4;
                    }
                }

                cpu.SP -= 4u * BuiltInFunctions.BitCount(registers);
            }

            return ExecutionResult.None;
        }

        protected abstract void EncodingSpecificOperations(
            Arm cpu,
            out uint registers);

        public sealed class A1 : Push
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
