using Aemula.Chips.Arm.Instructions;
using System;

namespace Aemula.Chips.Arm
{
    internal static class InstructionDecoder
    {
        // A5.1
        public static Instruction Decode(uint rawInstruction)
        {
            var condition = rawInstruction.GetBits(28, 31);
            var op1 = (rawInstruction >> 25) & 0b111;

            switch (condition)
            {
                case 0b1111:
                    throw new NotImplementedException();

                default:
                    switch (op1)
                    {
                        case 0b000:
                        case 0b001:
                            return DecodeDataProcessingAndMiscellaneous(rawInstruction);

                        case 0b010:
                            return DecodeLoadStoreWordAndUnsignedByte(rawInstruction);

                        case var _ when (op1 & 0b110) == 0b100:
                            return DecodeBranchOrBranchWithLinkOrBlockTransfer(rawInstruction);

                        default:
                            throw new InvalidOperationException();
                    }
            }
        }

        // A5.2
        private static Instruction DecodeDataProcessingAndMiscellaneous(uint rawInstruction)
        {
            var op = (rawInstruction >> 25) & 0b1;
            var op1 = (rawInstruction >> 20) & 0b11111;

            switch (op)
            {
                case 0:
                    var op2 = (rawInstruction >> 4) & 0b1111;
                    switch (op1, op2)
                    {
                        case (_, _) when (op1 & 0b11001) != 0b10000 && (op2 & 0b0001) == 0b0000:
                            return DecodeDataProcessingRegister(rawInstruction, op1);

                        case (_, _) when (op1 & 0b11001) == 0b10000 && (op2 & 0b1000) == 0b0000:
                            return DecodeMiscellaneous(rawInstruction);

                        default:
                            throw new InvalidOperationException();
                    }

                case 1:
                    switch (op1)
                    {
                        case 0b10000: // 16-bit immediate load, MOV (immediate)
                            throw new NotImplementedException();

                        case 0b10100: // High halfword 16-bit immediate load, MOVT
                            throw new NotImplementedException();

                        case 0b10010: // MSR (immediate)
                        case 0b10110:
                            throw new NotImplementedException();

                        default: // Data-processing (immediate)
                            return DecodeDataProcessingImmediate(rawInstruction, op1);
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        // A5.2.1 Data-processing (register)
        private static Instruction DecodeDataProcessingRegister(uint rawInstruction, uint op)
        {
            var imm5 = rawInstruction.GetBits(7, 11);
            var op2 = rawInstruction.GetBits(5, 6);

            switch (op)
            {
                case 0b01000:
                case 0b01001:
                    return new AddRegister.A1(rawInstruction);

                case 0b10101:
                    return new CmpRegister.A1(rawInstruction);

                case 0b11010:
                case 0b11011:
                    switch (op2, imm5)
                    {
                        case (0b00, 0b00000):
                            return new MovRegister.A1(rawInstruction);

                        default:
                            throw new InvalidOperationException();
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        // A5.2.12 Miscellaneous instructions
        private static Instruction DecodeMiscellaneous(uint rawInstruction)
        {
            var op = rawInstruction.GetBits(21, 22);
            var op1 = rawInstruction.GetBits(16, 19);
            var b = rawInstruction.GetBitAsBool(9);
            var op2 = rawInstruction.GetBits(4, 6);

            switch (op2)
            {
                case 0b001:
                    switch (op)
                    {
                        case 0b01:
                            return new BX.A1(rawInstruction);

                        default:
                            throw new InvalidOperationException();
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        // A5.3 Load/store word and unsigned byte
        private static Instruction DecodeLoadStoreWordAndUnsignedByte(uint rawInstruction)
        {
            var a = rawInstruction.GetBit(25);
            var op1 = rawInstruction.GetBits(20, 24);
            var rn = rawInstruction.GetBits(16, 19);
            var b = rawInstruction.GetBit(4);

            switch (a, b)
            {
                case (0, _) when (op1 & 0b00101) == 0b00000 && (op1 & 0b10111) != 0b00010:
                    return new StrImmediate.A1(rawInstruction, rn);

                default:
                    throw new InvalidOperationException();
            }
        }

        // A5.5 Branch, branch with link, and block data transfer
        private static Instruction DecodeBranchOrBranchWithLinkOrBlockTransfer(uint rawInstruction)
        {
            var op = (rawInstruction >> 20) & 0b111111;
            var rn = (rawInstruction >> 16) & 0b1111;
            var r = (rawInstruction >> 15) & 0b1;

            switch (op)
            {
                case 0b001011:
                    switch (rn)
                    {
                        case 0b1101:
                            return new Pop.A1(rawInstruction);

                        default:
                            throw new NotImplementedException();
                    }

                case 0b010010:
                    switch (rn)
                    {
                        case 0b1101:
                            return new Push.A1(rawInstruction);

                        default:
                            throw new NotImplementedException();
                    }

                case var _ when (op & 0b110000) == 0b100000:
                    return new B.A1(rawInstruction);

                default:
                    throw new NotImplementedException();
            }
        }

        // A5.2.3 Data-processing (immediate)
        private static Instruction DecodeDataProcessingImmediate(uint rawInstruction, uint op)
        {
            var rn = (rawInstruction >> 16) & 0b1111;

            switch (op)
            {
                case 0b01000:
                case 0b01001:
                    switch (rn)
                    {
                        case 0b1111:
                            throw new NotImplementedException();

                        default:
                            return new AddImmediate.A1(rawInstruction, rn);
                    }

                case 0b11010:
                case 0b11011:
                    return new MovImmediate.A1(rawInstruction);

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
