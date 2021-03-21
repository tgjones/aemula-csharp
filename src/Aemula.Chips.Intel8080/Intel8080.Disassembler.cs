using System;
using System.Text;

namespace Aemula.Chips.Intel8080
{
    partial class Intel8080
    {
        private static Encoding Encoding = Encoding.UTF8;

        public static void Disassemble(
            ushort address,
            Func<ushort, byte> readMemory,
            OutputStringDelegate output)
        {
            var opcode = readMemory(address);

            switch (opcode)
            {
                case 0x00: Do0("NOP"); break;
                case 0x01: Do2("LXI B, 0x"); break;
                case 0x02: Do0("STAX B"); break;
                case 0x03: Do0("INX B"); break;
                case 0x04: Do0("INR B"); break;
                case 0x05: Do0("DCR B"); break;
                case 0x06: Do1("MVI B, 0x"); break;
                case 0x07: Do0("RLC"); break;

                case 0x09: Do0("DAD B"); break;
                case 0x0A: Do0("LDAX B"); break;
                case 0x0B: Do0("DCX B"); break;
                case 0x0C: Do0("INR C"); break;
                case 0x0D: Do0("DCR C"); break;
                case 0x0E: Do1("MVI C, 0x"); break;
                case 0x0F: Do0("RRC"); break;

                case 0x11: Do2("LXI D, 0x"); break;
                case 0x12: Do0("STAX D"); break;
                case 0x13: Do0("INX D"); break;
                case 0x14: Do0("INR D"); break;
                case 0x15: Do0("DCR D"); break;
                case 0x16: Do1("MVI D, 0x"); break;
                case 0x17: Do0("RAL"); break;

                case 0x19: Do0("DAD D"); break;
                case 0x1A: Do0("LDAX D"); break;
                case 0x1B: Do0("DCX D"); break;
                case 0x1C: Do0("INR E"); break;
                case 0x1D: Do0("DCR E"); break;
                case 0x1E: Do1("MVI E, 0x"); break;
                case 0x1F: Do0("RAR"); break;

                case 0x21: Do2("LXI H, 0x"); break;
                case 0x22: Do2("SHLD 0x"); break;
                case 0x23: Do0("INX H"); break;
                case 0x24: Do0("INR H"); break;
                case 0x25: Do0("DCR H"); break;
                case 0x26: Do1("MVI H, 0x"); break;
                case 0x27: Do0("DAA"); break;

                case 0x29: Do0("DAD H"); break;
                case 0x2A: Do2("LHLD 0x"); break;
                case 0x2B: Do0("DCX H"); break;
                case 0x2C: Do0("INR L"); break;
                case 0x2D: Do0("DCR L"); break;
                case 0x2E: Do1("MVI L, 0x"); break;
                case 0x2F: Do0("CMA"); break;

                case 0x31: Do2("LXI SP, 0x"); break;
                case 0x32: Do2("STA 0x"); break;
                case 0x33: Do0("INX SP"); break;
                case 0x34: Do0("INR M"); break;
                case 0x35: Do0("DCR M"); break;
                case 0x36: Do1("MVI M, 0x"); break;
                case 0x37: Do0("STC"); break;

                case 0x39: Do0("DAD SP"); break;
                case 0x3A: Do2("LDA 0x"); break;
                case 0x3B: Do0("DCX SP"); break;
                case 0x3C: Do0("INR A"); break;
                case 0x3D: Do0("DCR A"); break;
                case 0x3E: Do1("MVI A, 0x"); break;
                case 0x3F: Do0("CMC"); break;

                case 0x40: Do0("MOV B, B"); break;
                case 0x41: Do0("MOV B, C"); break;
                case 0x42: Do0("MOV B, D"); break;
                case 0x43: Do0("MOV B, E"); break;
                case 0x44: Do0("MOV B, H"); break;
                case 0x45: Do0("MOV B, L"); break;
                case 0x46: Do0("MOV B, M"); break;
                case 0x47: Do0("MOV B, A"); break;

                case 0x48: Do0("MOV C, B"); break;
                case 0x49: Do0("MOV C, C"); break;
                case 0x4A: Do0("MOV C, D"); break;
                case 0x4B: Do0("MOV C, E"); break;
                case 0x4C: Do0("MOV C, H"); break;
                case 0x4D: Do0("MOV C, L"); break;
                case 0x4E: Do0("MOV C, M"); break;
                case 0x4F: Do0("MOV C, A"); break;

                case 0x50: Do0("MOV D, B"); break;
                case 0x51: Do0("MOV D, C"); break;
                case 0x52: Do0("MOV D, D"); break;
                case 0x53: Do0("MOV D, E"); break;
                case 0x54: Do0("MOV D, H"); break;
                case 0x55: Do0("MOV D, L"); break;
                case 0x56: Do0("MOV D, M"); break;
                case 0x57: Do0("MOV D, A"); break;

                case 0x58: Do0("MOV E, B"); break;
                case 0x59: Do0("MOV E, C"); break;
                case 0x5A: Do0("MOV E, D"); break;
                case 0x5B: Do0("MOV E, E"); break;
                case 0x5C: Do0("MOV E, H"); break;
                case 0x5D: Do0("MOV E, L"); break;
                case 0x5E: Do0("MOV E, M"); break;
                case 0x5F: Do0("MOV E, A"); break;

                case 0x60: Do0("MOV H, B"); break;
                case 0x61: Do0("MOV H, C"); break;
                case 0x62: Do0("MOV H, D"); break;
                case 0x63: Do0("MOV H, E"); break;
                case 0x64: Do0("MOV H, H"); break;
                case 0x65: Do0("MOV H, L"); break;
                case 0x66: Do0("MOV H, M"); break;
                case 0x67: Do0("MOV H, A"); break;

                case 0x68: Do0("MOV L, B"); break;
                case 0x69: Do0("MOV L, C"); break;
                case 0x6A: Do0("MOV L, D"); break;
                case 0x6B: Do0("MOV L, E"); break;
                case 0x6C: Do0("MOV L, H"); break;
                case 0x6D: Do0("MOV L, L"); break;
                case 0x6E: Do0("MOV L, M"); break;
                case 0x6F: Do0("MOV L, A"); break;

                case 0x70: Do0("MOV M, B"); break;
                case 0x71: Do0("MOV M, C"); break;
                case 0x72: Do0("MOV M, D"); break;
                case 0x73: Do0("MOV M, E"); break;
                case 0x74: Do0("MOV M, H"); break;
                case 0x75: Do0("MOV M, L"); break;
                case 0x76: Do0("HLT"); break;
                case 0x77: Do0("MOV M, A"); break;

                case 0x78: Do0("MOV A, B"); break;
                case 0x79: Do0("MOV A, C"); break;
                case 0x7A: Do0("MOV A, D"); break;
                case 0x7B: Do0("MOV A, E"); break;
                case 0x7C: Do0("MOV A, H"); break;
                case 0x7D: Do0("MOV A, L"); break;
                case 0x7E: Do0("MOV A, M"); break;
                case 0x7F: Do0("MOV A, A"); break;

                case 0x80: Do0("ADD B"); break;
                case 0x81: Do0("ADD C"); break;
                case 0x82: Do0("ADD D"); break;
                case 0x83: Do0("ADD E"); break;
                case 0x84: Do0("ADD H"); break;
                case 0x85: Do0("ADD L"); break;
                case 0x86: Do0("ADD M"); break;
                case 0x87: Do0("ADD A"); break;

                case 0x88: Do0("ADC B"); break;
                case 0x89: Do0("ADC C"); break;
                case 0x8A: Do0("ADC D"); break;
                case 0x8B: Do0("ADC E"); break;
                case 0x8C: Do0("ADC H"); break;
                case 0x8D: Do0("ADC L"); break;
                case 0x8E: Do0("ADC M"); break;
                case 0x8F: Do0("ADC A"); break;

                case 0x90: Do0("SUB B"); break;
                case 0x91: Do0("SUB C"); break;
                case 0x92: Do0("SUB D"); break;
                case 0x93: Do0("SUB E"); break;
                case 0x94: Do0("SUB H"); break;
                case 0x95: Do0("SUB L"); break;
                case 0x96: Do0("SUB M"); break;
                case 0x97: Do0("SUB A"); break;

                case 0x98: Do0("SBB B"); break;
                case 0x99: Do0("SBB C"); break;
                case 0x9A: Do0("SBB D"); break;
                case 0x9B: Do0("SBB E"); break;
                case 0x9C: Do0("SBB H"); break;
                case 0x9D: Do0("SBB L"); break;
                case 0x9E: Do0("SBB M"); break;
                case 0x9F: Do0("SBB A"); break;

                case 0xA0: Do0("ANA B"); break;
                case 0xA1: Do0("ANA C"); break;
                case 0xA2: Do0("ANA D"); break;
                case 0xA3: Do0("ANA E"); break;
                case 0xA4: Do0("ANA H"); break;
                case 0xA5: Do0("ANA L"); break;
                case 0xA6: Do0("ANA M"); break;
                case 0xA7: Do0("ANA A"); break;

                case 0xA8: Do0("XRA B"); break;
                case 0xA9: Do0("XRA C"); break;
                case 0xAA: Do0("XRA D"); break;
                case 0xAB: Do0("XRA E"); break;
                case 0xAC: Do0("XRA H"); break;
                case 0xAD: Do0("XRA L"); break;
                case 0xAE: Do0("XRA M"); break;
                case 0xAF: Do0("XRA A"); break;

                case 0xB0: Do0("ORA B"); break;
                case 0xB1: Do0("ORA C"); break;
                case 0xB2: Do0("ORA D"); break;
                case 0xB3: Do0("ORA E"); break;
                case 0xB4: Do0("ORA H"); break;
                case 0xB5: Do0("ORA L"); break;
                case 0xB6: Do0("ORA M"); break;
                case 0xB7: Do0("ORA A"); break;

                case 0xB8: Do0("CMP B"); break;
                case 0xB9: Do0("CMP C"); break;
                case 0xBA: Do0("CMP D"); break;
                case 0xBB: Do0("CMP E"); break;
                case 0xBC: Do0("CMP H"); break;
                case 0xBD: Do0("CMP L"); break;
                case 0xBE: Do0("CMP M"); break;
                case 0xBF: Do0("CMP A"); break;

                case 0xC0: Do0("RNZ"); break;
                case 0xC1: Do0("POP B"); break;
                case 0xC2: Do2("JNZ 0x"); break;
                case 0xC3: Do2("JMP 0x"); break;
                case 0xC4: Do2("CNZ 0x"); break;
                case 0xC5: Do0("PUSH B"); break;
                case 0xC6: Do1("ADI 0x"); break;

                case 0xC8: Do0("RZ"); break;
                case 0xC9: Do0("RET"); break;
                case 0xCA: Do2("JZ 0x"); break;
                case 0xCC: Do2("CZ 0x"); break;
                case 0xCD: Do2("CALL 0x"); break;
                case 0xCE: Do1("ACI 0x"); break;

                case 0xD0: Do0("RNC"); break;
                case 0xD1: Do0("POP D"); break;
                case 0xD2: Do2("JNC 0x"); break;
                case 0xD4: Do2("CNC 0x"); break;
                case 0xD5: Do0("PUSH D"); break;
                case 0xD6: Do1("SUI 0x"); break;

                case 0xD8: Do0("RC"); break;
                case 0xDA: Do2("JC 0x"); break;
                case 0xDC: Do2("CC 0x"); break;
                case 0xDE: Do1("SBI 0x"); break;

                case 0xE0: Do0("RPO"); break;
                case 0xE1: Do0("POP H"); break;
                case 0xE2: Do2("JPO 0x"); break;
                case 0xE3: Do0("XTHL"); break;
                case 0xE4: Do2("CPO 0x"); break;
                case 0xE5: Do0("PUSH H"); break;
                case 0xE6: Do1("ANI 0x"); break;

                case 0xE8: Do0("RPE"); break;
                case 0xE9: Do0("PCHL"); break;
                case 0xEB: Do0("XCHG"); break;
                case 0xEC: Do2("CPE 0x"); break;
                case 0xEA: Do2("JPE 0x"); break;
                case 0xEE: Do1("XRI 0x"); break;

                case 0xF0: Do0("RP"); break;
                case 0xF1: Do0("POP PSW"); break;
                case 0xF2: Do2("JP 0x"); break;
                case 0xF4: Do2("CP 0x"); break;
                case 0xF5: Do0("PUSH PSW"); break;
                case 0xF6: Do1("ORI 0x"); break;

                case 0xF8: Do0("RM"); break;
                case 0xF9: Do0("SPHL"); break;
                case 0xFA: Do2("JM 0x"); break;
                case 0xFC: Do2("CM 0x"); break;
                case 0xFE: Do1("CPI 0x"); break;

                default:
                    throw new InvalidOperationException($"Opcode 0x{opcode:X2} not supported");
            }

            void Do0(string prefix)
            {
                DoHelper(prefix, ReadOnlySpan<char>.Empty);
            }

            void Do1(string prefix)
            {
                var operand = readMemory((ushort)(address + 1));

                DoHelper(
                    prefix,
                    stackalloc char[2]
                    {
                        HexChars[operand / 16],
                        HexChars[operand % 16],
                    });
            }

            void Do2(string prefix)
            {
                var operandLo = readMemory((ushort)(address + 1));
                var operandHi = readMemory((ushort)(address + 2));

                DoHelper(
                    prefix,
                    stackalloc char[4]
                    {
                        HexChars[operandHi / 16],
                        HexChars[operandHi % 16],
                        HexChars[operandLo / 16],
                        HexChars[operandLo % 16],
                    });
            }

            unsafe void DoHelper(string prefix, ReadOnlySpan<char> operandChars)
            {
                var prefixByteCount = Encoding.GetByteCount(prefix);
                var operandByteCount = Encoding.GetByteCount(operandChars);
                var totalByteCount = prefixByteCount + operandByteCount;

                var bytes = stackalloc byte[totalByteCount];
                var bytesSpan = new Span<byte>(bytes, totalByteCount);

                // Copy prefix.
                Encoding.GetBytes(prefix, bytesSpan);

                // Copy operands.
                if (operandChars.Length > 0)
                {
                    fixed (char* operandCharsPtr = operandChars)
                    {
                        Encoding.GetBytes(
                            operandCharsPtr,
                            operandChars.Length,
                            bytes + prefixByteCount,
                            operandByteCount);
                    }
                }

                output(bytesSpan);
            }
        }

        private static readonly char[] HexChars =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F'
        };
    }

    public unsafe delegate void OutputStringDelegate(ReadOnlySpan<byte> bytes);
}
