//using System.Collections.Generic;

//namespace Aemula.Chips.Mos6502
//{
//    partial class Mos6502
//    {
//        private readonly Dictionary<byte, InstructionImplementation> _instructionImplementations;

//        private Dictionary<byte, InstructionImplementation> CreateInstructionImplementations()
//        {
//            return new Dictionary<byte, InstructionImplementation>
//            {
//                // Branch
//                { 0x90, new Branch("BCC", () => P.C, false) },
//                { 0xB0, new Branch("BCS", () => P.C, true) },
//                { 0xD0, new Branch("BNE", () => P.Z, false) },
//                { 0xF0, new Branch("BEQ", () => P.Z, true) },
//                { 0x10, new Branch("BPL", () => P.N, false) },
//                { 0x30, new Branch("BMI", () => P.N, true) },
//                { 0x50, new Branch("BVC", () => P.V, false) },
//                { 0x70, new Branch("BVS", () => P.V, true) },

//                // Flags
//                { 0x18, new Simple("CLC", () => P.C = false) },
//                { 0xD8, new Simple("CLD", () => P.D = false) },
//                { 0x58, new Simple("CLI", () => P.I = false) },
//                { 0xB8, new Simple("CLV", () => P.V = false) },
//                { 0x38, new Simple("SLC", () => P.C = true) },
//                { 0xF8, new Simple("SED", () => P.D = true) },
//                { 0x78, new Simple("SEI", () => P.I = true) },

//                // NOP
//                { 0x1A, new Simple("NOP", Nop) }, // Undocumented
//                { 0x3A, new Simple("NOP", Nop) }, // Undocumented
//                { 0x5A, new Simple("NOP", Nop) }, // Undocumented
//                { 0x7A, new Simple("NOP", Nop) }, // Undocumented
//                { 0xDA, new Simple("NOP", Nop) }, // Undocumented
//                { 0xEA, new Simple("NOP", Nop) },
//                { 0xFA, new Simple("NOP", Nop) }, // Undocumented
//                { 0x80, new Immediate("NOP", Nop) }, // Undocumented
//                { 0x1C, new AbsoluteXRead("NOP", Nop) }, // Undocumented
//                { 0x3C, new AbsoluteXRead("NOP", Nop) }, // Undocumented
//                { 0x5C, new AbsoluteXRead("NOP", Nop) }, // Undocumented
//                { 0x7C, new AbsoluteXRead("NOP", Nop) }, // Undocumented
//                { 0xDC, new AbsoluteXRead("NOP", Nop) }, // Undocumented
//                { 0xFC, new AbsoluteXRead("NOP", Nop) }, // Undocumented
//                { 0x04, new ZeroPageRead("NOP", Nop) }, // Undocumented
//                { 0x44, new ZeroPageRead("NOP", Nop) }, // Undocumented
//                { 0x64, new ZeroPageRead("NOP", Nop) }, // Undocumented
//                { 0x0C, new AbsoluteRead("NOP", Nop) }, // Undocumented
//                { 0x14, new ZeroPageXRead("NOP", Nop) }, // Undocumented
//                { 0x34, new ZeroPageXRead("NOP", Nop) }, // Undocumented
//                { 0x54, new ZeroPageXRead("NOP", Nop) }, // Undocumented
//                { 0x74, new ZeroPageXRead("NOP", Nop) }, // Undocumented
//                { 0xD4, new ZeroPageXRead("NOP", Nop) }, // Undocumented
//                { 0xF4, new ZeroPageXRead("NOP", Nop) }, // Undocumented

//                // Simple
//                { 0xAA, new Simple("TAX", Tax) },
//                { 0x8A, new Simple("TXA", Txa) },
//                { 0xA8, new Simple("TAY", Tay) },
//                { 0x98, new Simple("TYA", Tya) },
//                { 0xBA, new Simple("TSX", Tsx) },
//                { 0x9A, new Simple("TXS", Txs) },
//                { 0xCA, new Simple("DEX", Dex) },
//                { 0x88, new Simple("DEY", Dey) },
//                { 0xE8, new Simple("INX", Inx) },
//                { 0xC8, new Simple("INY", Iny) },

