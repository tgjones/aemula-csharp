using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Aemula.Chips.Mos6502.Generators
{
    [Generator]
    public class Mos6502InstructionGenerator : ISourceGenerator
    {
        public void Initialize(InitializationContext context)
        {
        }

        public void Execute(SourceGeneratorContext context)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("");
            sb.AppendLine("namespace Aemula.Chips.Mos6502");
            sb.AppendLine("{");
            sb.AppendLine("    partial class Mos6502");
            sb.AppendLine("    {");
            sb.AppendLine("        private void ExecuteInstruction()");
            sb.AppendLine("        {");
            sb.AppendLine("            switch ((_ir, _tr))");
            sb.AppendLine("            {");

            foreach (var instruction in Instructions)
            {
                var instructionCode = InstructionCode.FromInstruction(instruction);

                sb.AppendLine($"                // {instructionCode.Comment}");

                for (var i = 0; i < instructionCode.Lines.Length; i++)
                {
                    var line = instructionCode.Lines[i];
                    if (line == string.Empty)
                    {
                        break;
                    }

                    sb.AppendLine($"                case (0x{instruction.Opcode:X2}, {i}):");
                    sb.AppendLine($"                    {line}");
                    sb.AppendLine("                    break;");
                    sb.AppendLine("");
                }
            }

            sb.AppendLine("                default:");
            sb.AppendLine($"                    throw new InvalidOperationException($\"Unimplemented opcode 0x{{_ir:X2}} timing {{_tr}}\");");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);

            File.WriteAllText(@"C:\CodePersonal\Aemula\src\Aemula.Chips.Mos6502\obj\Test.cs", sb.ToString());

            context.AddSource("Mos6502.InstructionExecution.g.cs", sourceText);
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
            private readonly List<string> _lines = new List<string>();

            public void Add(string text)
            {
                _lines.Add(text);
            }

            public void ModifyPrevious(string text)
            {
                var numLines = _lines.Count;
                if (_lines[numLines - 1] != string.Empty)
                {
                    _lines[numLines - 1] += " ";
                }
                _lines[numLines - 1] += text;
            }

            public void EncodeOperation(string mnemonic, AddressingMode addressingMode)
            {
                string GetMethodName() => mnemonic.Substring(0, 1) + mnemonic.Substring(1).ToLowerInvariant();

                switch (mnemonic)
                {
                    // Special cases
                    case "BRK":
                        Add("Brk0();");
                        Add("Brk1();");
                        Add("Brk2();");
                        Add("Brk3();");
                        Add("Brk4();");
                        Add("Brk5();");
                            break;

                    case "JMP": 
                        ModifyPrevious("Jmp();");
                        break;

                    case "JSR":
                        Add("Jsr0();");
                        Add("Jsr1();");
                        Add("Jsr2();");
                        Add("Jsr3();");
                        Add("Jsr4();");
                        Add("Jsr5();");
                        break;

                    case "PLA":
                        Add("Pla0();");
                        Add("Pla1();");
                        Add("Pla2();");
                        break;

                    case "PLP":
                        Add("Plp0();");
                        Add("Plp1();");
                        Add("Plp2();");
                        break;

                    case "RTI":
                        Add("Rti0();");
                        Add("Rti1();");
                        Add("Rti2();");
                        Add("Rti3();");
                        Add("Rti4();");
                        break;

                    case "RTS":
                        Add("Rts0();");
                        Add("Rts1();");
                        Add("Rts2();");
                        Add("Rts3();");
                        Add("");
                        break;

                    case "CLC":
                    case "CLD":
                    case "CLI":
                    case "CLV":
                    case "SED":
                    case "SEI":
                    case "SLC":
                    case "DEX":
                    case "DEY":
                    case "INX":
                    case "INY":
                    case "TAX":
                    case "TAY":
                    case "TSX":
                    case "TXA":
                    case "TXS":
                    case "TYA":
                    case "PHA":
                    case "PHP":
                        Add($"{GetMethodName()}();");
                        break;

                    // Write memory
                    case "SAX":
                    case "SHA":
                    case "SHX":
                    case "SHY":
                    case "SHS":
                    case "STA":
                    case "STX":
                    case "STY":
                        ModifyPrevious($"{GetMethodName()}();");
                        break;

                    case "ADC":
                    case "AND":
                    case "BIT":
                    case "CMP":
                    case "CPX":
                    case "CPY":
                    case "EOR":
                    case "LAX":
                    case "LDA":
                    case "LDX":
                    case "LDY":
                    case "ORA":
                    case "SBC":
                    case "ANC":
                    case "ANE":
                    case "ARR":
                    case "ASR":
                    case "LAS":
                    case "LXA":
                    case "SBX":
                        Add($"{GetMethodName()}();");
                        break;

                    // Accumulator
                    case "ASL" when addressingMode == AddressingMode.Accumulator:
                    case "LSR" when addressingMode == AddressingMode.Accumulator:
                    case "ROL" when addressingMode == AddressingMode.Accumulator:
                    case "ROR" when addressingMode == AddressingMode.Accumulator:
                        Add($"{GetMethodName()}a();");
                        break;

                    // Read / modify / write memory
                    case "ASL":
                    case "DEC":
                    case "DCP":
                    case "INC":
                    case "ISB":
                    case "LSR":
                    case "RLA":
                    case "ROL":
                    case "ROR":
                    case "RRA":
                    case "SLO":
                    case "SRE":
                        Add("RmwCycle();");
                        Add($"{GetMethodName()}();");
                        break;

                    // Branch
                    case "BCC":
                    case "BCS":
                    case "BEQ":
                    case "BMI":
                    case "BNE":
                    case "BPL":
                    case "BVC":
                    case "BVS":
                        Add($"Branch0{GetMethodName()}();");
                        Add("Branch1();");
                        Add("Branch2();");
                        break;

                    // Invalid
                    case "JAM":
                        break;

                    case "NOP":
                        Add("Nop();");
                        break;

                    default:
                        throw new InvalidOperationException($"Unexpected mnemonic {mnemonic}");
                }
            }

            public void EncodeAddressingMode(Instruction instruction)
            {
                switch (instruction.AddressingMode)
                {
                    case AddressingMode.None:
                    case AddressingMode.Accumulator:
                        Add("AddressingModeNoneCycle0();");
                        break;

                    case AddressingMode.Immediate:
                        Add("AddressingModeImmediateCycle0();");
                        break;

                    case AddressingMode.ZeroPage:
                        Add("AddressingModeZeroPageCycle0();");
                        Add("AddressingModeZeroPageCycle1();");
                        break;

                    case AddressingMode.ZeroPageX:
                        Add("AddressingModeZeroPageIndexedCycle0();");
                        Add("AddressingModeZeroPageIndexedCycle1();");
                        Add("AddressingModeZeroPageXCycle2();");
                        break;

                    case AddressingMode.ZeroPageY:
                        Add("AddressingModeZeroPageIndexedCycle0();");
                        Add("AddressingModeZeroPageIndexedCycle1();");
                        Add("AddressingModeZeroPageYCycle2();");
                        break;

                    case AddressingMode.Absolute:
                        Add("AddressingModeAbsoluteCycle0();");
                        Add("AddressingModeAbsoluteCycle1();");
                        Add("AddressingModeAbsoluteCycle2();");
                        break;

                    case AddressingMode.AbsoluteX:
                        Add("AddressingModeAbsoluteIndexedCycle0();");
                        Add("AddressingModeAbsoluteIndexedCycle1();");
                        Add("AddressingModeAbsoluteIndexedCycle2(X);");
                        if (instruction.MemoryAccess == MemoryAccess.Read)
                        {
                            ModifyPrevious("AddressingModeAbsoluteIndexedCycle2Read(X);");
                        }
                        Add("AddressingModeAbsoluteIndexedCycle3(X);");
                        break;

                    case AddressingMode.AbsoluteY:
                        Add("AddressingModeAbsoluteIndexedCycle0();");
                        Add("AddressingModeAbsoluteIndexedCycle1();");
                        Add("AddressingModeAbsoluteIndexedCycle2(Y);");
                        if (instruction.MemoryAccess == MemoryAccess.Read)
                        {
                            ModifyPrevious("AddressingModeAbsoluteIndexedCycle2Read(Y);");
                        }
                        Add("AddressingModeAbsoluteIndexedCycle3(Y);");
                        break;

                    case AddressingMode.IndexedIndirectX:
                        Add("AddressingModeIndexedIndirectXCycle0();");
                        Add("AddressingModeIndexedIndirectXCycle1();");
                        Add("AddressingModeIndexedIndirectXCycle2();");
                        Add("AddressingModeIndexedIndirectXCycle3();");
                        Add("AddressingModeIndexedIndirectXCycle4();");
                        break;

                    case AddressingMode.IndirectIndexedY:
                        Add("AddressingModeIndirectIndexedYCycle0();");
                        Add("AddressingModeIndirectIndexedYCycle1();");
                        Add("AddressingModeIndirectIndexedYCycle2();");
                        Add("AddressingModeIndirectIndexedYCycle3();");
                        if (instruction.MemoryAccess == MemoryAccess.Read)
                        {
                            ModifyPrevious("AddressingModeIndirectIndexedYCycle3Read();");
                        }
                        Add("AddressingModeIndirectIndexedYCycle4();");
                        break;

                    case AddressingMode.Indirect:
                        Add("AddressingModeIndirectCycle0();");
                        Add("AddressingModeIndirectCycle1();");
                        Add("AddressingModeIndirectCycle2();");
                        Add("AddressingModeIndirectCycle3();");
                        Add("AddressingModeIndirectCycle4();");
                        break;

                    case AddressingMode.JSR:
                        break;

                    case AddressingMode.Invalid:
                        Add("AddressingModeInvalidCycle0();");
                        Add("AddressingModeInvalidCycle1();");
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            public string[] ToArray()
            {
                return _lines.ToArray();
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

                return new InstructionCode(comment, codeBuilder.ToArray());
            }

            public string Comment { get; }
            public string[] Lines { get; }

            private InstructionCode(string comment, string[] lines)
            {
                Comment = comment;
                Lines = lines;
            }
        }
    }
}
