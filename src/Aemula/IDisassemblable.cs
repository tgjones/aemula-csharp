using System;

namespace Aemula
{
    public interface IDisassemblable
    {
        Action OnFetchingInstruction { get; set; }

        DisassembledInstruction Disassemble(ulong address);
    }

    public sealed class DisassembledInstruction
    {
        public readonly ulong Address;
        public readonly byte InstructionSizeInBytes;
        public readonly string RawBytes;
        public readonly string Disassembly;

        public DisassembledInstruction(ulong address, byte instructionSizeInBytes, string rawBytes, string disassembly)
        {
            Address = address;
            InstructionSizeInBytes = instructionSizeInBytes;
            RawBytes = rawBytes;
            Disassembly = disassembly;
        }
    }
}