//                // Stack
//                { 0x48, new Pha() },
//                { 0x68, new Pla() },
//                { 0x08, new Php() },
//                { 0x28, new Plp() },

//                // Jumps, returns
//                { 0x00, new Break() },
//                { 0x4C, new JmpAbsolute() },
//                { 0x6C, new JmpIndirect() },
//                { 0x20, new Jsr() },
//                { 0x60, new Rts() },
//                { 0x40, new Rti() },

//                // ADC
//                { 0x69, new Immediate("AND", Adc) },
//                { 0x65, new ZeroPageRead("AND", Adc) },
//                { 0x75, new ZeroPageXRead("AND", Adc) },
//                { 0x6D, new AbsoluteRead("AND", Adc) },
//                { 0x7D, new AbsoluteXRead("AND", Adc) },
//                { 0x79, new AbsoluteYRead("AND", Adc) },
//                { 0x61, new IndexedIndirectXRead("AND", Adc) },
//                { 0x71, new IndirectIndexedYRead("AND", Adc) },

//                // AND
//                { 0x29, new Immediate("AND", And) },
//                { 0x25, new ZeroPageRead("AND", And) },
//                { 0x35, new ZeroPageXRead("AND", And) },
//                { 0x2D, new AbsoluteRead("AND", And) },
//                { 0x3D, new AbsoluteXRead("AND", And) },
//                { 0x39, new AbsoluteYRead("AND", And) },
//                { 0x21, new IndexedIndirectXRead("AND", And) },
//                { 0x31, new IndirectIndexedYRead("AND", And) },

//                // ASL
//                { 0x0A, new AccumulatorReadModifyWrite("ASL", Asl) },
//                { 0x06, new ZeroPageReadModifyWrite("ASL", Asl) },
//                { 0x16, new ZeroPageXReadModifyWrite("ASL", Asl) },
//                { 0x0E, new AbsoluteReadModifyWrite("ASL", Asl) },
//                { 0x1E, new AbsoluteXReadModifyWrite("ASL", Asl) },

//                // BIT
//                { 0x24, new ZeroPageRead("BIT", Bit) },
//                { 0x2C, new AbsoluteRead("BIT", Bit) },

//                // CMP
//                { 0xC9, new Immediate("CMP", Cmp) },
//                { 0xC5, new ZeroPageRead("CMP", Cmp) },
//                { 0xD5, new ZeroPageXRead("CMP", Cmp) },
//                { 0xCD, new AbsoluteRead("CMP", Cmp) },
//                { 0xDD, new AbsoluteXRead("CMP", Cmp) },
//                { 0xD9, new AbsoluteYRead("CMP", Cmp) },
//                { 0xC1, new IndexedIndirectXRead("CMP", Cmp) },
//                { 0xD1, new IndirectIndexedYRead("CMP", Cmp) },

//                // CPX
//                { 0xE0, new Immediate("CPX", Cpx) },
//                { 0xE4, new ZeroPageRead("CPX", Cpx) },
//                { 0xEC, new AbsoluteRead("CPX", Cpx) },

//                // CPY
//                { 0xC0, new Immediate("CPY", Cpy) },
//                { 0xC4, new ZeroPageRead("CPY", Cpy) },
//                { 0xCC, new AbsoluteRead("CPY", Cpy) },

//                // DEC
//                { 0xC6, new ZeroPageReadModifyWrite("DEC", Dec) },
//                { 0xD6, new ZeroPageXReadModifyWrite("DEC", Dec) },
//                { 0xCE, new AbsoluteReadModifyWrite("DEC", Dec) },
//                { 0xDE, new AbsoluteXReadModifyWrite("DEC", Dec) },

