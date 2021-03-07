using System;
using System.Collections.Generic;

namespace Aemula
{
    public interface IDisassemblable
    {
        event EventHandler ProgramLoaded;

        ushort LastPC { get; }

        SortedDictionary<ushort, DisassembledInstruction> Disassemble();
    }

    public readonly struct DisassembledInstruction
    {
        public readonly byte Opcode;
        public readonly ushort AddressNumeric;
        public readonly string Address;
        public readonly byte InstructionSizeInBytes;
        public readonly string RawBytes;
        public readonly string Disassembly;
        public readonly ushort? Next0;
        public readonly ushort? Next1;

        public DisassembledInstruction(
            byte opcode,
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