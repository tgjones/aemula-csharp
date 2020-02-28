namespace Aemula.Chips.Mos6502
{
    partial class Mos6502 : IDisassemblable
    {
        public DisassembledInstruction Disassemble(ulong address)
        {
            var localPC = (ushort)address;

            var opcode = ReadMemory(localPC);

            if (_instructionImplementations.TryGetValue(opcode, out var instructionImplementation))
            {
                var disassembly = instructionImplementation.GetDisassembly(this, localPC);
                var rawBytes = opcode.ToString("X2");
                for (var j = 1; j < instructionImplementation.InstructionSizeInBytes; j++)
                {
                    rawBytes += $" {ReadMemory((ushort)(localPC + j)):X2}";
                }
                return new DisassembledInstruction(localPC, instructionImplementation.InstructionSizeInBytes, rawBytes, disassembly);
            }
            else
            {
                return new DisassembledInstruction(localPC, 1, opcode.ToString("X2"), "-");
            }
        }
    }
}