//                // EOR
//                { 0x49, new Immediate("EOR", Eor) },
//                { 0x45, new ZeroPageRead("EOR", Eor) },
//                { 0x55, new ZeroPageXRead("EOR", Eor) },
//                { 0x4D, new AbsoluteRead("EOR", Eor) },
//                { 0x5D, new AbsoluteXRead("EOR", Eor) },
//                { 0x59, new AbsoluteYRead("EOR", Eor) },
//                { 0x41, new IndexedIndirectXRead("EOR", Eor) },
//                { 0x51, new IndirectIndexedYRead("EOR", Eor) },

//                // INC
//                { 0xE6, new ZeroPageReadModifyWrite("INC", Inc) },
//                { 0xF6, new ZeroPageXReadModifyWrite("INC", Inc) },
//                { 0xEE, new AbsoluteReadModifyWrite("INC", Inc) },
//                { 0xFE, new AbsoluteXReadModifyWrite("INC", Inc) },

//                // LDA
//                { 0xA9, new Immediate("LDA", Lda) },
//                { 0xA5, new ZeroPageRead("LDA", Lda) },
//                { 0xB5, new ZeroPageXRead("LDA", Lda) },
//                { 0xAD, new AbsoluteRead("LDA", Lda) },
//                { 0xBD, new AbsoluteXRead("LDA", Lda) },
//                { 0xB9, new AbsoluteYRead("LDA", Lda) },
//                { 0xA1, new IndexedIndirectXRead("LDA", Lda) },
//                { 0xB1, new IndirectIndexedYRead("LDA", Lda) },

//                // LDX
//                { 0xA2, new Immediate("LDX", Ldx) },
//                { 0xA6, new ZeroPageRead("LDX", Ldx) },
//                { 0xB6, new ZeroPageYRead("LDX", Ldx) },
//                { 0xAE, new AbsoluteRead("LDX", Ldx) },
//                { 0xBE, new AbsoluteYRead("LDX", Ldx) },

//                // LDY
//                { 0xA0, new Immediate("LDY", Ldy) },
//                { 0xA4, new ZeroPageRead("LDY", Ldy) },
//                { 0xB4, new ZeroPageXRead("LDY", Ldy) },
//                { 0xAC, new AbsoluteRead("LDY", Ldy) },
//                { 0xBC, new AbsoluteXRead("LDY", Ldy) },

//                // LSR
//                { 0x4A, new AccumulatorReadModifyWrite("LSR", Lsr) },
//                { 0x46, new ZeroPageReadModifyWrite("LSR", Lsr) },
//                { 0x56, new ZeroPageXReadModifyWrite("LSR", Lsr) },
//                { 0x4E, new AbsoluteReadModifyWrite("LSR", Lsr) },
//                { 0x5E, new AbsoluteXReadModifyWrite("LSR", Lsr) },

//                // ORA
//                { 0x09, new Immediate("ORA", Ora) },
//                { 0x05, new ZeroPageRead("ORA", Ora) },
//                { 0x15, new ZeroPageXRead("ORA", Ora) },
//                { 0x0D, new AbsoluteRead("ORA", Ora) },
//                { 0x1D, new AbsoluteXRead("ORA", Ora) },
//                { 0x19, new AbsoluteYRead("ORA", Ora) },
//                { 0x01, new IndexedIndirectXRead("ORA", Ora) },
//                { 0x11, new IndirectIndexedYRead("ORA", Ora) },

//                // ROL
//                { 0x2A, new AccumulatorReadModifyWrite("ROL", Rol) },
//                { 0x26, new ZeroPageReadModifyWrite("ROL", Rol) },
//                { 0x36, new ZeroPageXReadModifyWrite("ROL", Rol) },
//                { 0x2E, new AbsoluteReadModifyWrite("ROL", Rol) },
//                { 0x3E, new AbsoluteXReadModifyWrite("ROL", Rol) },

