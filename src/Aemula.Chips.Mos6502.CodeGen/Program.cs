using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aemula.Chips.Mos6502.CodeGen
{
    public static class Program
    {
        public static void Main()
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Diagnostics;");
            sb.AppendLine("");
            sb.AppendLine("namespace Aemula.Chips.Mos6502");
            sb.AppendLine("{");
            sb.AppendLine("    partial class Mos6502");
            sb.AppendLine("    {");
            sb.AppendLine("        private void ExecuteInstruction()");
            sb.AppendLine("        {");
            sb.AppendLine("            int tempInt32 = 0;");
            sb.AppendLine("            ");
            sb.AppendLine("            switch ((_ir << 3) | _tr)");
            sb.AppendLine("            {");

            var orderedInstructions = Instructions.OrderBy(x => x.Opcode);

            foreach (var instruction in orderedInstructions)
            {
                var instructionCode = InstructionCode.FromInstruction(instruction);

                sb.AppendLine($"                // {instructionCode.Comment}");

                for (var i = 0; i < instructionCode.Lines.Count; i++)
                {
                    var line = instructionCode.Lines[i];
                    if (line.Count == 0)
                    {
                        throw new InvalidOperationException();
                        //break;
                    }

                    sb.AppendLine($"                case (0x{instruction.Opcode:X2} << 3) | {i}:");
                    foreach (var segment in line)
                    {
                        sb.AppendLine($"                    {segment}");
                    }
                    sb.AppendLine("                    break;");
                }

                // Fill in remaining slots, so that our switch statement has an unbroken sequence of integers.
                for (var i = instructionCode.Lines.Count; i < 8; i++)
                {
                    sb.AppendLine($"                case (0x{instruction.Opcode:X2} << 3) | {i}:");
                    sb.AppendLine("                    Debug.Assert(false);");
                    sb.AppendLine("                    break;");
                }

                sb.AppendLine("");
            }

            sb.AppendLine("                default:");
            sb.AppendLine($"                    throw new InvalidOperationException($\"Unimplemented opcode 0x{{_ir:X2}} timing {{_tr}}\");");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(@"..\..\..\..\Aemula.Chips.Mos6502\Mos6502.generated.cs", sb.ToString());
        }

        private enum AddressingMode
        {
            None,
            Accumulator,
            Immediate,
            ZeroPage,
            ZeroPageX,
            ZeroPageY,
            Absolute,
            AbsoluteX,
            AbsoluteY,
            IndexedIndirectX,
            IndirectIndexedY,
            Indirect,
            JSR,
            Invalid,
        }

        private static string AddressingModeAsDisplayName(AddressingMode value) => value switch
        {
            AddressingMode.None => "",
            AddressingMode.Accumulator => "",
            AddressingMode.Immediate => "#",
            AddressingMode.ZeroPage => "zp",
            AddressingMode.ZeroPageX => "zp,X",
            AddressingMode.ZeroPageY => "zp,Y",
            AddressingMode.Absolute => "abs",
            AddressingMode.AbsoluteX => "abs,X",
            AddressingMode.AbsoluteY => "abs,Y",
            AddressingMode.IndexedIndirectX => "(zp,X)",
            AddressingMode.IndirectIndexedY => "(zp),Y",
            AddressingMode.Indirect => "ind",
            AddressingMode.JSR => "",
            AddressingMode.Invalid => "invalid",
            _ => throw new ArgumentOutOfRangeException(),
        };

        private enum MemoryAccess
        {
            None,
            Read,
            Write,
            ReadWrite,
        }

        private sealed class Instruction
        {
            public byte Opcode { get; }
            public string Mnemonic { get; }
            public AddressingMode AddressingMode { get; }
            public MemoryAccess MemoryAccess { get; }

            public Instruction(byte opcode, string mnemonic, AddressingMode addressingMode, MemoryAccess memoryAccess)
            {
                Opcode = opcode;
                Mnemonic = mnemonic;
                AddressingMode = addressingMode;
                MemoryAccess = memoryAccess;
            }
        }

        private static readonly Instruction[] Instructions =
        {
            // Interrupt, jump, subroutine
            new Instruction(0x00, "BRK", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0x20, "JSR", AddressingMode.JSR,              MemoryAccess.None),
            new Instruction(0x40, "RTI", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0x60, "RTS", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0x4C, "JMP", AddressingMode.Absolute,         MemoryAccess.None),
            new Instruction(0x6C, "JMP", AddressingMode.Indirect,         MemoryAccess.None),

            // Flags
            new Instruction(0x18, "CLC", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0x38, "SLC", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0x58, "CLI", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0x78, "SEI", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0xB8, "CLV", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0xD8, "CLD", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0xF8, "SED", AddressingMode.None,             MemoryAccess.None),

            // Branch
            new Instruction(0x10, "BPL", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0x30, "BMI", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0x50, "BVC", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0x70, "BVS", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0x90, "BCC", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0xB0, "BCS", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0xD0, "BNE", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0xF0, "BEQ", AddressingMode.Immediate,        MemoryAccess.None),

            // Stack
            new Instruction(0x08, "PHP", AddressingMode.None,             MemoryAccess.Write),
            new Instruction(0x28, "PLP", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0x48, "PHA", AddressingMode.None,             MemoryAccess.Write),
            new Instruction(0x68, "PLA", AddressingMode.None,             MemoryAccess.None),

            // Implied arithmetic
            new Instruction(0x88, "DEY", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0xCA, "DEX", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0xC8, "INY", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0xE8, "INX", AddressingMode.None,             MemoryAccess.None),

            // Transfer
            new Instruction(0x8A, "TXA", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0x9A, "TXS", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0x98, "TYA", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0xA8, "TAY", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0xAA, "TAX", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0xBA, "TSX", AddressingMode.None,             MemoryAccess.None),

            // ADC
            new Instruction(0x61, "ADC", AddressingMode.IndexedIndirectX, MemoryAccess.Read),
            new Instruction(0x65, "ADC", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0x6D, "ADC", AddressingMode.Absolute,         MemoryAccess.Read),
            new Instruction(0x69, "ADC", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0x71, "ADC", AddressingMode.IndirectIndexedY, MemoryAccess.Read),
            new Instruction(0x75, "ADC", AddressingMode.ZeroPageX,        MemoryAccess.Read),
            new Instruction(0x79, "ADC", AddressingMode.AbsoluteY,        MemoryAccess.Read),
            new Instruction(0x7D, "ADC", AddressingMode.AbsoluteX,        MemoryAccess.Read),

            // ANC (undocumented)
            new Instruction(0x0B, "ANC", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0x2B, "ANC", AddressingMode.Immediate,        MemoryAccess.None),

            // AND
            new Instruction(0x21, "AND", AddressingMode.IndexedIndirectX, MemoryAccess.Read),
            new Instruction(0x25, "AND", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0x29, "AND", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0x2D, "AND", AddressingMode.Absolute,         MemoryAccess.Read),
            new Instruction(0x31, "AND", AddressingMode.IndirectIndexedY, MemoryAccess.Read),
            new Instruction(0x35, "AND", AddressingMode.ZeroPageX,        MemoryAccess.Read),
            new Instruction(0x39, "AND", AddressingMode.AbsoluteY,        MemoryAccess.Read),
            new Instruction(0x3D, "AND", AddressingMode.AbsoluteX,        MemoryAccess.Read),

            // ANE (undocumented)
            new Instruction(0x8B, "ANE", AddressingMode.Immediate,        MemoryAccess.None),

            // ASL
            new Instruction(0x06, "ASL", AddressingMode.ZeroPage,         MemoryAccess.ReadWrite),
            new Instruction(0x16, "ASL", AddressingMode.ZeroPageX,        MemoryAccess.ReadWrite),
            new Instruction(0x0A, "ASL", AddressingMode.Accumulator,      MemoryAccess.None),
            new Instruction(0x0E, "ASL", AddressingMode.Absolute,         MemoryAccess.ReadWrite),
            new Instruction(0x1E, "ASL", AddressingMode.AbsoluteX,        MemoryAccess.ReadWrite),

            // ARR (undocumented, AND + ROR)
            new Instruction(0x6B, "ARR", AddressingMode.Immediate,        MemoryAccess.None),

            // ASR (undocumented, aka ALR, AND + LSR)
            new Instruction(0x4B, "ASR", AddressingMode.Immediate,        MemoryAccess.None),

            // BIT
            new Instruction(0x24, "BIT", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0x2C, "BIT", AddressingMode.Absolute,         MemoryAccess.Read),

            // CMP
            new Instruction(0xC1, "CMP", AddressingMode.IndexedIndirectX, MemoryAccess.Read),
            new Instruction(0xC5, "CMP", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0xC9, "CMP", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0xCD, "CMP", AddressingMode.Absolute,         MemoryAccess.Read),
            new Instruction(0xD1, "CMP", AddressingMode.IndirectIndexedY, MemoryAccess.Read),
            new Instruction(0xD5, "CMP", AddressingMode.ZeroPageX,        MemoryAccess.Read),
            new Instruction(0xD9, "CMP", AddressingMode.AbsoluteY,        MemoryAccess.Read),
            new Instruction(0xDD, "CMP", AddressingMode.AbsoluteX,        MemoryAccess.Read),

            // CPX
            new Instruction(0xE0, "CPX", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0xE4, "CPX", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0xEC, "CPX", AddressingMode.Absolute,         MemoryAccess.Read),

            // CPY
            new Instruction(0xC0, "CPY", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0xC4, "CPY", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0xCC, "CPY", AddressingMode.Absolute,         MemoryAccess.Read),
    
            // DCP (undocumented)
            new Instruction(0xC3, "DCP", AddressingMode.IndexedIndirectX, MemoryAccess.ReadWrite),
            new Instruction(0xC7, "DCP", AddressingMode.ZeroPage,         MemoryAccess.ReadWrite),
            new Instruction(0xCF, "DCP", AddressingMode.Absolute,         MemoryAccess.ReadWrite),
            new Instruction(0xD3, "DCP", AddressingMode.IndirectIndexedY, MemoryAccess.ReadWrite),
            new Instruction(0xD7, "DCP", AddressingMode.ZeroPageX,        MemoryAccess.ReadWrite),
            new Instruction(0xDB, "DCP", AddressingMode.AbsoluteY,        MemoryAccess.ReadWrite),
            new Instruction(0xDF, "DCP", AddressingMode.AbsoluteX,        MemoryAccess.ReadWrite),

            // DEC
            new Instruction(0xC6, "DEC", AddressingMode.ZeroPage,         MemoryAccess.ReadWrite),
            new Instruction(0xCE, "DEC", AddressingMode.Absolute,         MemoryAccess.ReadWrite),
            new Instruction(0xD6, "DEC", AddressingMode.ZeroPageX,        MemoryAccess.ReadWrite),
            new Instruction(0xDE, "DEC", AddressingMode.AbsoluteX,        MemoryAccess.ReadWrite),

            // EOR
            new Instruction(0x41, "EOR", AddressingMode.IndexedIndirectX, MemoryAccess.Read),
            new Instruction(0x45, "EOR", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0x49, "EOR", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0x4D, "EOR", AddressingMode.Absolute,         MemoryAccess.Read),
            new Instruction(0x51, "EOR", AddressingMode.IndirectIndexedY, MemoryAccess.Read),
            new Instruction(0x55, "EOR", AddressingMode.ZeroPageX,        MemoryAccess.Read),
            new Instruction(0x59, "EOR", AddressingMode.AbsoluteY,        MemoryAccess.Read),
            new Instruction(0x5D, "EOR", AddressingMode.AbsoluteX,        MemoryAccess.Read),

            // INC
            new Instruction(0xE6, "INC", AddressingMode.ZeroPage,         MemoryAccess.ReadWrite),
            new Instruction(0xEE, "INC", AddressingMode.Absolute,         MemoryAccess.ReadWrite),
            new Instruction(0xF6, "INC", AddressingMode.ZeroPageX,        MemoryAccess.ReadWrite),
            new Instruction(0xFE, "INC", AddressingMode.AbsoluteX,        MemoryAccess.ReadWrite),

            // ISB (undocumented, aka ISC)
            new Instruction(0xE3, "ISB", AddressingMode.IndexedIndirectX, MemoryAccess.ReadWrite),
            new Instruction(0xE7, "ISB", AddressingMode.ZeroPage,         MemoryAccess.ReadWrite),
            new Instruction(0xEF, "ISB", AddressingMode.Absolute,         MemoryAccess.ReadWrite),
            new Instruction(0xF3, "ISB", AddressingMode.IndirectIndexedY, MemoryAccess.ReadWrite),
            new Instruction(0xF7, "ISB", AddressingMode.ZeroPageX,        MemoryAccess.ReadWrite),
            new Instruction(0xFB, "ISB", AddressingMode.AbsoluteY,        MemoryAccess.ReadWrite),
            new Instruction(0xFF, "ISB", AddressingMode.AbsoluteX,        MemoryAccess.ReadWrite),

            // JAM (undocumented, aka KIL)
            new Instruction(0x02, "JAM", AddressingMode.Invalid,          MemoryAccess.None),
            new Instruction(0x12, "JAM", AddressingMode.Invalid,          MemoryAccess.None),
            new Instruction(0x22, "JAM", AddressingMode.Invalid,          MemoryAccess.None),
            new Instruction(0x32, "JAM", AddressingMode.Invalid,          MemoryAccess.None),
            new Instruction(0x42, "JAM", AddressingMode.Invalid,          MemoryAccess.None),
            new Instruction(0x52, "JAM", AddressingMode.Invalid,          MemoryAccess.None),
            new Instruction(0x62, "JAM", AddressingMode.Invalid,          MemoryAccess.None),
            new Instruction(0x72, "JAM", AddressingMode.Invalid,          MemoryAccess.None),
            new Instruction(0x92, "JAM", AddressingMode.Invalid,          MemoryAccess.None),
            new Instruction(0xB2, "JAM", AddressingMode.Invalid,          MemoryAccess.None),
            new Instruction(0xD2, "JAM", AddressingMode.Invalid,          MemoryAccess.None),
            new Instruction(0xF2, "JAM", AddressingMode.Invalid,          MemoryAccess.None),

            // LAS (undocumented)
            new Instruction(0xBB, "LAS", AddressingMode.AbsoluteY,        MemoryAccess.Read),

            // LAX (undocumented, LDA + LAX)
            new Instruction(0xA3, "LAX", AddressingMode.IndexedIndirectX, MemoryAccess.Read),
            new Instruction(0xA7, "LAX", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0xAF, "LAX", AddressingMode.Absolute,         MemoryAccess.Read),
            new Instruction(0xB3, "LAX", AddressingMode.IndirectIndexedY, MemoryAccess.Read),
            new Instruction(0xB7, "LAX", AddressingMode.ZeroPageY,        MemoryAccess.Read),
            new Instruction(0xBF, "LAX", AddressingMode.AbsoluteY,        MemoryAccess.Read),

            // LDA
            new Instruction(0xA1, "LDA", AddressingMode.IndexedIndirectX, MemoryAccess.Read),
            new Instruction(0xA5, "LDA", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0xA9, "LDA", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0xAD, "LDA", AddressingMode.Absolute,         MemoryAccess.Read),
            new Instruction(0xB1, "LDA", AddressingMode.IndirectIndexedY, MemoryAccess.Read),
            new Instruction(0xB5, "LDA", AddressingMode.ZeroPageX,        MemoryAccess.Read),
            new Instruction(0xB9, "LDA", AddressingMode.AbsoluteY,        MemoryAccess.Read),
            new Instruction(0xBD, "LDA", AddressingMode.AbsoluteX,        MemoryAccess.Read),

            // LDX
            new Instruction(0xA2, "LDX", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0xA6, "LDX", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0xAE, "LDX", AddressingMode.Absolute,         MemoryAccess.Read),
            new Instruction(0xB6, "LDX", AddressingMode.ZeroPageY,        MemoryAccess.Read),
            new Instruction(0xBE, "LDX", AddressingMode.AbsoluteY,        MemoryAccess.Read),

            // LDY
            new Instruction(0xA0, "LDY", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0xA4, "LDY", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0xAC, "LDY", AddressingMode.Absolute,         MemoryAccess.Read),
            new Instruction(0xB4, "LDY", AddressingMode.ZeroPageX,        MemoryAccess.Read),
            new Instruction(0xBC, "LDY", AddressingMode.AbsoluteX,        MemoryAccess.Read),

            // LSR
            new Instruction(0x46, "LSR", AddressingMode.ZeroPage,         MemoryAccess.ReadWrite),
            new Instruction(0x4A, "LSR", AddressingMode.Accumulator,      MemoryAccess.None),
            new Instruction(0x4E, "LSR", AddressingMode.Absolute,         MemoryAccess.ReadWrite),
            new Instruction(0x56, "LSR", AddressingMode.ZeroPageX,        MemoryAccess.ReadWrite),
            new Instruction(0x5E, "LSR", AddressingMode.AbsoluteX,        MemoryAccess.ReadWrite),

            // LXA (undocumented)
            new Instruction(0xAB, "LXA", AddressingMode.Immediate,        MemoryAccess.None),

            // NOP
            new Instruction(0x04, "NOP", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0x0C, "NOP", AddressingMode.Absolute,         MemoryAccess.Read),
            new Instruction(0x14, "NOP", AddressingMode.ZeroPageX,        MemoryAccess.Read),
            new Instruction(0x1A, "NOP", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0x1C, "NOP", AddressingMode.AbsoluteX,        MemoryAccess.Read),
            new Instruction(0x34, "NOP", AddressingMode.ZeroPageX,        MemoryAccess.Read),
            new Instruction(0x3A, "NOP", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0x3C, "NOP", AddressingMode.AbsoluteX,        MemoryAccess.Read),
            new Instruction(0x44, "NOP", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0x54, "NOP", AddressingMode.ZeroPageX,        MemoryAccess.Read),
            new Instruction(0x5A, "NOP", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0x5C, "NOP", AddressingMode.AbsoluteX,        MemoryAccess.Read),
            new Instruction(0x64, "NOP", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0x74, "NOP", AddressingMode.ZeroPageX,        MemoryAccess.Read),
            new Instruction(0x7A, "NOP", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0x7C, "NOP", AddressingMode.AbsoluteX,        MemoryAccess.Read),
            new Instruction(0x80, "NOP", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0x82, "NOP", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0x89, "NOP", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0xC2, "NOP", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0xD4, "NOP", AddressingMode.ZeroPageX,        MemoryAccess.Read),
            new Instruction(0xDA, "NOP", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0xDC, "NOP", AddressingMode.AbsoluteX,        MemoryAccess.Read),
            new Instruction(0xE2, "NOP", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0xEA, "NOP", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0xF4, "NOP", AddressingMode.ZeroPageX,        MemoryAccess.Read),
            new Instruction(0xFA, "NOP", AddressingMode.None,             MemoryAccess.None),
            new Instruction(0xFC, "NOP", AddressingMode.AbsoluteX,        MemoryAccess.Read),
    
            // ORA
            new Instruction(0x01, "ORA", AddressingMode.IndexedIndirectX, MemoryAccess.Read),
            new Instruction(0x05, "ORA", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0x09, "ORA", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0x0D, "ORA", AddressingMode.Absolute,         MemoryAccess.Read),
            new Instruction(0x11, "ORA", AddressingMode.IndirectIndexedY, MemoryAccess.Read),
            new Instruction(0x15, "ORA", AddressingMode.ZeroPageX,        MemoryAccess.Read),
            new Instruction(0x19, "ORA", AddressingMode.AbsoluteY,        MemoryAccess.Read),
            new Instruction(0x1D, "ORA", AddressingMode.AbsoluteX,        MemoryAccess.Read),

            // RLA (undocumented, ROL + AND)
            new Instruction(0x23, "RLA", AddressingMode.IndexedIndirectX, MemoryAccess.ReadWrite),
            new Instruction(0x27, "RLA", AddressingMode.ZeroPage,         MemoryAccess.ReadWrite),
            new Instruction(0x2F, "RLA", AddressingMode.Absolute,         MemoryAccess.ReadWrite),
            new Instruction(0x33, "RLA", AddressingMode.IndirectIndexedY, MemoryAccess.ReadWrite),
            new Instruction(0x37, "RLA", AddressingMode.ZeroPageX,        MemoryAccess.ReadWrite),
            new Instruction(0x3B, "RLA", AddressingMode.AbsoluteY,        MemoryAccess.ReadWrite),
            new Instruction(0x3F, "RLA", AddressingMode.AbsoluteX,        MemoryAccess.ReadWrite),

            // ROL
            new Instruction(0x26, "ROL", AddressingMode.ZeroPage,         MemoryAccess.ReadWrite),
            new Instruction(0x2A, "ROL", AddressingMode.Accumulator,      MemoryAccess.None),
            new Instruction(0x2E, "ROL", AddressingMode.Absolute,         MemoryAccess.ReadWrite),
            new Instruction(0x36, "ROL", AddressingMode.ZeroPageX,        MemoryAccess.ReadWrite),
            new Instruction(0x3E, "ROL", AddressingMode.AbsoluteX,        MemoryAccess.ReadWrite),

            // ROR
            new Instruction(0x66, "ROR", AddressingMode.ZeroPage,         MemoryAccess.ReadWrite),
            new Instruction(0x6A, "ROR", AddressingMode.Accumulator,      MemoryAccess.None),
            new Instruction(0x6E, "ROR", AddressingMode.Absolute,         MemoryAccess.ReadWrite),
            new Instruction(0x76, "ROR", AddressingMode.ZeroPageX,        MemoryAccess.ReadWrite),
            new Instruction(0x7E, "ROR", AddressingMode.AbsoluteX,        MemoryAccess.ReadWrite),

            // RRA (undocumented, ROR + ADC)
            new Instruction(0x63, "RRA", AddressingMode.IndexedIndirectX, MemoryAccess.ReadWrite),
            new Instruction(0x67, "RRA", AddressingMode.ZeroPage,         MemoryAccess.ReadWrite),
            new Instruction(0x6F, "RRA", AddressingMode.Absolute,         MemoryAccess.ReadWrite),
            new Instruction(0x73, "RRA", AddressingMode.IndirectIndexedY, MemoryAccess.ReadWrite),
            new Instruction(0x77, "RRA", AddressingMode.ZeroPageX,        MemoryAccess.ReadWrite),
            new Instruction(0x7B, "RRA", AddressingMode.AbsoluteY,        MemoryAccess.ReadWrite),
            new Instruction(0x7F, "RRA", AddressingMode.AbsoluteX,        MemoryAccess.ReadWrite),

            // SAX (undocumented)
            new Instruction(0x83, "SAX", AddressingMode.IndexedIndirectX, MemoryAccess.Write),
            new Instruction(0x87, "SAX", AddressingMode.ZeroPage,         MemoryAccess.Write),
            new Instruction(0x8F, "SAX", AddressingMode.Absolute,         MemoryAccess.Write),
            new Instruction(0x97, "SAX", AddressingMode.ZeroPageY,        MemoryAccess.Write),

            // SBC
            new Instruction(0xE1, "SBC", AddressingMode.IndexedIndirectX, MemoryAccess.Read),
            new Instruction(0xE5, "SBC", AddressingMode.ZeroPage,         MemoryAccess.Read),
            new Instruction(0xE9, "SBC", AddressingMode.Immediate,        MemoryAccess.None),
            new Instruction(0xEB, "SBC", AddressingMode.Immediate,        MemoryAccess.None), // Undocumented
            new Instruction(0xED, "SBC", AddressingMode.Absolute,         MemoryAccess.Read),
            new Instruction(0xF1, "SBC", AddressingMode.IndirectIndexedY, MemoryAccess.Read),
            new Instruction(0xF5, "SBC", AddressingMode.ZeroPageX,        MemoryAccess.Read),
            new Instruction(0xF9, "SBC", AddressingMode.AbsoluteY,        MemoryAccess.Read),
            new Instruction(0xFD, "SBC", AddressingMode.AbsoluteX,        MemoryAccess.Read),

            // SBX (undocumented)
            new Instruction(0xCB, "SBX", AddressingMode.Immediate,        MemoryAccess.None),

            // SHA (undocumented)
            new Instruction(0x93, "SHA", AddressingMode.IndirectIndexedY, MemoryAccess.Write),
            new Instruction(0x9F, "SHA", AddressingMode.AbsoluteY,        MemoryAccess.Write),

            // SHS (undocumented)
            new Instruction(0x9B, "SHS", AddressingMode.AbsoluteY,        MemoryAccess.Write),

            // SHX (undocumented)
            new Instruction(0x9E, "SHX", AddressingMode.AbsoluteY,        MemoryAccess.Write),

            // SHY (undocumented)
            new Instruction(0x9C, "SHY", AddressingMode.AbsoluteX,        MemoryAccess.Write),

            // SLO (undocumented, ASL + ORA)
            new Instruction(0x03, "SLO", AddressingMode.IndexedIndirectX, MemoryAccess.ReadWrite),
            new Instruction(0x07, "SLO", AddressingMode.ZeroPage,         MemoryAccess.ReadWrite),
            new Instruction(0x0F, "SLO", AddressingMode.Absolute,         MemoryAccess.ReadWrite),
            new Instruction(0x13, "SLO", AddressingMode.IndirectIndexedY, MemoryAccess.ReadWrite),
            new Instruction(0x17, "SLO", AddressingMode.ZeroPageX,        MemoryAccess.ReadWrite),
            new Instruction(0x1B, "SLO", AddressingMode.AbsoluteY,        MemoryAccess.ReadWrite),
            new Instruction(0x1F, "SLO", AddressingMode.AbsoluteX,        MemoryAccess.ReadWrite),

            // SRE (undocumented, LSR + EOR)
            new Instruction(0x43, "SRE", AddressingMode.IndexedIndirectX, MemoryAccess.ReadWrite),
            new Instruction(0x47, "SRE", AddressingMode.ZeroPage,         MemoryAccess.ReadWrite),
            new Instruction(0x4F, "SRE", AddressingMode.Absolute,         MemoryAccess.ReadWrite),
            new Instruction(0x53, "SRE", AddressingMode.IndirectIndexedY, MemoryAccess.ReadWrite),
            new Instruction(0x57, "SRE", AddressingMode.ZeroPageX,        MemoryAccess.ReadWrite),
            new Instruction(0x5B, "SRE", AddressingMode.AbsoluteY,        MemoryAccess.ReadWrite),
            new Instruction(0x5F, "SRE", AddressingMode.AbsoluteX,        MemoryAccess.ReadWrite),

            // STA
            new Instruction(0x81, "STA", AddressingMode.IndexedIndirectX, MemoryAccess.Write),
            new Instruction(0x85, "STA", AddressingMode.ZeroPage,         MemoryAccess.Write),
            new Instruction(0x8D, "STA", AddressingMode.Absolute,         MemoryAccess.Write),
            new Instruction(0x91, "STA", AddressingMode.IndirectIndexedY, MemoryAccess.Write),
            new Instruction(0x95, "STA", AddressingMode.ZeroPageX,        MemoryAccess.Write),
            new Instruction(0x99, "STA", AddressingMode.AbsoluteY,        MemoryAccess.Write),
            new Instruction(0x9D, "STA", AddressingMode.AbsoluteX,        MemoryAccess.Write),

            // STX
            new Instruction(0x86, "STX", AddressingMode.ZeroPage,         MemoryAccess.Write),
            new Instruction(0x8E, "STX", AddressingMode.Absolute,         MemoryAccess.Write),
            new Instruction(0x96, "STX", AddressingMode.ZeroPageY,        MemoryAccess.Write),

            // STY
            new Instruction(0x84, "STY", AddressingMode.ZeroPage,         MemoryAccess.Write),
            new Instruction(0x8C, "STY", AddressingMode.Absolute,         MemoryAccess.Write),
            new Instruction(0x94, "STY", AddressingMode.ZeroPageX,        MemoryAccess.Write),
        };

        private sealed class InstructionCodeBuilder
        {
            private readonly List<List<string>> _lines = new List<List<string>>();

            public string Description { get; private set; }
            public List<List<string>> Lines => _lines;

            public void Add(params string[] text)
            {
                _lines.Add(text.ToList());
            }

            public void ModifyPrevious(params string[] text)
            {
                var numLines = _lines.Count;
                if (_lines[numLines - 1] == null)
                {
                    _lines[numLines - 1] = new List<string>();
                }
                if (_lines[numLines - 1].Count == 1 && _lines[numLines - 1][0] == "")
                {
                    _lines[numLines - 1].Clear();
                }
                _lines[numLines - 1].AddRange(text);
            }

            public void EncodeOperation(string mnemonic, AddressingMode addressingMode)
            {
                string GetMethodName() => mnemonic.Substring(0, 1) + mnemonic.Substring(1).ToLowerInvariant();

                switch (mnemonic)
                {
                    // Special cases
                    case "BRK":
                        Add("if ((_brkFlags & (BrkFlags.Irq | BrkFlags.Nmi)) == 0)",
                            "{",
                            "    PC += 1;",
                            "}",
                            "Address.Hi = 0x01;",
                            "Address.Lo = SP--;",
                            "_data = PC.Hi;",
                            "_rw = (_brkFlags & BrkFlags.Reset) != 0;");
                        Add("Address.Lo = SP--;",
                            "_data = PC.Lo;",
                            "_rw = (_brkFlags & BrkFlags.Reset) != 0;");
                        Add("Address.Lo = SP--;",
                            "_data = P.AsByte(_brkFlags == BrkFlags.None);",
                            "_ad.Hi = 0xFF;",
                            "if ((_brkFlags & BrkFlags.Reset) != 0)",
                            "{",
                            "    _ad.Lo = 0xFC;",
                            "}",
                            "else",
                            "{",
                            "    _rw = false;",
                            "    _ad.Lo = (_brkFlags & BrkFlags.Nmi) != 0",
                            "        ? (byte)0xFA",
                            "        : (byte)0xFE;",
                            "}");
                        Add("Address = _ad;", "_ad.Lo += 1;", "P.I = true;", "_brkFlags = BrkFlags.None;");
                        Add("Address.Lo = _ad.Lo;", "_ad.Lo = _data;");
                        Add("PC.Hi = _data;", "PC.Lo = _ad.Lo;");
                        break;

                    case "JMP":
                        ModifyPrevious("PC = Address;");
                        break;

                    case "JSR":
                        // Read low byte of target address.
                        Add("Address = PC++;");
                        // Put SP on address bus.
                        Add("Address.Hi = 0x01;", "Address.Lo = SP;", "_ad.Lo = _data;");
                        // Write PC high byte to stack.
                        Add("Address.Lo = SP--;", "_data = PC.Hi;", "_rw = false;");
                        // Write PC low byte to stack.
                        Add("Address.Lo = SP--;", "_data = PC.Lo;", "_rw = false;");
                        // Read high byte of target address.
                        Add("Address = PC;");
                        Add("PC.Hi = _data;", "PC.Lo = _ad.Lo;");
                        break;

                    case "PLA":
                        Add("Address.Hi = 0x01;",
                            "Address.Lo = SP++;");
                        Add("Address.Lo = SP;");
                        Add("A = P.SetZeroNegativeFlags(_data);");
                        break;

                    case "PLP":
                        Add("Address.Hi = 0x01;",
                            "Address.Lo = SP++;");
                        Add("Address.Lo = SP;");
                        Add("P.SetFromByte(P.SetZeroNegativeFlags(_data));");
                        break;

                    case "RTI":
                        Add("Address.Hi = 0x01;", "Address.Lo = SP++;");
                        Add("Address.Lo = SP++;");
                        Add("Address.Lo = SP++;", "P.SetFromByte(_data);");
                        Add("Address.Lo = SP;", "_ad.Lo = _data;");
                        Add("PC.Hi = _data;", "PC.Lo = _ad.Lo;");
                        break;

                    case "RTS":
                        Add("Address.Hi = 0x01;", "Address.Lo = SP++;");
                        Add("Address.Lo = SP++;");
                        Add("Address.Lo = SP;", "_ad.Lo = _data;");
                        Add("PC.Hi = _data;", "PC.Lo = _ad.Lo;", "Address = PC++;");
                        Add("");
                        break;

                    case "CLC":
                        Add("P.C = false;");
                        break;

                    case "CLD":
                        Add("P.D = false;");
                        break;

                    case "CLI":
                        Add("P.I = false;");
                        break;

                    case "CLV":
                        Add("P.V = false;");
                        break;

                    case "SED":
                        Add("P.D = true;");
                        break;

                    case "SEI":
                        Add("P.I = true;");
                        break;

                    case "SLC":
                        Add("P.C = true;");
                        break;

                    case "DEX":
                        Description = "Decrement X Register";
                        Add("X = P.SetZeroNegativeFlags((byte)(X - 1));");
                        break;

                    case "DEY":
                        Description = "Decrement Y Register";
                        Add("Y = P.SetZeroNegativeFlags((byte)(Y - 1));");
                        break;

                    case "INX":
                        Description = "Increment X Register";
                        Add("X = P.SetZeroNegativeFlags((byte)(X + 1));");
                        break;

                    case "INY":
                        Description = "Increment Y Register";
                        Add("Y = P.SetZeroNegativeFlags((byte)(Y + 1));");
                        break;

                    case "PHA":
                        Add("Address.Hi = 0x01;",
                            "Address.Lo = SP--;",
                            "_data = A;",
                            "_rw = false;");
                        break;

                    case "PHP":
                        Add("Address.Hi = 0x01;",
                            "Address.Lo = SP--;",
                            "_data = P.AsByte(true);",
                            "_rw = false;");
                        break;

                    case "TAX":
                        Description = "Transfer Accumulator to X";
                        Add("X = P.SetZeroNegativeFlags(A);");
                        break;

                    case "TAY":
                        Description = "Transfer Accumulator to Y";
                        Add("Y = P.SetZeroNegativeFlags(A);");
                        break;

                    case "TSX":
                        Description = "Transfer Stack Pointer to X";
                        Add("X = P.SetZeroNegativeFlags(SP);");
                        break;

                    case "TXA":
                        Description = "Transfer X to Accumulator";
                        Add("A = P.SetZeroNegativeFlags(X);");
                        break;

                    case "TXS":
                        Description = "Transfer X to Stack Pointer";
                        Add("SP = X;");
                        break;

                    case "TYA":
                        Description = "Transfer Y to Accumulator";
                        Add("A = P.SetZeroNegativeFlags(Y);");
                        break;

                    case "STA":
                        Description = "Store Accumulator";
                        ModifyPrevious("_data = A;", "_rw = false;");
                        break;

                    case "STX":
                        Description = "Store X Register";
                        ModifyPrevious("_data = X;", "_rw = false;");
                        break;

                    case "STY":
                        Description = "Store Y Register";
                        ModifyPrevious("_data = Y;", "_rw = false;");
                        break;

                    case "SAX":
                        Description = "Store Accumulator and X (undocumented)";
                        ModifyPrevious("_data = (byte)(A & X);", "_rw = false;");
                        break;

                    case "SHA":
                        ModifyPrevious(OpsSha);
                        break;

                    case "SHS":
                        ModifyPrevious(new [] { "SP = (byte)(A & X);" }.Concat(OpsSha).ToArray());
                        break;

                    case "SHX":
                        ModifyPrevious("_data = (byte)(X & (Address.Hi + 1));", "_rw = false;");
                        break;

                    case "SHY":
                        ModifyPrevious("_data = (byte)(Y & (Address.Hi + 1));", "_rw = false;");
                        break;

                    case "LAX":
                        Description = "Load A and X Registers (undocumented)";
                        Add("A = P.SetZeroNegativeFlags(_data);", "X = P.SetZeroNegativeFlags(_data);");
                        break;

                    case "LDA":
                        Description = "Load Accumulator";
                        Add("A = P.SetZeroNegativeFlags(_data);");
                        break;

                    case "LDX":
                        Description = "Load X Register";
                        Add("X = P.SetZeroNegativeFlags(_data);");
                        break;

                    case "LDY":
                        Description = "Load Y Register";
                        Add("Y = P.SetZeroNegativeFlags(_data);");
                        break;

                    case "AND":
                        Description = "Logical AND";
                        Add(OpsAnd);
                        break;

                    case "EOR":
                        Description = "Exclusive OR";
                        Add(OpsEor);
                        break;

                    case "ORA":
                        Description = "Logical Inclusive OR";
                        Add(OpsOra);
                        break;

                    case "BIT":
                        Description = "Bit Test";
                        Add("P.Z = (A & _data) == 0;",
                            "P.V = (_data & 0x40) == 0x40;",
                            "P.N = (_data & 0x80) == 0x80;");
                        break;

                    case "CMP":
                        Description = "Compare";
                        Add(OpsCmp);
                        break;

                    case "CPX":
                        Description = "Compare X Register";
                        Add(GetOpsCmp("X"));
                        break;

                    case "CPY":
                        Description = "Compare Y Register";
                        Add(GetOpsCmp("Y"));
                        break;

                    case "LXA":
                        Add("A = (byte)((A | 0xEE) & _data);",
                            "X = A;",
                            "P.SetZeroNegativeFlags(A);");
                        break;

                    case "SBX":
                        Add("tempInt32 = (A & X) - _data;",
                            "X = (byte)tempInt32;",
                            "P.C = tempInt32 >= 0;",
                            "P.SetZeroNegativeFlags(X);");
                        break;

                    case "LAS":
                        Add("A = (byte)(_data & SP);",
                            "X = A;",
                            "SP = A;",
                            "P.SetZeroNegativeFlags(A);");
                        break;

                    case "ANC":
                        Add("A &= _data;",
                            "P.SetZeroNegativeFlags(A);",
                            "P.C = (A & 0x80) != 0;");
                        break;

                    case "ANE":
                        Add("A = (byte)((A | 0xEE) & X & _data);",
                            "P.SetZeroNegativeFlags(A);");
                        break;

                    case "ASR":
                        Add(OpsAnd.Concat(OpsLsra).ToArray());
                        break;

                    case "ADC":
                    case "SBC":
                    case "ARR":
                        Add($"{GetMethodName()}();");
                        break;

                    case "ROL" when addressingMode == AddressingMode.Accumulator:
                        Description = "Rotate Left";
                        Add("A = RolHelper(A);");
                        break;

                    case "ASL" when addressingMode == AddressingMode.Accumulator:
                        Description = "Arithmetic Shift Left";
                        Add("A = AslHelper(A);");
                        break;

                    case "LSR" when addressingMode == AddressingMode.Accumulator:
                        Description = "Logical Shift Right";
                        Add(OpsLsra);
                        break;

                    case "ROR" when addressingMode == AddressingMode.Accumulator:
                        Description = "Rotate Right";
                        Add(OpsRora);
                        break;

                    case "DEC":
                        Description = "Decrement Memory";
                        AddRmwCycle();
                        Add(OpsDec);
                        break;

                    case "DCP":
                        Description = "Decrement Memory then Compare (undocumented)";
                        AddRmwCycle();
                        Add(OpsDec.Concat(OpsCmp).ToArray());
                        break;

                    case "INC":
                        Description = "Increment Memory";
                        AddRmwCycle();
                        Add(OpsInc);
                        break;

                    case "ISB":
                        Description = "Increment Memory then Subtract (undocumented, also known as ISC)";
                        AddRmwCycle();
                        Add(OpsInc.Concat(new [] { "Sbc();" }).ToArray());
                        break;

                    case "ROL":
                        Description = "Rotate Left";
                        AddRmwCycle();
                        Add(OpsRol);
                        break;

                    case "RLA":
                        Description = "ROL + AND (undocumented)";
                        AddRmwCycle();
                        Add(OpsRol.Concat(OpsAnd).ToArray());
                        break;

                    case "ASL":
                        Description = "Arithmetic Shift Left";
                        AddRmwCycle();
                        Add(OpsAsl);
                        break;

                    case "SLO":
                        Description = "ASL + ORA (undocumented)";
                        AddRmwCycle();
                        Add(OpsAsl.Concat(OpsOra).ToArray());
                        break;

                    case "LSR":
                        Description = "Logical Shift Right";
                        AddRmwCycle();
                        Add(OpsLsr);
                        break;

                    case "SRE":
                        Description = "LSR + EOR (undocumented)";
                        AddRmwCycle();
                        Add(OpsLsr.Concat(OpsEor).ToArray());
                        break;

                    case "RRA":
                        Description = "ROR + ADC (undocumented)";
                        AddRmwCycle();
                        Add(OpsRor.Concat(OpsAdc).ToArray());
                        break;

                    case "ROR":
                        AddRmwCycle();
                        Add(OpsRor);
                        break;

                    // Branch
                    case "BCC":
                        EncodeBranch("C", false);
                        break;
                    case "BCS":
                        EncodeBranch("C", true);
                        break;
                    case "BEQ":
                        EncodeBranch("Z", true);
                        break;
                    case "BMI":
                        EncodeBranch("N", true);
                        break;
                    case "BNE":
                        EncodeBranch("Z", false);
                        break;
                    case "BPL":
                        EncodeBranch("N", false);
                        break;
                    case "BVC":
                        EncodeBranch("V", false);
                        break;
                    case "BVS":
                        EncodeBranch("V", true);
                        break;

                    // Invalid
                    case "JAM":
                        break;

                    case "NOP":
                        Add("");
                        break;

                    default:
                        throw new InvalidOperationException($"Unexpected mnemonic {mnemonic}");
                }
            }

            private void EncodeBranch(string register, bool value)
            {
                Add("Address = PC;",
                    "_ad = PC + (sbyte)_data;",
                    $"if (P.{register} != {value.ToString().ToLowerInvariant()})",
                    "{",
                    "    FetchNextInstruction();",
                    "}");

                // Executed if branch was taken.
                Add("Address.Lo = _ad.Lo;",
                    "if (_ad.Hi == PC.Hi)",
                    "{",
                    "    PC = _ad;",
                    "    FetchNextInstruction();",
                    "}");

                // Only executed if page was crossed.
                Add("PC = _ad;");
            }

            private void AddRmwCycle()
            {
                Add("_ad.Lo = _data;", "_rw = false;");
            }

            private static string[] GetOpsCmp(string register) => new[]
            {
                $"P.SetZeroNegativeFlags((byte)({register} - _data));",
                $"P.C = {register} >= _data;"
            };

            private static readonly string[] OpsAdc =
            {
                "Adc();"
            };

            private static readonly string[] OpsAnd =
            {
                "And();"
            };

            private static readonly string[] OpsAsl =
            {
                "_data = AslHelper(_ad.Lo);",
                "_rw = false;"
            };

            private static readonly string[] OpsCmp = GetOpsCmp("A");

            private static readonly string[] OpsDec =
            {
                "_data = P.SetZeroNegativeFlags((byte)(_ad.Lo - 1));",
                "_rw = false;"
            };

            private static readonly string[] OpsEor =
            {
                "A = P.SetZeroNegativeFlags((byte)(A ^ _data));"
            };

            private static readonly string[] OpsInc =
            {
                "_data = P.SetZeroNegativeFlags((byte)(_ad.Lo + 1));",
                "_rw = false;"
            };

            private static readonly string[] OpsLsr =
            {
                "_data = LsrHelper(_ad.Lo);",
                "_rw = false;"
            };

            private static readonly string[] OpsLsra =
            {
                "A = LsrHelper(A);"
            };

            private static readonly string[] OpsOra =
            {
                "A = P.SetZeroNegativeFlags((byte)(A | _data));"
            };

            private static readonly string[] OpsRol =
            {
                "_data = RolHelper(_ad.Lo);",
                "_rw = false;"
            };

            private static readonly string[] OpsRor =
            {
                "_data = RorHelper(_ad.Lo);",
                "_rw = false;"
            };

            private static readonly string[] OpsRora =
            {
                "Rora();"
            };

            private static readonly string[] OpsSha =
            {
                "_data = (byte)(A & X & (Address.Hi + 1));",
                "_rw = false;"
            };

            public void EncodeAddressingMode(Instruction instruction)
            {
                switch (instruction.AddressingMode)
                {
                    case AddressingMode.None:
                    case AddressingMode.Accumulator:
                        Add("Address = PC;");
                        break;

                    case AddressingMode.Immediate:
                        Add("Address = PC++;");
                        break;

                    case AddressingMode.ZeroPage:
                        Add("Address = PC++;");
                        Add("Address = _data;");
                        break;

                    case AddressingMode.ZeroPageX:
                        Add("Address = PC++;");
                        Add("_ad = _data;", "Address = _ad;");
                        Add("Address = (byte)(_ad.Lo + X);");
                        break;

                    case AddressingMode.ZeroPageY:
                        Add("Address = PC++;");
                        Add("_ad = _data;", "Address = _ad;");
                        Add("Address = (byte)(_ad.Lo + Y);");
                        break;

                    case AddressingMode.Absolute:
                        Add("Address = PC++;");
                        Add("Address = PC++;", "_ad.Lo = _data;");
                        Add("Address.Hi = _data;", "Address.Lo = _ad.Lo;");
                        break;

                    case AddressingMode.AbsoluteX:
                        Add("Address = PC++;");
                        Add("Address = PC++;", "_ad.Lo = _data;");
                        Add("_ad.Hi = _data;", "Address.Hi = _ad.Hi;", "Address.Lo = (byte)(_ad.Lo + X);");
                        if (instruction.MemoryAccess == MemoryAccess.Read)
                        {
                            ModifyPrevious("IncrementTimingRegisterIfNoPageCrossing(X);");
                        }
                        Add("Address = _ad + X;");
                        break;

                    case AddressingMode.AbsoluteY:
                        Add("Address = PC++;");
                        Add("Address = PC++;", "_ad.Lo = _data;");
                        Add("_ad.Hi = _data;", "Address.Hi = _ad.Hi;", "Address.Lo = (byte)(_ad.Lo + Y);");
                        if (instruction.MemoryAccess == MemoryAccess.Read)
                        {
                            ModifyPrevious("IncrementTimingRegisterIfNoPageCrossing(Y);");
                        }
                        Add("Address = _ad + Y;");
                        break;

                    case AddressingMode.IndexedIndirectX:
                        Add("Address = PC++;");
                        Add("_ad = _data;", "Address = _ad;");
                        Add("_ad.Lo += X;", "Address.Lo = _ad.Lo;");
                        Add("Address.Lo = (byte)(_ad.Lo + 1);", "_ad.Lo = _data;");
                        Add("Address.Hi = _data;", "Address.Lo = _ad.Lo;");
                        break;

                    case AddressingMode.IndirectIndexedY:
                        Add("Address = PC++;");
                        Add("_ad = _data;", "Address = _ad;");
                        Add("Address.Lo = (byte)(_ad.Lo + 1);", "_ad.Lo = _data;");
                        Add("_ad.Hi = _data;", "Address.Hi = _ad.Hi;", "Address.Lo = (byte)(_ad.Lo + Y);");
                        if (instruction.MemoryAccess == MemoryAccess.Read)
                        {
                            ModifyPrevious("IncrementTimingRegisterIfNoPageCrossing(Y);");
                        }
                        Add("Address = _ad + Y;");
                        break;

                    case AddressingMode.Indirect:
                        Add("Address = PC++;");
                        Add("Address = PC++;", "_ad.Lo = _data;");
                        Add("_ad.Hi = _data;", "Address = _ad;");
                        Add("Address.Lo = (byte)(_ad.Lo + 1);", "_ad.Lo = _data;");
                        Add("Address.Hi = _data;", "Address.Lo = _ad.Lo;");
                        break;

                    case AddressingMode.JSR:
                        break;

                    case AddressingMode.Invalid:
                        Add("Address = PC;");
                        Add("Address.Hi = 0xFF;", "Address.Lo = 0xFF;", "_data = 0xFF;", "_ir--;");
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private sealed class InstructionCode
        {
            public static InstructionCode FromInstruction(Instruction instruction)
            {
                var comment = $"{instruction.Mnemonic} {AddressingModeAsDisplayName(instruction.AddressingMode)}";

                var codeBuilder = new InstructionCodeBuilder();
                codeBuilder.EncodeAddressingMode(instruction);

                codeBuilder.EncodeOperation(instruction.Mnemonic, instruction.AddressingMode);

                switch (instruction.MemoryAccess)
                {
                    case MemoryAccess.None:
                    case MemoryAccess.Read:
                        codeBuilder.ModifyPrevious("FetchNextInstruction();");
                        break;

                    default:
                        codeBuilder.Add("FetchNextInstruction();");
                        break;
                }

                if (codeBuilder.Description != null)
                {
                    comment += " - " + codeBuilder.Description;
                }

                return new InstructionCode(comment, codeBuilder.Lines);
            }

            public string Comment { get; }
            public List<List<string>> Lines { get; }

            private InstructionCode(string comment, List<List<string>> lines)
            {
                Comment = comment;
                Lines = lines;
            }
        }
    }
}
