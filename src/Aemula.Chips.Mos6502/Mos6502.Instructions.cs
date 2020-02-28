using System;

namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        /// <summary>
        /// ADC - Add with Carry
        /// </summary>
        private void Adc(byte value)
        {
            DoAdc(value);
        }

        /// <summary>
        /// AND - Logical AND
        /// </summary>
        private void And(byte value)
        {
            A = SetZeroNegativeFlags(A & value);
        }

        /// <summary>
        /// ASL - Arithmetic Shift Left
        /// </summary>
        private byte Asl(byte value)
        {
            P.C = (value & 0x80) == 0x80;
            return SetZeroNegativeFlags(value << 1);
        }

        /// <summary>
        /// BIT - Bit Test
        /// </summary>
        private void Bit(byte value)
        {
            P.Z = (A & value) == 0;
            P.V = (value & 0x40) == 0x40;
            P.N = (value & 0x80) == 0x80;
        }

        /// <summary>
        /// BRK - Force Interrupt
        /// </summary>
        private sealed class Break : InstructionImplementation
        {
            private int _state;
            private ushort _address;

            public Break()
                : base("BRK")
            {
            }

            public override InstructionCycleResult Cycle(Mos6502 cpu)
            {
                switch (_state)
                {
                    case 0:
                        cpu.ReadMemory(cpu.PC);
                        cpu.PC++;
                        _state = 1;
                        return InstructionCycleResult.NotFinished;

                    case 1:
                        cpu.Push((byte)(cpu.PC >> 8));
                        _state = 2;
                        return InstructionCycleResult.NotFinished;

                    case 2:
                        cpu.Push((byte)(cpu.PC & 0xFF));
                        _state = 3;
                        return InstructionCycleResult.NotFinished;

                    case 3:
                        cpu.Push(cpu.P.AsByte(true));
                        cpu.P.I = true; // Disable interrupts
                        _state = 4;
                        return InstructionCycleResult.NotFinished;

                    case 4:
                        _address = cpu.ReadMemory(IrqVector);
                        _state = 5;
                        return InstructionCycleResult.NotFinished;

                    case 5:
                        _address |= (ushort)(cpu.ReadMemory(IrqVector + 1) << 8);
                        cpu.PC = _address;
                        return InstructionCycleResult.Finished;

                    default:
                        throw new InvalidOperationException();
                }
            }

            public override void Reset()
            {
                _state = 0;
            }
        }

        /// <summary>
        /// CMP - Compare
        /// </summary>
        private void Cmp(byte value)
        {
            SetZeroNegativeFlags(A - value);
            P.C = A >= value;
        }

        /// <summary>
        /// CPX - Compare X Register
        /// </summary>
        private void Cpx(byte value)
        {
            SetZeroNegativeFlags(X - value);
            P.C = X >= value;
        }

        /// <summary>
        /// CPY - Compare Y Register
        /// </summary>
        private void Cpy(byte value)
        {
            SetZeroNegativeFlags(Y - value);
            P.C = Y >= value;
        }

        /// <summary>
        /// DCP - Decrement Memory then Compare (undocumented)
        /// </summary>
        private byte Dcp(byte value)
        {
            var result = Dec(value);
            Cmp(result);
            return result;
        }

        /// <summary>
        /// DEC - Decrement Memory
        /// </summary>
        private byte Dec(byte value)
        {
            return SetZeroNegativeFlags(value - 1);
        }

        /// <summary>
        /// DEX - Decrement X Register
        /// </summary>
        private void Dex()
        {
            X = SetZeroNegativeFlags(X - 1);
        }

        /// <summary>
        /// DEY - Decrement Y Register
        /// </summary>
        private void Dey()
        {
            Y = SetZeroNegativeFlags(Y - 1);
        }

        /// <summary>
        /// EOR - Exclusive OR
        /// </summary>
        private void Eor(byte value)
        {
            A = SetZeroNegativeFlags(A ^ value);
        }

        /// <summary>
        /// INC - Increment Memory
        /// </summary>
        private byte Inc(byte value)
        {
            return SetZeroNegativeFlags(value + 1);
        }

        /// <summary>
        /// INX - Increment X Register
        /// </summary>
        private void Inx()
        {
            X = SetZeroNegativeFlags(X + 1);
        }

        /// <summary>
        /// INY - Increment Y Register
        /// </summary>
        private void Iny()
        {
            Y = SetZeroNegativeFlags(Y + 1);
        }

        /// <summary>
        /// ISB (also known as ISC) - Increment Memory then Subtract (undocumented)
        /// </summary>
        private byte Isb(byte value)
        {
            var result = Inc(value);
            Sbc(result);
            return result;
        }

        /// <summary>
        /// JMP - Jump
        /// </summary>
        private sealed class JmpAbsolute : InstructionImplementation
        {
            private int _state;
            private ushort _address;

            public override byte InstructionSizeInBytes => 3;

            public JmpAbsolute()
                : base("JMP")
            {
            }

            public override InstructionCycleResult Cycle(Mos6502 cpu)
            {
                switch (_state)
                {
                    case 0:
                        _address = cpu.ReadMemory(cpu.PC);
                        cpu.PC++;
                        _state = 1;
                        return InstructionCycleResult.NotFinished;

                    case 1:
                        _address |= (ushort)(cpu.ReadMemory(cpu.PC) << 8);
                        cpu.PC = _address;
                        return InstructionCycleResult.Finished;

                    default:
                        throw new InvalidOperationException();
                }
            }

            public override void Reset()
            {
                _state = 0;
            }
        }

        /// <summary>
        /// JMP - Jump
        /// </summary>
        private sealed class JmpIndirect : InstructionImplementation
        {
            private int _state;
            private ushort _indirectAddress;
            private ushort _address;

            public override byte InstructionSizeInBytes => 3;

            public JmpIndirect()
                : base("JMP")
            {
            }

            public override InstructionCycleResult Cycle(Mos6502 cpu)
            {
                switch (_state)
                {
                    case 0:
                        _indirectAddress = cpu.ReadMemory(cpu.PC);
                        cpu.PC++;
                        _state = 1;
                        return InstructionCycleResult.NotFinished;

                    case 1:
                        _indirectAddress |= (ushort)(cpu.ReadMemory(cpu.PC) << 8);
                        cpu.PC++;
                        _state = 2;
                        return InstructionCycleResult.NotFinished;

                    case 2:
                        _address = cpu.ReadMemory(_indirectAddress);
                        _state = 3;
                        return InstructionCycleResult.NotFinished;

                    case 3:
                        if ((_indirectAddress & 0xFF) == 0xFF) // 6502 had a bug when crossing a page boundary, which we replicate here.
                        {
                            _address |= (ushort)(cpu.ReadMemory((ushort)(_indirectAddress + 1 + 0xFF00)) << 8);
                        }
                        else
                        {
                            _address |= (ushort)(cpu.ReadMemory((ushort)(_indirectAddress + 1)) << 8);
                        }
                        cpu.PC = _address;
                        return InstructionCycleResult.Finished;

                    default:
                        throw new InvalidOperationException();
                }
            }

            public override void Reset()
            {
                _state = 0;
            }
        }

        /// <summary>
        /// JSR - Jump to Subroutine
        /// </summary>
        private sealed class Jsr : InstructionImplementation
        {
            private int _state;
            private ushort _address;

            public override byte InstructionSizeInBytes => 3;

            public Jsr()
                : base("JSR")
            {
            }

            public override InstructionCycleResult Cycle(Mos6502 cpu)
            {
                switch (_state)
                {
                    case 0: // Read low byte of address.
                        _address = cpu.ReadMemory(cpu.PC);
                        cpu.PC++;
                        _state = 1;
                        return InstructionCycleResult.NotFinished;

                    case 1: // Ignored read on stack.
                        cpu.ReadMemory((ushort)(0x100 + cpu.SP));
                        _state = 2;
                        return InstructionCycleResult.NotFinished;

                    case 2: // Push PCH to stack.
                        cpu.Push((byte)(cpu.PC >> 8));
                        _state = 3;
                        return InstructionCycleResult.NotFinished;

                    case 3: // Push PCL to stack.
                        cpu.Push((byte)(cpu.PC & 0xFF));
                        _state = 4;
                        return InstructionCycleResult.NotFinished;

                    case 4: // Read high byte of address, set PC to address.
                        _address |= (ushort)(cpu.ReadMemory(cpu.PC) << 8);
                        cpu.PC = _address;
                        return InstructionCycleResult.Finished;

                    default:
                        throw new InvalidOperationException();
                }
            }

            public override void Reset()
            {
                _state = 0;
            }
        }

        /// <summary>
        /// LAX - Load A and X Registers (undocumented)
        /// </summary>
        private void Lax(byte value)
        {
            Lda(value);
            Ldx(value);
        }

        /// <summary>
        /// LDA - Load Accumulator
        /// </summary>
        private void Lda(byte value)
        {
            A = SetZeroNegativeFlags(value);
        }

        /// <summary>
        /// LDX - Load X Register
        /// </summary>
        private void Ldx(byte value)
        {
            X = SetZeroNegativeFlags(value);
        }

        /// <summary>
        /// LDY - Load Y Register
        /// </summary>
        private void Ldy(byte value)
        {
            Y = SetZeroNegativeFlags(value);
        }

        /// <summary>
        /// LSR - Logical Shift Right
        /// </summary>
        private byte Lsr(byte value)
        {
            P.C = (value & 0x1) == 0x1;
            return SetZeroNegativeFlags(value >> 1);
        }

        /// <summary>
        /// NOP
        /// </summary>
        private void Nop() { }

        /// <summary>
        /// NOP for undocumented addressing modes
        /// </summary>
        private void Nop(byte value) { }

        /// <summary>
        /// ORA - Logical Inclusive OR
        /// </summary>
        private void Ora(byte value)
        {
            A = SetZeroNegativeFlags(A | value);
        }

        /// <summary>
        /// PHA - Push Accumulator
        /// </summary>
        private sealed class Pha : InstructionImplementation
        {
            private int _state;

            public Pha()
                : base("PHA")
            {
            }

            public override InstructionCycleResult Cycle(Mos6502 cpu)
            {
                switch (_state)
                {
                    case 0:
                        cpu.ReadMemory(cpu.PC); // Spurious read
                        _state = 1;
                        return InstructionCycleResult.NotFinished;

                    case 1:
                        cpu.WriteMemory((ushort)(0x100 + cpu.SP), cpu.A);
                        cpu.SP--;
                        return InstructionCycleResult.Finished;

                    default:
                        throw new InvalidOperationException();
                }
            }

            public override void Reset()
            {
                _state = 0;
            }
        }

        /// <summary>
        /// PHP - Push Processor Status
        /// </summary>
        private sealed class Php : InstructionImplementation
        {
            private int _state;

            public Php()
                : base("PHP")
            {
            }

            public override InstructionCycleResult Cycle(Mos6502 cpu)
            {
                switch (_state)
                {
                    case 0:
                        cpu.ReadMemory(cpu.PC); // Spurious read
                        _state = 1;
                        return InstructionCycleResult.NotFinished;

                    case 1:
                        cpu.WriteMemory((ushort)(0x100 + cpu.SP), cpu.P.AsByte(true));
                        cpu.SP--;
                        return InstructionCycleResult.Finished;

                    default:
                        throw new InvalidOperationException();
                }
            }

            public override void Reset()
            {
                _state = 0;
            }
        }

        /// <summary>
        /// PLA - Pull Accumulator
        /// </summary>
        private sealed class Pla : InstructionImplementation
        {
            private int _state;

            public Pla()
                : base("PLA")
            {
            }

            public override InstructionCycleResult Cycle(Mos6502 cpu)
            {
                switch (_state)
                {
                    case 0:
                        cpu.ReadMemory(cpu.PC); // Spurious read
                        _state = 1;
                        return InstructionCycleResult.NotFinished;

                    case 1:
                        cpu.ReadMemory((ushort)(0x100 + cpu.SP));
                        cpu.SP++;
                        _state = 2;
                        return InstructionCycleResult.NotFinished;

                    case 2:
                        cpu.A = cpu.SetZeroNegativeFlags(cpu.ReadMemory((ushort)(0x100 + cpu.SP)));
                        return InstructionCycleResult.Finished;

                    default:
                        throw new InvalidOperationException();
                }
            }

            public override void Reset()
            {
                _state = 0;
            }
        }

        /// <summary>
        /// PLP - Pull Processor Status
        /// </summary>
        private sealed class Plp : InstructionImplementation
        {
            private int _state;

            public Plp()
                : base("PLP")
            {
            }

            public override InstructionCycleResult Cycle(Mos6502 cpu)
            {
                switch (_state)
                {
                    case 0:
                        cpu.ReadMemory(cpu.PC); // Spurious read
                        _state = 1;
                        return InstructionCycleResult.NotFinished;

                    case 1:
                        cpu.ReadMemory((ushort)(0x100 + cpu.SP));
                        cpu.SP++;
                        _state = 2;
                        return InstructionCycleResult.NotFinished;

                    case 2:
                        cpu.P.SetFromByte(cpu.SetZeroNegativeFlags(cpu.ReadMemory((ushort)(0x100 + cpu.SP))));
                        return InstructionCycleResult.Finished;

                    default:
                        throw new InvalidOperationException();
                }
            }

            public override void Reset()
            {
                _state = 0;
            }
        }

        /// <summary>
        /// RLA - ROL + AND (undocumented)
        /// </summary>
        private byte Rla(byte value)
        {
            var result = Rol(value);
            And(result);
            return result;
        }

        /// <summary>
        /// ROL - Rotate Left
        /// </summary>
        private byte Rol(byte value)
        {
            var temp = (byte)(P.C ? 1 : 0);
            P.C = (value & 0x80) == 0x80;
            return SetZeroNegativeFlags((value << 1) | temp);
        }

        /// <summary>
        /// ROR - Rotate Right
        /// </summary>
        private byte Ror(byte value)
        {
            var temp = (byte)((P.C ? 1 : 0) << 7);
            P.C = (value & 0x1) == 0x1;
            return SetZeroNegativeFlags((value >> 1) | temp);
        }

        /// <summary>
        /// RRA - ROR + ADC (undocumented)
        /// </summary>
        private byte Rra(byte value)
        {
            var result = Ror(value);
            Adc(result);
            return result;
        }

        /// <summary>
        /// RTI - Return from Interrupt
        /// </summary>
        private sealed class Rti : InstructionImplementation
        {
            private int _state;
            private ushort _address;

            public Rti()
                : base("RTI")
            {
            }

            public override InstructionCycleResult Cycle(Mos6502 cpu)
            {
                switch (_state)
                {
                    case 0: // Read next instruction byte and throw it away.
                        cpu.ReadMemory(cpu.PC);
                        _state = 1;
                        return InstructionCycleResult.NotFinished;

                    case 1: // Ignored read on stack, and increment SP.
                        cpu.ReadMemory((ushort)(0x100 + cpu.SP));
                        cpu.SP++;
                        _state = 2;
                        return InstructionCycleResult.NotFinished;

                    case 2: // Pull P from stack, and increment SP.
                        cpu.P.SetFromByte(cpu.ReadMemory((ushort)(0x100 + cpu.SP)));
                        cpu.SP++;
                        _state = 3;
                        return InstructionCycleResult.NotFinished;

                    case 3: // Pull PCL from stack, and increment SP.
                        _address = cpu.ReadMemory((ushort)(0x100 + cpu.SP));
                        cpu.SP++;
                        _state = 4;
                        return InstructionCycleResult.NotFinished;

                    case 4: // Pull PCH from stack.
                        _address |= (ushort)(cpu.ReadMemory((ushort)(0x100 + cpu.SP)) << 8);
                        cpu.PC = _address;
                        return InstructionCycleResult.Finished;

                    default:
                        throw new InvalidOperationException();
                }
            }

            public override void Reset()
            {
                _state = 0;
            }
        }

        /// <summary>
        /// RTS - Return from Subroutine
        /// </summary>
        private sealed class Rts : InstructionImplementation
        {
            private int _state;
            private ushort _address;

            public Rts()
                : base("RTS")
            {
            }

            public override InstructionCycleResult Cycle(Mos6502 cpu)
            {
                switch (_state)
                {
                    case 0: // Read next instruction byte and throw it away.
                        cpu.ReadMemory(cpu.PC);
                        _state = 1;
                        return InstructionCycleResult.NotFinished;

                    case 1: // Ignored read on stack, and increment SP.
                        cpu.ReadMemory((ushort)(0x100 + cpu.SP));
                        cpu.SP++;
                        _state = 2;
                        return InstructionCycleResult.NotFinished;

                    case 2: // Pull PCL from stack, and increment SP.
                        _address = cpu.ReadMemory((ushort)(0x100 + cpu.SP));
                        cpu.SP++;
                        _state = 3;
                        return InstructionCycleResult.NotFinished;

                    case 3: // Pull PCH from stack.
                        _address |= (ushort)(cpu.ReadMemory((ushort)(0x100 + cpu.SP)) << 8);
                        cpu.PC = _address;
                        _state = 4;
                        return InstructionCycleResult.NotFinished;

                    case 4: // Increment PC
                        cpu.ReadMemory(cpu.PC);
                        cpu.PC++; // Because we pushed PC(next) - 1
                        return InstructionCycleResult.Finished;

                    default:
                        throw new InvalidOperationException();
                }
            }

            public override void Reset()
            {
                _state = 0;
            }
        }

        /// <summary>
        /// SAX - Store Accumulator and X (undocumented)
        /// </summary>
        private byte Sax()
        {
            return (byte)(A & X);
        }

        /// <summary>
        /// SBC - Subtract with Carry
        /// </summary>
        private void Sbc(byte value)
        {
            DoSbc(value);
        }

        /// <summary>
        /// SLO - ASL + ORA (undocumented)
        /// </summary>
        private byte Slo(byte value)
        {
            var result = Asl(value);
            Ora(result);
            return result;
        }

        /// <summary>
        /// SRE - LSR + EOR (undocumented)
        /// </summary>
        private byte Sre(byte value)
        {
            var result = Lsr(value);
            Eor(result);
            return result;
        }

        /// <summary>
        /// STA - Store Accumulator
        /// </summary>
        private byte Sta()
        {
            return A;
        }

        /// <summary>
        /// STX - Store X Register
        /// </summary>
        private byte Stx()
        {
            return X;
        }

        /// <summary>
        /// STY - Store Y Register
        /// </summary>
        private byte Sty()
        {
            return Y;
        }

        /// <summary>
        /// TAX - Transfer Accumulator to X
        /// </summary>
        private void Tax()
        {
            X = SetZeroNegativeFlags(A);
        }

        /// <summary>
        /// TAY - Transfer Accumulator to Y
        /// </summary>
        private void Tay()
        {
            Y = SetZeroNegativeFlags(A);
        }

        /// <summary>
        /// TSX - Transfer Stack Pointer to X
        /// </summary>
        private void Tsx()
        {
            X = SetZeroNegativeFlags(SP);
        }

        /// <summary>
        /// TXA - Transfer X to Accumulator
        /// </summary>
        private void Txa()
        {
            A = SetZeroNegativeFlags(X);
        }

        /// <summary>
        /// TXS - Transfer X to Stack Pointer
        /// </summary>
        private void Txs()
        {
            SP = X;
        }

        /// <summary>
        /// TYA - Transfer Y to Accumulator
        /// </summary>
        private void Tya()
        {
            A = SetZeroNegativeFlags(Y);
        }
    }
}