//                // ROR
//                { 0x6A, new AccumulatorReadModifyWrite("ROR", Ror) },
//                { 0x66, new ZeroPageReadModifyWrite("ROR", Ror) },
//                { 0x76, new ZeroPageXReadModifyWrite("ROR", Ror) },
//                { 0x6E, new AbsoluteReadModifyWrite("ROR", Ror) },
//                { 0x7E, new AbsoluteXReadModifyWrite("ROR", Ror) },

//                // SBC
//                { 0xE9, new Immediate("SBC", Sbc) },
//                { 0xEB, new Immediate("SBC", Sbc) }, // Undocumented
//                { 0xE5, new ZeroPageRead("SBC", Sbc) },
//                { 0xF5, new ZeroPageXRead("SBC", Sbc) },
//                { 0xED, new AbsoluteRead("SBC", Sbc) },
//                { 0xFD, new AbsoluteXRead("SBC", Sbc) },
//                { 0xF9, new AbsoluteYRead("SBC", Sbc) },
//                { 0xE1, new IndexedIndirectXRead("SBC", Sbc) },
//                { 0xF1, new IndirectIndexedYRead("SBC", Sbc) },

//                // STA
//                { 0x85, new ZeroPageWrite("STA", Sta) },
//                { 0x95, new ZeroPageXWrite("STA", Sta) },
//                { 0x8D, new AbsoluteWrite("STA", Sta) },
//                { 0x9D, new AbsoluteXWrite("STA", Sta) },
//                { 0x99, new AbsoluteYWrite("STA", Sta) },
//                { 0x81, new IndexedIndirectXWrite("STA", Sta) },
//                { 0x91, new IndirectIndexedYWrite("STA", Sta) },

//                // STX
//                { 0x86, new ZeroPageWrite("STX", Stx) },
//                { 0x96, new ZeroPageYWrite("STX", Stx) },
//                { 0x8E, new AbsoluteWrite("STX", Stx) },

//                // STY
//                { 0x84, new ZeroPageWrite("STY", Sty) },
//                { 0x94, new ZeroPageXWrite("STY", Sty) },
//                { 0x8C, new AbsoluteWrite("STY", Sty) },

//                // LAX (undocumented)
//                { 0xA3, new IndexedIndirectXRead("LAX", Lax) }, // Undocumented
//                { 0xA7, new ZeroPageRead("LAX", Lax) }, // Undocumented
//                { 0xAF, new AbsoluteRead("LAX", Lax) }, // Undocumented
//                { 0xB3, new IndirectIndexedYRead("LAX", Lax) }, // Undocumented
//                { 0xB7, new ZeroPageYRead("LAX", Lax) }, // Undocumented
//                { 0xBF, new AbsoluteYRead("LAX", Lax) }, // Undocumented

//                // SAX (undocumented)
//                { 0x83, new IndexedIndirectXWrite("SAX", Sax) }, // Undocumented
//                { 0x87, new ZeroPageWrite("SAX", Sax) }, // Undocumented
//                { 0x8F, new AbsoluteWrite("SAX", Sax) }, // Undocumented
//                { 0x97, new ZeroPageYWrite("SAX", Sax) }, // Undocumented

//                // DCP (undocumented)
//                { 0xC3, new IndexedIndirectXReadModifyWrite("DCP", Dcp) }, // Undocumented
//                { 0xC7, new ZeroPageReadModifyWrite("DCP", Dcp) }, // Undocumented
//                { 0xCF, new AbsoluteReadModifyWrite("DCP", Dcp) }, // Undocumented
//                { 0xD3, new IndirectIndexedYReadModifyWrite("DCP", Dcp) }, // Undocumented
//                { 0xD7, new ZeroPageXReadModifyWrite("DCP", Dcp) }, // Undocumented
//                { 0xDB, new AbsoluteYReadModifyWrite("DCP", Dcp) }, // Undocumented
//                { 0xDF, new AbsoluteXReadModifyWrite("DCP", Dcp) }, // Undocumented

