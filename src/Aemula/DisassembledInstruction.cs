namespace Aemula
{
    public readonly struct DisassembledInstruction
    {
        public readonly ushort Opcode;
        public readonly ushort AddressNumeric;
        public readonly string Address;
        public readonly byte InstructionSizeInBytes;
        public readonly string RawBytes;
        public readonly string Disassembly;
        public readonly ushort? Next0;
        public readonly ushort? Next1;

        public DisassembledInstruction(
            ushort opcode,
            ushort addressNumeric,
            string address, 
            byte instructionSizeInBytes, 
            string rawBytes, 
            string disassembly,
            ushort? next0,
            ushort? next1)
        {
            Opcode = opcode;
            AddressNumeric = addressNumeric;
            Address = address;
            InstructionSizeInBytes = instructionSizeInBytes;
            RawBytes = rawBytes;
            Disassembly = disassembly;
            Next0 = next0;
            Next1 = next1;
        }
    }
}