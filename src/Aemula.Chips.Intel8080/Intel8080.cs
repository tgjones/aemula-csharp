using System;

namespace Aemula.Chips.Intel8080
{
    public class Intel8080
    {
        private readonly byte[] _ram;

        // Registers
        public ushort PC;
        public ushort SP;
        public byte A;
        public byte B;
        public byte C;
        public byte D;
        public byte E;
        public byte H;
        public byte L;

        // Flags
        public bool Zero;
        public bool Carry;
        public bool AuxiliaryCarry;
        public bool Sign;
        public bool Parity;

        public Intel8080(byte[] ram)
        {
            _ram = ram;
        }

        public void Cycle()
        {
            var opcode = _ram[PC++];

            switch (opcode)
            {
                // MOV A,M
                case 0b01111110:
                    A = _ram[(H << 8) | L];
                    break;

                // PUSH D - Push Data Onto Stack
                case 0b11010101:
                    _ram[--SP] = D;
                    _ram[--SP] = E;
                    break;

                // LXI B
                case 0b00000001:
                    C = _ram[PC++];
                    B = _ram[PC++];
                    break;

                // LXI D
                case 0b00010001:
                    E = _ram[PC++];
                    D = _ram[PC++];
                    break;

                // LXI H
                case 0b00100001:
                    L = _ram[PC++];
                    H = _ram[PC++];
                    break;

                // LXI SP
                case 0b00110001:
                    SP = _ram[PC++];
                    SP |= (ushort)(_ram[PC++] << 8);
                    break;

                // CPI - Compare Immediate With Accumulator
                //case 0b11111110:
                //    break;

                // JMP - Jump
                case 0b11000011:
                    Jump(true);
                    break;

                // JZ - Jump If Zero
                case 0b11001010:
                    Jump(Zero);
                    break;

                // JZ - Jump If Not Zero
                case 0b11000010:
                    Jump(!Zero);
                    break;

                // JC - Jump If Carry
                case 0b11011010:
                    Jump(Carry);
                    break;

                // JNC - Jump If No Carry
                case 0b11010010:
                    Jump(!Carry);
                    break;

                // JPO - Jump If Parity Odd
                case 0b11100010:
                    Jump(!Parity);
                    break;

                // JPE - Jump If Parity Even
                case 0b11101010:
                    Jump(Parity);
                    break;

                // JP - Jump If Positive
                case 0b11110010:
                    Jump(!Sign);
                    break;

                // JM - Jump If Minus
                case 0b11111010:
                    Jump(Sign);
                    break;

                // ADI - Add Immediate To Accumulator
                case 0b11000110:
                    Add(_ram[PC++]);
                    break;

                // CALL - Call
                case 0b11001101:
                    Call(true);
                    break;

                // ANI - And Immediate With Accumulator
                case 0b11100110:
                    A &= _ram[PC++];
                    Carry = false;
                    SetZeroBit();
                    SetSignBit();
                    SetParityBit();
                    break;

                // CPI - Compare Immediate With Accumulator
                case 0b11111110:
                    Subtract(A - _ram[PC++]);
                    break;

                default:
                    throw new InvalidOperationException($"Opcode {opcode:X2} not supported");
            }
        }

        private void Add(byte operand)
        {
            var newValue = A + operand;

            Carry = newValue > byte.MaxValue;

            // TODO: There's probably a better way of doing this.
            AuxiliaryCarry = (A & 0xF) + (operand & 0xF) > 0xF;

            A = (byte)newValue;

            SetZeroBit();
            SetSignBit();
            SetParityBit();
        }

        private byte Subtract(byte operand)
        {
            var newValue = A - operand;

            var newValueByte = (byte)newValue;

            Zero = newValueByte == 0;

            return newValueByte;
        }

        private void SetZeroBit() => Zero = A == 0;
        private void SetSignBit() => Sign = (A & 0x80) != 0;
        private void SetParityBit() => Parity = (System.Runtime.Intrinsics.X86.Popcnt.PopCount(A) % 2) == 0;

        private void Jump(bool condition)
        {
            var addressLo = _ram[PC++];
            var addressHi = _ram[PC++];

            if (condition)
            {
                PC = (ushort)((addressHi << 8) | addressLo);
            }
        }

        private void Call(bool condition)
        {
            var addressLo = _ram[PC++];
            var addressHi = _ram[PC++];

            if (condition)
            {
                _ram[--SP] = (byte)(PC >> 8);
                _ram[--SP] = (byte)(PC & 0xFF);

                PC = (ushort)((addressHi << 8) | addressLo);
            }
        }
    }
}