//                // ISB (undocumented)
//                { 0xE3, new IndexedIndirectXReadModifyWrite("ISB", Isb) }, // Undocumented
//                { 0xE7, new ZeroPageReadModifyWrite("ISB", Isb) }, // Undocumented
//                { 0xEF, new AbsoluteReadModifyWrite("ISB", Isb) }, // Undocumented
//                { 0xF3, new IndirectIndexedYReadModifyWrite("ISB", Isb) }, // Undocumented
//                { 0xF7, new ZeroPageXReadModifyWrite("ISB", Isb) }, // Undocumented
//                { 0xFB, new AbsoluteYReadModifyWrite("ISB", Isb) }, // Undocumented
//                { 0xFF, new AbsoluteXReadModifyWrite("ISB", Isb) }, // Undocumented

//                // SLO (undocumented)
//                { 0x07, new ZeroPageReadModifyWrite("SLO", Slo) }, // Undocumented
//                { 0x17, new ZeroPageXReadModifyWrite("SLO", Slo) }, // Undocumented
//                { 0x03, new IndexedIndirectXReadModifyWrite("SLO", Slo) }, // Undocumented
//                { 0x13, new IndirectIndexedYReadModifyWrite("SLO", Slo) }, // Undocumented
//                { 0x0F, new AbsoluteReadModifyWrite("SLO", Slo) }, // Undocumented
//                { 0x1F, new AbsoluteXReadModifyWrite("SLO", Slo) }, // Undocumented
//                { 0x1B, new AbsoluteYReadModifyWrite("SLO", Slo) }, // Undocumented

//                // RLA (undocumented)
//                { 0x27, new ZeroPageReadModifyWrite("RLA", Rla) }, // Undocumented
//                { 0x37, new ZeroPageXReadModifyWrite("RLA", Rla) }, // Undocumented
//                { 0x23, new IndexedIndirectXReadModifyWrite("RLA", Rla) }, // Undocumented
//                { 0x33, new IndirectIndexedYReadModifyWrite("RLA", Rla) }, // Undocumented
//                { 0x2F, new AbsoluteReadModifyWrite("RLA", Rla) }, // Undocumented
//                { 0x3F, new AbsoluteXReadModifyWrite("RLA", Rla) }, // Undocumented
//                { 0x3B, new AbsoluteYReadModifyWrite("RLA", Rla) }, // Undocumented

//                // SRE (undocumented)
//                { 0x47, new ZeroPageReadModifyWrite("SRE", Sre) }, // Undocumented
//                { 0x57, new ZeroPageXReadModifyWrite("SRE", Sre) }, // Undocumented
//                { 0x43, new IndexedIndirectXReadModifyWrite("SRE", Sre) }, // Undocumented
//                { 0x53, new IndirectIndexedYReadModifyWrite("SRE", Sre) }, // Undocumented
//                { 0x4F, new AbsoluteReadModifyWrite("SRE", Sre) }, // Undocumented
//                { 0x5F, new AbsoluteXReadModifyWrite("SRE", Sre) }, // Undocumented
//                { 0x5B, new AbsoluteYReadModifyWrite("SRE", Sre) }, // Undocumented

//                // RRA (undocumented)
//                { 0x67, new ZeroPageReadModifyWrite("RRA", Rra) }, // Undocumented
//                { 0x77, new ZeroPageXReadModifyWrite("RRA", Rra) }, // Undocumented
//                { 0x63, new IndexedIndirectXReadModifyWrite("RRA", Rra) }, // Undocumented
//                { 0x73, new IndirectIndexedYReadModifyWrite("RRA", Rra) }, // Undocumented
//                { 0x6F, new AbsoluteReadModifyWrite("RRA", Rra) }, // Undocumented
//                { 0x7F, new AbsoluteXReadModifyWrite("RRA", Rra) }, // Undocumented
//                { 0x7B, new AbsoluteYReadModifyWrite("RRA", Rra) }, // Undocumented
//            };
//        }
//    }
//}