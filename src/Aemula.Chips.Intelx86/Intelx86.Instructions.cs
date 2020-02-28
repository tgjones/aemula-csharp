using System;
using System.Collections.Generic;

namespace Aemula.Chips.Intelx86
{
    partial class Intelx86
    {
        private Dictionary<byte, Action> CreateInstructions()
        {
            return new Dictionary<byte, Action>
            {
                {
                    0x31,
                    () =>
                    {
                        FetchModRM();
                        WriteEffective16(Xor16(ReadEffective16(), ReadRegister16()));
                    }
                },

                { 0x40, () => Registers[RegAX].Word0 = Inc16(Registers[RegAX].Word0) },
                { 0x41, () => Registers[RegCX].Word0 = Inc16(Registers[RegCX].Word0) },
                { 0x42, () => Registers[RegDX].Word0 = Inc16(Registers[RegDX].Word0) },
                { 0x43, () => Registers[RegBX].Word0 = Inc16(Registers[RegBX].Word0) },
                { 0x44, () => Registers[RegSP].Word0 = Inc16(Registers[RegSP].Word0) },
                { 0x45, () => Registers[RegBP].Word0 = Inc16(Registers[RegBP].Word0) },
                { 0x46, () => Registers[RegSI].Word0 = Inc16(Registers[RegSI].Word0) },
                { 0x47, () => Registers[RegDI].Word0 = Inc16(Registers[RegDI].Word0) },

                { 0x48, () => Registers[RegAX].Word0 = Dec16(Registers[RegAX].Word0) },
                { 0x49, () => Registers[RegCX].Word0 = Dec16(Registers[RegCX].Word0) },
                { 0x4A, () => Registers[RegDX].Word0 = Dec16(Registers[RegDX].Word0) },
                { 0x4B, () => Registers[RegBX].Word0 = Dec16(Registers[RegBX].Word0) },
                { 0x4C, () => Registers[RegSP].Word0 = Dec16(Registers[RegSP].Word0) },
                { 0x4D, () => Registers[RegBP].Word0 = Dec16(Registers[RegBP].Word0) },
                { 0x4E, () => Registers[RegSI].Word0 = Dec16(Registers[RegSI].Word0) },
                { 0x4F, () => Registers[RegDI].Word0 = Dec16(Registers[RegDI].Word0) },

                { 0x50, () => Push16(Registers[RegAX].Word0) },
                { 0x51, () => Push16(Registers[RegCX].Word0) },
                { 0x52, () => Push16(Registers[RegDX].Word0) },
                { 0x53, () => Push16(Registers[RegBX].Word0) },
                { 0x54, () => Push16(Registers[RegSP].Word0) },
                { 0x55, () => Push16(Registers[RegBP].Word0) },
                { 0x56, () => Push16(Registers[RegSI].Word0) },
                { 0x57, () => Push16(Registers[RegDI].Word0) },

                { 0x58, () => Registers[RegAX].Word0 = Pop16() },
                { 0x59, () => Registers[RegCX].Word0 = Pop16() },
                { 0x5A, () => Registers[RegDX].Word0 = Pop16() },
                { 0x5B, () => Registers[RegBX].Word0 = Pop16() },
                { 0x5C, () => Registers[RegSP].Word0 = Pop16() },
                { 0x5D, () => Registers[RegBP].Word0 = Pop16() },
                { 0x5E, () => Registers[RegSI].Word0 = Pop16() },
                { 0x5F, () => Registers[RegDI].Word0 = Pop16() },

                { 0x70, () => Jmp8(Flags.OF) },
                { 0x71, () => Jmp8(!Flags.OF) },
                { 0x72, () => Jmp8(Flags.CF) },
                { 0x73, () => Jmp8(!Flags.CF) },
                { 0x74, () => Jmp8(Flags.ZF) },
                { 0x75, () => Jmp8(!Flags.ZF) },
                { 0x76, () => Jmp8(Flags.CF || Flags.ZF) },
                { 0x77, () => Jmp8(!Flags.CF && !Flags.ZF) },
                { 0x78, () => Jmp8(Flags.SF) },
                { 0x79, () => Jmp8(!Flags.SF) },
                { 0x7A, () => Jmp8(Flags.PF) },
                { 0x7B, () => Jmp8(!Flags.PF) },
                { 0x7C, () => Jmp8(Flags.SF != Flags.OF) },
                { 0x7D, () => Jmp8(Flags.SF == Flags.OF) },
                { 0x7E, () => Jmp8(Flags.ZF || Flags.SF != Flags.OF) },
                { 0x7F, () => Jmp8(!Flags.ZF && Flags.SF == Flags.OF) },

                {
                    0x81,
                    () =>
                    {
                        FetchModRM();
                        switch (_modRM.RegOrOpcodeExtension)
                        {
                            case 0: WriteEffective16(Add16(ReadEffective16(), Fetch16())); break;
                            case 7: Cmp16(ReadEffective16(), Fetch16()); break;
                        }
                    }
                },

                {
                    0x83,
                    () =>
                    {
                        FetchModRM();
                        switch (_modRM.RegOrOpcodeExtension)
                        {
                            case 0: WriteEffective16(Add16(ReadEffective16(), SignExtend8to16(Fetch8()))); break;
                            case 7: Cmp16(ReadEffective16(), SignExtend8to16(Fetch8())); break;
                        }
                    }
                },

                {
                    0x88,
                    () =>
                    {
                        FetchModRM();
                        WriteEffective8(ReadRegister8());
                    }
                },

                {
                    0x89,
                    () =>
                    {
                        FetchModRM();
                        WriteEffective16(ReadRegister16());
                    }
                },

                {
                    0x8B,
                    () =>
                    {
                        FetchModRM();
                        WriteRegister16(ReadEffective16());
                    }
                },

                { 0xB0, () => Registers[RegAX].Byte0 = Fetch8() },
                { 0xB1, () => Registers[RegCX].Byte0 = Fetch8() },
                { 0xB2, () => Registers[RegDX].Byte0 = Fetch8() },
                { 0xB3, () => Registers[RegBX].Byte0 = Fetch8() },
                { 0xB4, () => Registers[RegAX].Byte1 = Fetch8() },
                { 0xB5, () => Registers[RegCX].Byte1 = Fetch8() },
                { 0xB6, () => Registers[RegDX].Byte1 = Fetch8() },
                { 0xB7, () => Registers[RegBX].Byte1 = Fetch8() },

                { 0xB8, () => Registers[RegAX].Word0 = Fetch16() },
                { 0xB9, () => Registers[RegCX].Word0 = Fetch16() },
                { 0xBA, () => Registers[RegDX].Word0 = Fetch16() },
                { 0xBB, () => Registers[RegBX].Word0 = Fetch16() },
                { 0xBC, () => Registers[RegSP].Word0 = Fetch16() },
                { 0xBD, () => Registers[RegBP].Word0 = Fetch16() },
                { 0xBE, () => Registers[RegSI].Word0 = Fetch16() },
                { 0xBF, () => Registers[RegDI].Word0 = Fetch16() },

                { 0xC3, () => IP = Pop16() },

                {
                    0xE8,
                    () =>
                    {
                        var immediate16 = FetchI16();
                        Push16(IP);
                        JmpRel16(immediate16);
                    }
                },
            };
        }

        private void Jmp8(bool condition)
        {
            var immediate8 = FetchI8();
            if (condition)
            {
                IP = (ushort)(IP + immediate8);
            }
        }

        private void JmpRel16(short offset)
        {
            // TODO: Limit to current segment?
            IP = (ushort)(IP + offset);
        }

        private void Push16(ushort value)
        {
            _bus.Write((uint)(SP - 2), (byte)(value & 0xFF));
            _bus.Write((uint)(SP - 1), (byte)(value >> 8));
            SP -= 2;
        }

        private ushort Pop16()
        {
            var result = (ushort)(_bus.Read(SP) | (_bus.Read((uint)(SP + 1)) << 8));
            SP += 2;
            return result;
        }
    }
}
