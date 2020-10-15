using System;
using System.Diagnostics;

namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        private void ExecuteInstruction()
        {
            int tempInt32 = 0;
            
            switch ((_ir << 3) | _tr)
            {
                // BRK 
                case (0x00 << 3) | 0:
                    Address = PC;
                    break;
                case (0x00 << 3) | 1:
                    if ((_brkFlags & (BrkFlags.Irq | BrkFlags.Nmi)) == 0)
                    {
                        PC += 1;
                    }
                    Address.Hi = 0x01;
                    Address.Lo = SP--;
                    _data = PC.Hi;
                    _rw = (_brkFlags & BrkFlags.Reset) != 0;
                    break;
                case (0x00 << 3) | 2:
                    Address.Lo = SP--;
                    _data = PC.Lo;
                    _rw = (_brkFlags & BrkFlags.Reset) != 0;
                    break;
                case (0x00 << 3) | 3:
                    Address.Lo = SP--;
                    _data = P.AsByte(_brkFlags == BrkFlags.None);
                    _ad.Hi = 0xFF;
                    if ((_brkFlags & BrkFlags.Reset) != 0)
                    {
                        _ad.Lo = 0xFC;
                    }
                    else
                    {
                        _rw = false;
                        _ad.Lo = (_brkFlags & BrkFlags.Nmi) != 0
                            ? (byte)0xFA
                            : (byte)0xFE;
                    }
                    break;
                case (0x00 << 3) | 4:
                    Address = _ad;
                    _ad.Lo += 1;
                    P.I = true;
                    _brkFlags = BrkFlags.None;
                    break;
                case (0x00 << 3) | 5:
                    Address.Lo = _ad.Lo;
                    _ad.Lo = _data;
                    break;
                case (0x00 << 3) | 6:
                    PC.Hi = _data;
                    PC.Lo = _ad.Lo;
                    FetchNextInstruction();
                    break;
                case (0x00 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ORA (zp,X) - Logical Inclusive OR
                case (0x01 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x01 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x01 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x01 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x01 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x01 << 3) | 5:
                    A = P.SetZeroNegativeFlags((byte)(A | _data));
                    FetchNextInstruction();
                    break;
                case (0x01 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x01 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JAM invalid
                case (0x02 << 3) | 0:
                    Address = PC;
                    break;
                case (0x02 << 3) | 1:
                    Address.Hi = 0xFF;
                    Address.Lo = 0xFF;
                    _data = 0xFF;
                    _ir--;
                    FetchNextInstruction();
                    break;
                case (0x02 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x02 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x02 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x02 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x02 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x02 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SLO (zp,X)
                case (0x03 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x03 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x03 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x03 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x03 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x03 << 3) | 5:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x03 << 3) | 6:
                    Slo();
                    break;
                case (0x03 << 3) | 7:
                    FetchNextInstruction();
                    break;

                // NOP zp
                case (0x04 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x04 << 3) | 1:
                    Address = _data;
                    break;
                case (0x04 << 3) | 2:
                    FetchNextInstruction();
                    break;
                case (0x04 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x04 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x04 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x04 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x04 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ORA zp - Logical Inclusive OR
                case (0x05 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x05 << 3) | 1:
                    Address = _data;
                    break;
                case (0x05 << 3) | 2:
                    A = P.SetZeroNegativeFlags((byte)(A | _data));
                    FetchNextInstruction();
                    break;
                case (0x05 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x05 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x05 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x05 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x05 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ASL zp
                case (0x06 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x06 << 3) | 1:
                    Address = _data;
                    break;
                case (0x06 << 3) | 2:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x06 << 3) | 3:
                    Asl();
                    break;
                case (0x06 << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x06 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x06 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x06 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SLO zp
                case (0x07 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x07 << 3) | 1:
                    Address = _data;
                    break;
                case (0x07 << 3) | 2:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x07 << 3) | 3:
                    Slo();
                    break;
                case (0x07 << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x07 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x07 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x07 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // PHP 
                case (0x08 << 3) | 0:
                    Address = PC;
                    break;
                case (0x08 << 3) | 1:
                    Address.Hi = 0x01;
                    Address.Lo = SP--;
                    _data = P.AsByte(true);
                    _rw = false;
                    break;
                case (0x08 << 3) | 2:
                    FetchNextInstruction();
                    break;
                case (0x08 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x08 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x08 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x08 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x08 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ORA # - Logical Inclusive OR
                case (0x09 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x09 << 3) | 1:
                    A = P.SetZeroNegativeFlags((byte)(A | _data));
                    FetchNextInstruction();
                    break;
                case (0x09 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x09 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x09 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x09 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x09 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x09 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ASL 
                case (0x0A << 3) | 0:
                    Address = PC;
                    break;
                case (0x0A << 3) | 1:
                    Asla();
                    FetchNextInstruction();
                    break;
                case (0x0A << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x0A << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x0A << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x0A << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x0A << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x0A << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ANC #
                case (0x0B << 3) | 0:
                    Address = PC++;
                    break;
                case (0x0B << 3) | 1:
                    A &= _data;
                    P.SetZeroNegativeFlags(A);
                    P.C = (A & 0x80) != 0;
                    FetchNextInstruction();
                    break;
                case (0x0B << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x0B << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x0B << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x0B << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x0B << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x0B << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP abs
                case (0x0C << 3) | 0:
                    Address = PC++;
                    break;
                case (0x0C << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x0C << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x0C << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0x0C << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x0C << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x0C << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x0C << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ORA abs - Logical Inclusive OR
                case (0x0D << 3) | 0:
                    Address = PC++;
                    break;
                case (0x0D << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x0D << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x0D << 3) | 3:
                    A = P.SetZeroNegativeFlags((byte)(A | _data));
                    FetchNextInstruction();
                    break;
                case (0x0D << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x0D << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x0D << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x0D << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ASL abs
                case (0x0E << 3) | 0:
                    Address = PC++;
                    break;
                case (0x0E << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x0E << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x0E << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x0E << 3) | 4:
                    Asl();
                    break;
                case (0x0E << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x0E << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x0E << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SLO abs
                case (0x0F << 3) | 0:
                    Address = PC++;
                    break;
                case (0x0F << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x0F << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x0F << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x0F << 3) | 4:
                    Slo();
                    break;
                case (0x0F << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x0F << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x0F << 3) | 7:
                    Debug.Assert(false);
                    break;

                // BPL #
                case (0x10 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x10 << 3) | 1:
                    Address = PC;
                    _ad = PC + (sbyte)_data;
                    if (P.N != false)
                    {
                        FetchNextInstruction();
                    }
                    break;
                case (0x10 << 3) | 2:
                    Address.Lo = _ad.Lo;
                    if (_ad.Hi == PC.Hi)
                    {
                        PC = _ad;
                        FetchNextInstruction();
                    }
                    break;
                case (0x10 << 3) | 3:
                    PC = _ad;
                    FetchNextInstruction();
                    break;
                case (0x10 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x10 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x10 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x10 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ORA (zp),Y - Logical Inclusive OR
                case (0x11 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x11 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x11 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x11 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0x11 << 3) | 4:
                    Address = _ad + Y;
                    break;
                case (0x11 << 3) | 5:
                    A = P.SetZeroNegativeFlags((byte)(A | _data));
                    FetchNextInstruction();
                    break;
                case (0x11 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x11 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JAM invalid
                case (0x12 << 3) | 0:
                    Address = PC;
                    break;
                case (0x12 << 3) | 1:
                    Address.Hi = 0xFF;
                    Address.Lo = 0xFF;
                    _data = 0xFF;
                    _ir--;
                    FetchNextInstruction();
                    break;
                case (0x12 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x12 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x12 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x12 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x12 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x12 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SLO (zp),Y
                case (0x13 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x13 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x13 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x13 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0x13 << 3) | 4:
                    Address = _ad + Y;
                    break;
                case (0x13 << 3) | 5:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x13 << 3) | 6:
                    Slo();
                    break;
                case (0x13 << 3) | 7:
                    FetchNextInstruction();
                    break;

                // NOP zp,X
                case (0x14 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x14 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x14 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x14 << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0x14 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x14 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x14 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x14 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ORA zp,X - Logical Inclusive OR
                case (0x15 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x15 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x15 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x15 << 3) | 3:
                    A = P.SetZeroNegativeFlags((byte)(A | _data));
                    FetchNextInstruction();
                    break;
                case (0x15 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x15 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x15 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x15 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ASL zp,X
                case (0x16 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x16 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x16 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x16 << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x16 << 3) | 4:
                    Asl();
                    break;
                case (0x16 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x16 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x16 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SLO zp,X
                case (0x17 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x17 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x17 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x17 << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x17 << 3) | 4:
                    Slo();
                    break;
                case (0x17 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x17 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x17 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CLC 
                case (0x18 << 3) | 0:
                    Address = PC;
                    break;
                case (0x18 << 3) | 1:
                    P.C = false;
                    FetchNextInstruction();
                    break;
                case (0x18 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x18 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x18 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x18 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x18 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x18 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ORA abs,Y - Logical Inclusive OR
                case (0x19 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x19 << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x19 << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0x19 << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0x19 << 3) | 4:
                    A = P.SetZeroNegativeFlags((byte)(A | _data));
                    FetchNextInstruction();
                    break;
                case (0x19 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x19 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x19 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP 
                case (0x1A << 3) | 0:
                    Address = PC;
                    break;
                case (0x1A << 3) | 1:
                    FetchNextInstruction();
                    break;
                case (0x1A << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x1A << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x1A << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x1A << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x1A << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x1A << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SLO abs,Y
                case (0x1B << 3) | 0:
                    Address = PC++;
                    break;
                case (0x1B << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x1B << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0x1B << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0x1B << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x1B << 3) | 5:
                    Slo();
                    break;
                case (0x1B << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0x1B << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP abs,X
                case (0x1C << 3) | 0:
                    Address = PC++;
                    break;
                case (0x1C << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x1C << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    IncrementTimingRegisterIfNoPageCrossing(X);
                    break;
                case (0x1C << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x1C << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x1C << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x1C << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x1C << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ORA abs,X - Logical Inclusive OR
                case (0x1D << 3) | 0:
                    Address = PC++;
                    break;
                case (0x1D << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x1D << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    IncrementTimingRegisterIfNoPageCrossing(X);
                    break;
                case (0x1D << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x1D << 3) | 4:
                    A = P.SetZeroNegativeFlags((byte)(A | _data));
                    FetchNextInstruction();
                    break;
                case (0x1D << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x1D << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x1D << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ASL abs,X
                case (0x1E << 3) | 0:
                    Address = PC++;
                    break;
                case (0x1E << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x1E << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    break;
                case (0x1E << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x1E << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x1E << 3) | 5:
                    Asl();
                    break;
                case (0x1E << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0x1E << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SLO abs,X
                case (0x1F << 3) | 0:
                    Address = PC++;
                    break;
                case (0x1F << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x1F << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    break;
                case (0x1F << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x1F << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x1F << 3) | 5:
                    Slo();
                    break;
                case (0x1F << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0x1F << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JSR 
                case (0x20 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x20 << 3) | 1:
                    Address.Hi = 0x01;
                    Address.Lo = SP;
                    _ad.Lo = _data;
                    break;
                case (0x20 << 3) | 2:
                    Address.Lo = SP--;
                    _data = PC.Hi;
                    _rw = false;
                    break;
                case (0x20 << 3) | 3:
                    Address.Lo = SP--;
                    _data = PC.Lo;
                    _rw = false;
                    break;
                case (0x20 << 3) | 4:
                    Address = PC;
                    break;
                case (0x20 << 3) | 5:
                    PC.Hi = _data;
                    PC.Lo = _ad.Lo;
                    FetchNextInstruction();
                    break;
                case (0x20 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x20 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // AND (zp,X) - Logical AND
                case (0x21 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x21 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x21 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x21 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x21 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x21 << 3) | 5:
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    FetchNextInstruction();
                    break;
                case (0x21 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x21 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JAM invalid
                case (0x22 << 3) | 0:
                    Address = PC;
                    break;
                case (0x22 << 3) | 1:
                    Address.Hi = 0xFF;
                    Address.Lo = 0xFF;
                    _data = 0xFF;
                    _ir--;
                    FetchNextInstruction();
                    break;
                case (0x22 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x22 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x22 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x22 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x22 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x22 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RLA (zp,X) - ROL + AND (undocumented)
                case (0x23 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x23 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x23 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x23 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x23 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x23 << 3) | 5:
                    _data = RolHelper(_ad.Lo);
                    _rw = false;
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    break;
                case (0x23 << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0x23 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // BIT zp - Bit Test
                case (0x24 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x24 << 3) | 1:
                    Address = _data;
                    break;
                case (0x24 << 3) | 2:
                    P.Z = (A & _data) == 0;
                    P.V = (_data & 0x40) == 0x40;
                    P.N = (_data & 0x80) == 0x80;
                    FetchNextInstruction();
                    break;
                case (0x24 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x24 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x24 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x24 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x24 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // AND zp - Logical AND
                case (0x25 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x25 << 3) | 1:
                    Address = _data;
                    break;
                case (0x25 << 3) | 2:
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    FetchNextInstruction();
                    break;
                case (0x25 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x25 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x25 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x25 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x25 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ROL zp - Rotate Left
                case (0x26 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x26 << 3) | 1:
                    Address = _data;
                    break;
                case (0x26 << 3) | 2:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x26 << 3) | 3:
                    _data = RolHelper(_ad.Lo);
                    _rw = false;
                    break;
                case (0x26 << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x26 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x26 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x26 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RLA zp - ROL + AND (undocumented)
                case (0x27 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x27 << 3) | 1:
                    Address = _data;
                    break;
                case (0x27 << 3) | 2:
                    _data = RolHelper(_ad.Lo);
                    _rw = false;
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    break;
                case (0x27 << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0x27 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x27 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x27 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x27 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // PLP 
                case (0x28 << 3) | 0:
                    Address = PC;
                    break;
                case (0x28 << 3) | 1:
                    Address.Hi = 0x01;
                    Address.Lo = SP++;
                    break;
                case (0x28 << 3) | 2:
                    Address.Lo = SP;
                    break;
                case (0x28 << 3) | 3:
                    P.SetFromByte(P.SetZeroNegativeFlags(_data));
                    FetchNextInstruction();
                    break;
                case (0x28 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x28 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x28 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x28 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // AND # - Logical AND
                case (0x29 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x29 << 3) | 1:
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    FetchNextInstruction();
                    break;
                case (0x29 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x29 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x29 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x29 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x29 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x29 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ROL  - Rotate Left
                case (0x2A << 3) | 0:
                    Address = PC;
                    break;
                case (0x2A << 3) | 1:
                    A = RolHelper(A);
                    FetchNextInstruction();
                    break;
                case (0x2A << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x2A << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x2A << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x2A << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x2A << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x2A << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ANC #
                case (0x2B << 3) | 0:
                    Address = PC++;
                    break;
                case (0x2B << 3) | 1:
                    A &= _data;
                    P.SetZeroNegativeFlags(A);
                    P.C = (A & 0x80) != 0;
                    FetchNextInstruction();
                    break;
                case (0x2B << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x2B << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x2B << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x2B << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x2B << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x2B << 3) | 7:
                    Debug.Assert(false);
                    break;

                // BIT abs - Bit Test
                case (0x2C << 3) | 0:
                    Address = PC++;
                    break;
                case (0x2C << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x2C << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x2C << 3) | 3:
                    P.Z = (A & _data) == 0;
                    P.V = (_data & 0x40) == 0x40;
                    P.N = (_data & 0x80) == 0x80;
                    FetchNextInstruction();
                    break;
                case (0x2C << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x2C << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x2C << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x2C << 3) | 7:
                    Debug.Assert(false);
                    break;

                // AND abs - Logical AND
                case (0x2D << 3) | 0:
                    Address = PC++;
                    break;
                case (0x2D << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x2D << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x2D << 3) | 3:
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    FetchNextInstruction();
                    break;
                case (0x2D << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x2D << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x2D << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x2D << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ROL abs - Rotate Left
                case (0x2E << 3) | 0:
                    Address = PC++;
                    break;
                case (0x2E << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x2E << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x2E << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x2E << 3) | 4:
                    _data = RolHelper(_ad.Lo);
                    _rw = false;
                    break;
                case (0x2E << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x2E << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x2E << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RLA abs - ROL + AND (undocumented)
                case (0x2F << 3) | 0:
                    Address = PC++;
                    break;
                case (0x2F << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x2F << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x2F << 3) | 3:
                    _data = RolHelper(_ad.Lo);
                    _rw = false;
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    break;
                case (0x2F << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x2F << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x2F << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x2F << 3) | 7:
                    Debug.Assert(false);
                    break;

                // BMI #
                case (0x30 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x30 << 3) | 1:
                    Address = PC;
                    _ad = PC + (sbyte)_data;
                    if (P.N != true)
                    {
                        FetchNextInstruction();
                    }
                    break;
                case (0x30 << 3) | 2:
                    Address.Lo = _ad.Lo;
                    if (_ad.Hi == PC.Hi)
                    {
                        PC = _ad;
                        FetchNextInstruction();
                    }
                    break;
                case (0x30 << 3) | 3:
                    PC = _ad;
                    FetchNextInstruction();
                    break;
                case (0x30 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x30 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x30 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x30 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // AND (zp),Y - Logical AND
                case (0x31 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x31 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x31 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x31 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0x31 << 3) | 4:
                    Address = _ad + Y;
                    break;
                case (0x31 << 3) | 5:
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    FetchNextInstruction();
                    break;
                case (0x31 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x31 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JAM invalid
                case (0x32 << 3) | 0:
                    Address = PC;
                    break;
                case (0x32 << 3) | 1:
                    Address.Hi = 0xFF;
                    Address.Lo = 0xFF;
                    _data = 0xFF;
                    _ir--;
                    FetchNextInstruction();
                    break;
                case (0x32 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x32 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x32 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x32 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x32 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x32 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RLA (zp),Y - ROL + AND (undocumented)
                case (0x33 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x33 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x33 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x33 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0x33 << 3) | 4:
                    Address = _ad + Y;
                    break;
                case (0x33 << 3) | 5:
                    _data = RolHelper(_ad.Lo);
                    _rw = false;
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    break;
                case (0x33 << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0x33 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP zp,X
                case (0x34 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x34 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x34 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x34 << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0x34 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x34 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x34 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x34 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // AND zp,X - Logical AND
                case (0x35 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x35 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x35 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x35 << 3) | 3:
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    FetchNextInstruction();
                    break;
                case (0x35 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x35 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x35 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x35 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ROL zp,X - Rotate Left
                case (0x36 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x36 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x36 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x36 << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x36 << 3) | 4:
                    _data = RolHelper(_ad.Lo);
                    _rw = false;
                    break;
                case (0x36 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x36 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x36 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RLA zp,X - ROL + AND (undocumented)
                case (0x37 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x37 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x37 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x37 << 3) | 3:
                    _data = RolHelper(_ad.Lo);
                    _rw = false;
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    break;
                case (0x37 << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x37 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x37 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x37 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SLC 
                case (0x38 << 3) | 0:
                    Address = PC;
                    break;
                case (0x38 << 3) | 1:
                    P.C = true;
                    FetchNextInstruction();
                    break;
                case (0x38 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x38 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x38 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x38 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x38 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x38 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // AND abs,Y - Logical AND
                case (0x39 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x39 << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x39 << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0x39 << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0x39 << 3) | 4:
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    FetchNextInstruction();
                    break;
                case (0x39 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x39 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x39 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP 
                case (0x3A << 3) | 0:
                    Address = PC;
                    break;
                case (0x3A << 3) | 1:
                    FetchNextInstruction();
                    break;
                case (0x3A << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x3A << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x3A << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x3A << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x3A << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x3A << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RLA abs,Y - ROL + AND (undocumented)
                case (0x3B << 3) | 0:
                    Address = PC++;
                    break;
                case (0x3B << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x3B << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0x3B << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0x3B << 3) | 4:
                    _data = RolHelper(_ad.Lo);
                    _rw = false;
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    break;
                case (0x3B << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x3B << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x3B << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP abs,X
                case (0x3C << 3) | 0:
                    Address = PC++;
                    break;
                case (0x3C << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x3C << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    IncrementTimingRegisterIfNoPageCrossing(X);
                    break;
                case (0x3C << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x3C << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x3C << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x3C << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x3C << 3) | 7:
                    Debug.Assert(false);
                    break;

                // AND abs,X - Logical AND
                case (0x3D << 3) | 0:
                    Address = PC++;
                    break;
                case (0x3D << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x3D << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    IncrementTimingRegisterIfNoPageCrossing(X);
                    break;
                case (0x3D << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x3D << 3) | 4:
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    FetchNextInstruction();
                    break;
                case (0x3D << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x3D << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x3D << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ROL abs,X - Rotate Left
                case (0x3E << 3) | 0:
                    Address = PC++;
                    break;
                case (0x3E << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x3E << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    break;
                case (0x3E << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x3E << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x3E << 3) | 5:
                    _data = RolHelper(_ad.Lo);
                    _rw = false;
                    break;
                case (0x3E << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0x3E << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RLA abs,X - ROL + AND (undocumented)
                case (0x3F << 3) | 0:
                    Address = PC++;
                    break;
                case (0x3F << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x3F << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    break;
                case (0x3F << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x3F << 3) | 4:
                    _data = RolHelper(_ad.Lo);
                    _rw = false;
                    A = P.SetZeroNegativeFlags((byte)(A & _data));
                    break;
                case (0x3F << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x3F << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x3F << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RTI 
                case (0x40 << 3) | 0:
                    Address = PC;
                    break;
                case (0x40 << 3) | 1:
                    Address.Hi = 0x01;
                    Address.Lo = SP++;
                    break;
                case (0x40 << 3) | 2:
                    Address.Lo = SP++;
                    break;
                case (0x40 << 3) | 3:
                    Address.Lo = SP++;
                    P.SetFromByte(_data);
                    break;
                case (0x40 << 3) | 4:
                    Address.Lo = SP;
                    _ad.Lo = _data;
                    break;
                case (0x40 << 3) | 5:
                    PC.Hi = _data;
                    PC.Lo = _ad.Lo;
                    FetchNextInstruction();
                    break;
                case (0x40 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x40 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // EOR (zp,X) - Exclusive OR
                case (0x41 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x41 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x41 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x41 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x41 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x41 << 3) | 5:
                    A = P.SetZeroNegativeFlags((byte)(A ^ _data));
                    FetchNextInstruction();
                    break;
                case (0x41 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x41 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JAM invalid
                case (0x42 << 3) | 0:
                    Address = PC;
                    break;
                case (0x42 << 3) | 1:
                    Address.Hi = 0xFF;
                    Address.Lo = 0xFF;
                    _data = 0xFF;
                    _ir--;
                    FetchNextInstruction();
                    break;
                case (0x42 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x42 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x42 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x42 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x42 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x42 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SRE (zp,X)
                case (0x43 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x43 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x43 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x43 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x43 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x43 << 3) | 5:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x43 << 3) | 6:
                    Sre();
                    break;
                case (0x43 << 3) | 7:
                    FetchNextInstruction();
                    break;

                // NOP zp
                case (0x44 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x44 << 3) | 1:
                    Address = _data;
                    break;
                case (0x44 << 3) | 2:
                    FetchNextInstruction();
                    break;
                case (0x44 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x44 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x44 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x44 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x44 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // EOR zp - Exclusive OR
                case (0x45 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x45 << 3) | 1:
                    Address = _data;
                    break;
                case (0x45 << 3) | 2:
                    A = P.SetZeroNegativeFlags((byte)(A ^ _data));
                    FetchNextInstruction();
                    break;
                case (0x45 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x45 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x45 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x45 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x45 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LSR zp
                case (0x46 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x46 << 3) | 1:
                    Address = _data;
                    break;
                case (0x46 << 3) | 2:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x46 << 3) | 3:
                    Lsr();
                    break;
                case (0x46 << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x46 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x46 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x46 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SRE zp
                case (0x47 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x47 << 3) | 1:
                    Address = _data;
                    break;
                case (0x47 << 3) | 2:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x47 << 3) | 3:
                    Sre();
                    break;
                case (0x47 << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x47 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x47 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x47 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // PHA 
                case (0x48 << 3) | 0:
                    Address = PC;
                    break;
                case (0x48 << 3) | 1:
                    Address.Hi = 0x01;
                    Address.Lo = SP--;
                    _data = A;
                    _rw = false;
                    break;
                case (0x48 << 3) | 2:
                    FetchNextInstruction();
                    break;
                case (0x48 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x48 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x48 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x48 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x48 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // EOR # - Exclusive OR
                case (0x49 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x49 << 3) | 1:
                    A = P.SetZeroNegativeFlags((byte)(A ^ _data));
                    FetchNextInstruction();
                    break;
                case (0x49 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x49 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x49 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x49 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x49 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x49 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LSR 
                case (0x4A << 3) | 0:
                    Address = PC;
                    break;
                case (0x4A << 3) | 1:
                    Lsra();
                    FetchNextInstruction();
                    break;
                case (0x4A << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x4A << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x4A << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x4A << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x4A << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x4A << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ASR #
                case (0x4B << 3) | 0:
                    Address = PC++;
                    break;
                case (0x4B << 3) | 1:
                    Asr();
                    FetchNextInstruction();
                    break;
                case (0x4B << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x4B << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x4B << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x4B << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x4B << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x4B << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JMP abs
                case (0x4C << 3) | 0:
                    Address = PC++;
                    break;
                case (0x4C << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x4C << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    PC = Address;
                    FetchNextInstruction();
                    break;
                case (0x4C << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x4C << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x4C << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x4C << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x4C << 3) | 7:
                    Debug.Assert(false);
                    break;

                // EOR abs - Exclusive OR
                case (0x4D << 3) | 0:
                    Address = PC++;
                    break;
                case (0x4D << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x4D << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x4D << 3) | 3:
                    A = P.SetZeroNegativeFlags((byte)(A ^ _data));
                    FetchNextInstruction();
                    break;
                case (0x4D << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x4D << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x4D << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x4D << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LSR abs
                case (0x4E << 3) | 0:
                    Address = PC++;
                    break;
                case (0x4E << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x4E << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x4E << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x4E << 3) | 4:
                    Lsr();
                    break;
                case (0x4E << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x4E << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x4E << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SRE abs
                case (0x4F << 3) | 0:
                    Address = PC++;
                    break;
                case (0x4F << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x4F << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x4F << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x4F << 3) | 4:
                    Sre();
                    break;
                case (0x4F << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x4F << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x4F << 3) | 7:
                    Debug.Assert(false);
                    break;

                // BVC #
                case (0x50 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x50 << 3) | 1:
                    Address = PC;
                    _ad = PC + (sbyte)_data;
                    if (P.V != false)
                    {
                        FetchNextInstruction();
                    }
                    break;
                case (0x50 << 3) | 2:
                    Address.Lo = _ad.Lo;
                    if (_ad.Hi == PC.Hi)
                    {
                        PC = _ad;
                        FetchNextInstruction();
                    }
                    break;
                case (0x50 << 3) | 3:
                    PC = _ad;
                    FetchNextInstruction();
                    break;
                case (0x50 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x50 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x50 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x50 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // EOR (zp),Y - Exclusive OR
                case (0x51 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x51 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x51 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x51 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0x51 << 3) | 4:
                    Address = _ad + Y;
                    break;
                case (0x51 << 3) | 5:
                    A = P.SetZeroNegativeFlags((byte)(A ^ _data));
                    FetchNextInstruction();
                    break;
                case (0x51 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x51 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JAM invalid
                case (0x52 << 3) | 0:
                    Address = PC;
                    break;
                case (0x52 << 3) | 1:
                    Address.Hi = 0xFF;
                    Address.Lo = 0xFF;
                    _data = 0xFF;
                    _ir--;
                    FetchNextInstruction();
                    break;
                case (0x52 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x52 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x52 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x52 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x52 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x52 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SRE (zp),Y
                case (0x53 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x53 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x53 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x53 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0x53 << 3) | 4:
                    Address = _ad + Y;
                    break;
                case (0x53 << 3) | 5:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x53 << 3) | 6:
                    Sre();
                    break;
                case (0x53 << 3) | 7:
                    FetchNextInstruction();
                    break;

                // NOP zp,X
                case (0x54 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x54 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x54 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x54 << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0x54 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x54 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x54 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x54 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // EOR zp,X - Exclusive OR
                case (0x55 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x55 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x55 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x55 << 3) | 3:
                    A = P.SetZeroNegativeFlags((byte)(A ^ _data));
                    FetchNextInstruction();
                    break;
                case (0x55 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x55 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x55 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x55 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LSR zp,X
                case (0x56 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x56 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x56 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x56 << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x56 << 3) | 4:
                    Lsr();
                    break;
                case (0x56 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x56 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x56 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SRE zp,X
                case (0x57 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x57 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x57 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x57 << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x57 << 3) | 4:
                    Sre();
                    break;
                case (0x57 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x57 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x57 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CLI 
                case (0x58 << 3) | 0:
                    Address = PC;
                    break;
                case (0x58 << 3) | 1:
                    P.I = false;
                    FetchNextInstruction();
                    break;
                case (0x58 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x58 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x58 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x58 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x58 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x58 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // EOR abs,Y - Exclusive OR
                case (0x59 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x59 << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x59 << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0x59 << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0x59 << 3) | 4:
                    A = P.SetZeroNegativeFlags((byte)(A ^ _data));
                    FetchNextInstruction();
                    break;
                case (0x59 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x59 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x59 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP 
                case (0x5A << 3) | 0:
                    Address = PC;
                    break;
                case (0x5A << 3) | 1:
                    FetchNextInstruction();
                    break;
                case (0x5A << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x5A << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x5A << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x5A << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x5A << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x5A << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SRE abs,Y
                case (0x5B << 3) | 0:
                    Address = PC++;
                    break;
                case (0x5B << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x5B << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0x5B << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0x5B << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x5B << 3) | 5:
                    Sre();
                    break;
                case (0x5B << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0x5B << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP abs,X
                case (0x5C << 3) | 0:
                    Address = PC++;
                    break;
                case (0x5C << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x5C << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    IncrementTimingRegisterIfNoPageCrossing(X);
                    break;
                case (0x5C << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x5C << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x5C << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x5C << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x5C << 3) | 7:
                    Debug.Assert(false);
                    break;

                // EOR abs,X - Exclusive OR
                case (0x5D << 3) | 0:
                    Address = PC++;
                    break;
                case (0x5D << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x5D << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    IncrementTimingRegisterIfNoPageCrossing(X);
                    break;
                case (0x5D << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x5D << 3) | 4:
                    A = P.SetZeroNegativeFlags((byte)(A ^ _data));
                    FetchNextInstruction();
                    break;
                case (0x5D << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x5D << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x5D << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LSR abs,X
                case (0x5E << 3) | 0:
                    Address = PC++;
                    break;
                case (0x5E << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x5E << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    break;
                case (0x5E << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x5E << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x5E << 3) | 5:
                    Lsr();
                    break;
                case (0x5E << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0x5E << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SRE abs,X
                case (0x5F << 3) | 0:
                    Address = PC++;
                    break;
                case (0x5F << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x5F << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    break;
                case (0x5F << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x5F << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x5F << 3) | 5:
                    Sre();
                    break;
                case (0x5F << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0x5F << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RTS 
                case (0x60 << 3) | 0:
                    Address = PC;
                    break;
                case (0x60 << 3) | 1:
                    Address.Hi = 0x01;
                    Address.Lo = SP++;
                    break;
                case (0x60 << 3) | 2:
                    Address.Lo = SP++;
                    break;
                case (0x60 << 3) | 3:
                    Address.Lo = SP;
                    _ad.Lo = _data;
                    break;
                case (0x60 << 3) | 4:
                    PC.Hi = _data;
                    PC.Lo = _ad.Lo;
                    Address = PC++;
                    break;
                case (0x60 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x60 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x60 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ADC (zp,X)
                case (0x61 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x61 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x61 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x61 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x61 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x61 << 3) | 5:
                    Adc();
                    FetchNextInstruction();
                    break;
                case (0x61 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x61 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JAM invalid
                case (0x62 << 3) | 0:
                    Address = PC;
                    break;
                case (0x62 << 3) | 1:
                    Address.Hi = 0xFF;
                    Address.Lo = 0xFF;
                    _data = 0xFF;
                    _ir--;
                    FetchNextInstruction();
                    break;
                case (0x62 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x62 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x62 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x62 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x62 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x62 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RRA (zp,X)
                case (0x63 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x63 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x63 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x63 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x63 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x63 << 3) | 5:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x63 << 3) | 6:
                    Rra();
                    break;
                case (0x63 << 3) | 7:
                    FetchNextInstruction();
                    break;

                // NOP zp
                case (0x64 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x64 << 3) | 1:
                    Address = _data;
                    break;
                case (0x64 << 3) | 2:
                    FetchNextInstruction();
                    break;
                case (0x64 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x64 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x64 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x64 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x64 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ADC zp
                case (0x65 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x65 << 3) | 1:
                    Address = _data;
                    break;
                case (0x65 << 3) | 2:
                    Adc();
                    FetchNextInstruction();
                    break;
                case (0x65 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x65 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x65 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x65 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x65 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ROR zp
                case (0x66 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x66 << 3) | 1:
                    Address = _data;
                    break;
                case (0x66 << 3) | 2:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x66 << 3) | 3:
                    Ror();
                    break;
                case (0x66 << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x66 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x66 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x66 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RRA zp
                case (0x67 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x67 << 3) | 1:
                    Address = _data;
                    break;
                case (0x67 << 3) | 2:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x67 << 3) | 3:
                    Rra();
                    break;
                case (0x67 << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x67 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x67 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x67 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // PLA 
                case (0x68 << 3) | 0:
                    Address = PC;
                    break;
                case (0x68 << 3) | 1:
                    Address.Hi = 0x01;
                    Address.Lo = SP++;
                    break;
                case (0x68 << 3) | 2:
                    Address.Lo = SP;
                    break;
                case (0x68 << 3) | 3:
                    A = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0x68 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x68 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x68 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x68 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ADC #
                case (0x69 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x69 << 3) | 1:
                    Adc();
                    FetchNextInstruction();
                    break;
                case (0x69 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x69 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x69 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x69 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x69 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x69 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ROR 
                case (0x6A << 3) | 0:
                    Address = PC;
                    break;
                case (0x6A << 3) | 1:
                    Rora();
                    FetchNextInstruction();
                    break;
                case (0x6A << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x6A << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x6A << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x6A << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x6A << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x6A << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ARR #
                case (0x6B << 3) | 0:
                    Address = PC++;
                    break;
                case (0x6B << 3) | 1:
                    Arr();
                    FetchNextInstruction();
                    break;
                case (0x6B << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x6B << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x6B << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x6B << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x6B << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x6B << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JMP ind
                case (0x6C << 3) | 0:
                    Address = PC++;
                    break;
                case (0x6C << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x6C << 3) | 2:
                    _ad.Hi = _data;
                    Address = _ad;
                    break;
                case (0x6C << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x6C << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    PC = Address;
                    FetchNextInstruction();
                    break;
                case (0x6C << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x6C << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x6C << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ADC abs
                case (0x6D << 3) | 0:
                    Address = PC++;
                    break;
                case (0x6D << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x6D << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x6D << 3) | 3:
                    Adc();
                    FetchNextInstruction();
                    break;
                case (0x6D << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x6D << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x6D << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x6D << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ROR abs
                case (0x6E << 3) | 0:
                    Address = PC++;
                    break;
                case (0x6E << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x6E << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x6E << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x6E << 3) | 4:
                    Ror();
                    break;
                case (0x6E << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x6E << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x6E << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RRA abs
                case (0x6F << 3) | 0:
                    Address = PC++;
                    break;
                case (0x6F << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x6F << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x6F << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x6F << 3) | 4:
                    Rra();
                    break;
                case (0x6F << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x6F << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x6F << 3) | 7:
                    Debug.Assert(false);
                    break;

                // BVS #
                case (0x70 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x70 << 3) | 1:
                    Address = PC;
                    _ad = PC + (sbyte)_data;
                    if (P.V != true)
                    {
                        FetchNextInstruction();
                    }
                    break;
                case (0x70 << 3) | 2:
                    Address.Lo = _ad.Lo;
                    if (_ad.Hi == PC.Hi)
                    {
                        PC = _ad;
                        FetchNextInstruction();
                    }
                    break;
                case (0x70 << 3) | 3:
                    PC = _ad;
                    FetchNextInstruction();
                    break;
                case (0x70 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x70 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x70 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x70 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ADC (zp),Y
                case (0x71 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x71 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x71 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x71 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0x71 << 3) | 4:
                    Address = _ad + Y;
                    break;
                case (0x71 << 3) | 5:
                    Adc();
                    FetchNextInstruction();
                    break;
                case (0x71 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x71 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JAM invalid
                case (0x72 << 3) | 0:
                    Address = PC;
                    break;
                case (0x72 << 3) | 1:
                    Address.Hi = 0xFF;
                    Address.Lo = 0xFF;
                    _data = 0xFF;
                    _ir--;
                    FetchNextInstruction();
                    break;
                case (0x72 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x72 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x72 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x72 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x72 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x72 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RRA (zp),Y
                case (0x73 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x73 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x73 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x73 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0x73 << 3) | 4:
                    Address = _ad + Y;
                    break;
                case (0x73 << 3) | 5:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x73 << 3) | 6:
                    Rra();
                    break;
                case (0x73 << 3) | 7:
                    FetchNextInstruction();
                    break;

                // NOP zp,X
                case (0x74 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x74 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x74 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x74 << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0x74 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x74 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x74 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x74 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ADC zp,X
                case (0x75 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x75 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x75 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x75 << 3) | 3:
                    Adc();
                    FetchNextInstruction();
                    break;
                case (0x75 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x75 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x75 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x75 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ROR zp,X
                case (0x76 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x76 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x76 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x76 << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x76 << 3) | 4:
                    Ror();
                    break;
                case (0x76 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x76 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x76 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RRA zp,X
                case (0x77 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x77 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x77 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0x77 << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x77 << 3) | 4:
                    Rra();
                    break;
                case (0x77 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x77 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x77 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SEI 
                case (0x78 << 3) | 0:
                    Address = PC;
                    break;
                case (0x78 << 3) | 1:
                    P.I = true;
                    FetchNextInstruction();
                    break;
                case (0x78 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x78 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x78 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x78 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x78 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x78 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ADC abs,Y
                case (0x79 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x79 << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x79 << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0x79 << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0x79 << 3) | 4:
                    Adc();
                    FetchNextInstruction();
                    break;
                case (0x79 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x79 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x79 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP 
                case (0x7A << 3) | 0:
                    Address = PC;
                    break;
                case (0x7A << 3) | 1:
                    FetchNextInstruction();
                    break;
                case (0x7A << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x7A << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x7A << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x7A << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x7A << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x7A << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RRA abs,Y
                case (0x7B << 3) | 0:
                    Address = PC++;
                    break;
                case (0x7B << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x7B << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0x7B << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0x7B << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x7B << 3) | 5:
                    Rra();
                    break;
                case (0x7B << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0x7B << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP abs,X
                case (0x7C << 3) | 0:
                    Address = PC++;
                    break;
                case (0x7C << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x7C << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    IncrementTimingRegisterIfNoPageCrossing(X);
                    break;
                case (0x7C << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x7C << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x7C << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x7C << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x7C << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ADC abs,X
                case (0x7D << 3) | 0:
                    Address = PC++;
                    break;
                case (0x7D << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x7D << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    IncrementTimingRegisterIfNoPageCrossing(X);
                    break;
                case (0x7D << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x7D << 3) | 4:
                    Adc();
                    FetchNextInstruction();
                    break;
                case (0x7D << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x7D << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x7D << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ROR abs,X
                case (0x7E << 3) | 0:
                    Address = PC++;
                    break;
                case (0x7E << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x7E << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    break;
                case (0x7E << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x7E << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x7E << 3) | 5:
                    Ror();
                    break;
                case (0x7E << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0x7E << 3) | 7:
                    Debug.Assert(false);
                    break;

                // RRA abs,X
                case (0x7F << 3) | 0:
                    Address = PC++;
                    break;
                case (0x7F << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x7F << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    break;
                case (0x7F << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0x7F << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0x7F << 3) | 5:
                    Rra();
                    break;
                case (0x7F << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0x7F << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP #
                case (0x80 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x80 << 3) | 1:
                    FetchNextInstruction();
                    break;
                case (0x80 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x80 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x80 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x80 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x80 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x80 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // STA (zp,X) - Store Accumulator
                case (0x81 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x81 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x81 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x81 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x81 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    _data = A;
                    _rw = false;
                    break;
                case (0x81 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x81 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x81 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP #
                case (0x82 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x82 << 3) | 1:
                    FetchNextInstruction();
                    break;
                case (0x82 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x82 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x82 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x82 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x82 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x82 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SAX (zp,X) - Store Accumulator and X (undocumented)
                case (0x83 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x83 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x83 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0x83 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x83 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    _data = (byte)(A & X);
                    _rw = false;
                    break;
                case (0x83 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x83 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x83 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // STY zp - Store Y Register
                case (0x84 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x84 << 3) | 1:
                    Address = _data;
                    _data = Y;
                    _rw = false;
                    break;
                case (0x84 << 3) | 2:
                    FetchNextInstruction();
                    break;
                case (0x84 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x84 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x84 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x84 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x84 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // STA zp - Store Accumulator
                case (0x85 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x85 << 3) | 1:
                    Address = _data;
                    _data = A;
                    _rw = false;
                    break;
                case (0x85 << 3) | 2:
                    FetchNextInstruction();
                    break;
                case (0x85 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x85 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x85 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x85 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x85 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // STX zp - Store X Register
                case (0x86 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x86 << 3) | 1:
                    Address = _data;
                    _data = X;
                    _rw = false;
                    break;
                case (0x86 << 3) | 2:
                    FetchNextInstruction();
                    break;
                case (0x86 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x86 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x86 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x86 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x86 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SAX zp - Store Accumulator and X (undocumented)
                case (0x87 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x87 << 3) | 1:
                    Address = _data;
                    _data = (byte)(A & X);
                    _rw = false;
                    break;
                case (0x87 << 3) | 2:
                    FetchNextInstruction();
                    break;
                case (0x87 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x87 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x87 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x87 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x87 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // DEY  - Decrement Y Register
                case (0x88 << 3) | 0:
                    Address = PC;
                    break;
                case (0x88 << 3) | 1:
                    Y = P.SetZeroNegativeFlags((byte)(Y - 1));
                    FetchNextInstruction();
                    break;
                case (0x88 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x88 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x88 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x88 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x88 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x88 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP #
                case (0x89 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x89 << 3) | 1:
                    FetchNextInstruction();
                    break;
                case (0x89 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x89 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x89 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x89 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x89 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x89 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // TXA  - Transfer X to Accumulator
                case (0x8A << 3) | 0:
                    Address = PC;
                    break;
                case (0x8A << 3) | 1:
                    A = P.SetZeroNegativeFlags(X);
                    FetchNextInstruction();
                    break;
                case (0x8A << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x8A << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x8A << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x8A << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x8A << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x8A << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ANE #
                case (0x8B << 3) | 0:
                    Address = PC++;
                    break;
                case (0x8B << 3) | 1:
                    A = (byte)((A | 0xEE) & X & _data);
                    P.SetZeroNegativeFlags(A);
                    FetchNextInstruction();
                    break;
                case (0x8B << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x8B << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x8B << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x8B << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x8B << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x8B << 3) | 7:
                    Debug.Assert(false);
                    break;

                // STY abs - Store Y Register
                case (0x8C << 3) | 0:
                    Address = PC++;
                    break;
                case (0x8C << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x8C << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    _data = Y;
                    _rw = false;
                    break;
                case (0x8C << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0x8C << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x8C << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x8C << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x8C << 3) | 7:
                    Debug.Assert(false);
                    break;

                // STA abs - Store Accumulator
                case (0x8D << 3) | 0:
                    Address = PC++;
                    break;
                case (0x8D << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x8D << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    _data = A;
                    _rw = false;
                    break;
                case (0x8D << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0x8D << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x8D << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x8D << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x8D << 3) | 7:
                    Debug.Assert(false);
                    break;

                // STX abs - Store X Register
                case (0x8E << 3) | 0:
                    Address = PC++;
                    break;
                case (0x8E << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x8E << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    _data = X;
                    _rw = false;
                    break;
                case (0x8E << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0x8E << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x8E << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x8E << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x8E << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SAX abs - Store Accumulator and X (undocumented)
                case (0x8F << 3) | 0:
                    Address = PC++;
                    break;
                case (0x8F << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x8F << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    _data = (byte)(A & X);
                    _rw = false;
                    break;
                case (0x8F << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0x8F << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x8F << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x8F << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x8F << 3) | 7:
                    Debug.Assert(false);
                    break;

                // BCC #
                case (0x90 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x90 << 3) | 1:
                    Address = PC;
                    _ad = PC + (sbyte)_data;
                    if (P.C != false)
                    {
                        FetchNextInstruction();
                    }
                    break;
                case (0x90 << 3) | 2:
                    Address.Lo = _ad.Lo;
                    if (_ad.Hi == PC.Hi)
                    {
                        PC = _ad;
                        FetchNextInstruction();
                    }
                    break;
                case (0x90 << 3) | 3:
                    PC = _ad;
                    FetchNextInstruction();
                    break;
                case (0x90 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x90 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x90 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x90 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // STA (zp),Y - Store Accumulator
                case (0x91 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x91 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x91 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x91 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0x91 << 3) | 4:
                    Address = _ad + Y;
                    _data = A;
                    _rw = false;
                    break;
                case (0x91 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x91 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x91 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JAM invalid
                case (0x92 << 3) | 0:
                    Address = PC;
                    break;
                case (0x92 << 3) | 1:
                    Address.Hi = 0xFF;
                    Address.Lo = 0xFF;
                    _data = 0xFF;
                    _ir--;
                    FetchNextInstruction();
                    break;
                case (0x92 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x92 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x92 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x92 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x92 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x92 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SHA (zp),Y
                case (0x93 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x93 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x93 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0x93 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0x93 << 3) | 4:
                    Address = _ad + Y;
                    _data = (byte)(A & X & (Address.Hi + 1));
                    _rw = false;
                    break;
                case (0x93 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0x93 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x93 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // STY zp,X - Store Y Register
                case (0x94 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x94 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x94 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    _data = Y;
                    _rw = false;
                    break;
                case (0x94 << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0x94 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x94 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x94 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x94 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // STA zp,X - Store Accumulator
                case (0x95 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x95 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x95 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    _data = A;
                    _rw = false;
                    break;
                case (0x95 << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0x95 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x95 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x95 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x95 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // STX zp,Y - Store X Register
                case (0x96 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x96 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x96 << 3) | 2:
                    Address = (byte)(_ad.Lo + Y);
                    _data = X;
                    _rw = false;
                    break;
                case (0x96 << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0x96 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x96 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x96 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x96 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SAX zp,Y - Store Accumulator and X (undocumented)
                case (0x97 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x97 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0x97 << 3) | 2:
                    Address = (byte)(_ad.Lo + Y);
                    _data = (byte)(A & X);
                    _rw = false;
                    break;
                case (0x97 << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0x97 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x97 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x97 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x97 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // TYA  - Transfer Y to Accumulator
                case (0x98 << 3) | 0:
                    Address = PC;
                    break;
                case (0x98 << 3) | 1:
                    A = P.SetZeroNegativeFlags(Y);
                    FetchNextInstruction();
                    break;
                case (0x98 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x98 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x98 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x98 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x98 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x98 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // STA abs,Y - Store Accumulator
                case (0x99 << 3) | 0:
                    Address = PC++;
                    break;
                case (0x99 << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x99 << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0x99 << 3) | 3:
                    Address = _ad + Y;
                    _data = A;
                    _rw = false;
                    break;
                case (0x99 << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x99 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x99 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x99 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // TXS  - Transfer X to Stack Pointer
                case (0x9A << 3) | 0:
                    Address = PC;
                    break;
                case (0x9A << 3) | 1:
                    SP = X;
                    FetchNextInstruction();
                    break;
                case (0x9A << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0x9A << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0x9A << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0x9A << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x9A << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x9A << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SHS abs,Y
                case (0x9B << 3) | 0:
                    Address = PC++;
                    break;
                case (0x9B << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x9B << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0x9B << 3) | 3:
                    Address = _ad + Y;
                    SP = (byte)(A & X);
                    _data = (byte)(A & X & (Address.Hi + 1));
                    _rw = false;
                    break;
                case (0x9B << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x9B << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x9B << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x9B << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SHY abs,X
                case (0x9C << 3) | 0:
                    Address = PC++;
                    break;
                case (0x9C << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x9C << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    break;
                case (0x9C << 3) | 3:
                    Address = _ad + X;
                    _data = (byte)(Y & (Address.Hi + 1));
                    _rw = false;
                    break;
                case (0x9C << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x9C << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x9C << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x9C << 3) | 7:
                    Debug.Assert(false);
                    break;

                // STA abs,X - Store Accumulator
                case (0x9D << 3) | 0:
                    Address = PC++;
                    break;
                case (0x9D << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x9D << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    break;
                case (0x9D << 3) | 3:
                    Address = _ad + X;
                    _data = A;
                    _rw = false;
                    break;
                case (0x9D << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x9D << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x9D << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x9D << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SHX abs,Y
                case (0x9E << 3) | 0:
                    Address = PC++;
                    break;
                case (0x9E << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x9E << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0x9E << 3) | 3:
                    Address = _ad + Y;
                    _data = (byte)(X & (Address.Hi + 1));
                    _rw = false;
                    break;
                case (0x9E << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x9E << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x9E << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x9E << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SHA abs,Y
                case (0x9F << 3) | 0:
                    Address = PC++;
                    break;
                case (0x9F << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0x9F << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0x9F << 3) | 3:
                    Address = _ad + Y;
                    _data = (byte)(A & X & (Address.Hi + 1));
                    _rw = false;
                    break;
                case (0x9F << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0x9F << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0x9F << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0x9F << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDY # - Load Y Register
                case (0xA0 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xA0 << 3) | 1:
                    Y = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xA0 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xA0 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xA0 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xA0 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xA0 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xA0 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDA (zp,X) - Load Accumulator
                case (0xA1 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xA1 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xA1 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xA1 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0xA1 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xA1 << 3) | 5:
                    A = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xA1 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xA1 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDX # - Load X Register
                case (0xA2 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xA2 << 3) | 1:
                    X = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xA2 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xA2 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xA2 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xA2 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xA2 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xA2 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LAX (zp,X) - Load A and X Registers (undocumented)
                case (0xA3 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xA3 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xA3 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xA3 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0xA3 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xA3 << 3) | 5:
                    A = P.SetZeroNegativeFlags(_data);
                    X = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xA3 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xA3 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDY zp - Load Y Register
                case (0xA4 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xA4 << 3) | 1:
                    Address = _data;
                    break;
                case (0xA4 << 3) | 2:
                    Y = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xA4 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xA4 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xA4 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xA4 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xA4 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDA zp - Load Accumulator
                case (0xA5 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xA5 << 3) | 1:
                    Address = _data;
                    break;
                case (0xA5 << 3) | 2:
                    A = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xA5 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xA5 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xA5 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xA5 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xA5 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDX zp - Load X Register
                case (0xA6 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xA6 << 3) | 1:
                    Address = _data;
                    break;
                case (0xA6 << 3) | 2:
                    X = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xA6 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xA6 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xA6 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xA6 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xA6 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LAX zp - Load A and X Registers (undocumented)
                case (0xA7 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xA7 << 3) | 1:
                    Address = _data;
                    break;
                case (0xA7 << 3) | 2:
                    A = P.SetZeroNegativeFlags(_data);
                    X = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xA7 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xA7 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xA7 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xA7 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xA7 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // TAY  - Transfer Accumulator to Y
                case (0xA8 << 3) | 0:
                    Address = PC;
                    break;
                case (0xA8 << 3) | 1:
                    Y = P.SetZeroNegativeFlags(A);
                    FetchNextInstruction();
                    break;
                case (0xA8 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xA8 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xA8 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xA8 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xA8 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xA8 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDA # - Load Accumulator
                case (0xA9 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xA9 << 3) | 1:
                    A = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xA9 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xA9 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xA9 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xA9 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xA9 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xA9 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // TAX  - Transfer Accumulator to X
                case (0xAA << 3) | 0:
                    Address = PC;
                    break;
                case (0xAA << 3) | 1:
                    X = P.SetZeroNegativeFlags(A);
                    FetchNextInstruction();
                    break;
                case (0xAA << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xAA << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xAA << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xAA << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xAA << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xAA << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LXA #
                case (0xAB << 3) | 0:
                    Address = PC++;
                    break;
                case (0xAB << 3) | 1:
                    A = (byte)((A | 0xEE) & _data);
                    X = A;
                    P.SetZeroNegativeFlags(A);
                    FetchNextInstruction();
                    break;
                case (0xAB << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xAB << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xAB << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xAB << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xAB << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xAB << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDY abs - Load Y Register
                case (0xAC << 3) | 0:
                    Address = PC++;
                    break;
                case (0xAC << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xAC << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xAC << 3) | 3:
                    Y = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xAC << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xAC << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xAC << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xAC << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDA abs - Load Accumulator
                case (0xAD << 3) | 0:
                    Address = PC++;
                    break;
                case (0xAD << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xAD << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xAD << 3) | 3:
                    A = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xAD << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xAD << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xAD << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xAD << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDX abs - Load X Register
                case (0xAE << 3) | 0:
                    Address = PC++;
                    break;
                case (0xAE << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xAE << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xAE << 3) | 3:
                    X = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xAE << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xAE << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xAE << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xAE << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LAX abs - Load A and X Registers (undocumented)
                case (0xAF << 3) | 0:
                    Address = PC++;
                    break;
                case (0xAF << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xAF << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xAF << 3) | 3:
                    A = P.SetZeroNegativeFlags(_data);
                    X = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xAF << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xAF << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xAF << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xAF << 3) | 7:
                    Debug.Assert(false);
                    break;

                // BCS #
                case (0xB0 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xB0 << 3) | 1:
                    Address = PC;
                    _ad = PC + (sbyte)_data;
                    if (P.C != true)
                    {
                        FetchNextInstruction();
                    }
                    break;
                case (0xB0 << 3) | 2:
                    Address.Lo = _ad.Lo;
                    if (_ad.Hi == PC.Hi)
                    {
                        PC = _ad;
                        FetchNextInstruction();
                    }
                    break;
                case (0xB0 << 3) | 3:
                    PC = _ad;
                    FetchNextInstruction();
                    break;
                case (0xB0 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xB0 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xB0 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xB0 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDA (zp),Y - Load Accumulator
                case (0xB1 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xB1 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xB1 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0xB1 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0xB1 << 3) | 4:
                    Address = _ad + Y;
                    break;
                case (0xB1 << 3) | 5:
                    A = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xB1 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xB1 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JAM invalid
                case (0xB2 << 3) | 0:
                    Address = PC;
                    break;
                case (0xB2 << 3) | 1:
                    Address.Hi = 0xFF;
                    Address.Lo = 0xFF;
                    _data = 0xFF;
                    _ir--;
                    FetchNextInstruction();
                    break;
                case (0xB2 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xB2 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xB2 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xB2 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xB2 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xB2 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LAX (zp),Y - Load A and X Registers (undocumented)
                case (0xB3 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xB3 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xB3 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0xB3 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0xB3 << 3) | 4:
                    Address = _ad + Y;
                    break;
                case (0xB3 << 3) | 5:
                    A = P.SetZeroNegativeFlags(_data);
                    X = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xB3 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xB3 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDY zp,X - Load Y Register
                case (0xB4 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xB4 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xB4 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0xB4 << 3) | 3:
                    Y = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xB4 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xB4 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xB4 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xB4 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDA zp,X - Load Accumulator
                case (0xB5 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xB5 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xB5 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0xB5 << 3) | 3:
                    A = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xB5 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xB5 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xB5 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xB5 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDX zp,Y - Load X Register
                case (0xB6 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xB6 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xB6 << 3) | 2:
                    Address = (byte)(_ad.Lo + Y);
                    break;
                case (0xB6 << 3) | 3:
                    X = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xB6 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xB6 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xB6 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xB6 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LAX zp,Y - Load A and X Registers (undocumented)
                case (0xB7 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xB7 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xB7 << 3) | 2:
                    Address = (byte)(_ad.Lo + Y);
                    break;
                case (0xB7 << 3) | 3:
                    A = P.SetZeroNegativeFlags(_data);
                    X = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xB7 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xB7 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xB7 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xB7 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CLV 
                case (0xB8 << 3) | 0:
                    Address = PC;
                    break;
                case (0xB8 << 3) | 1:
                    P.V = false;
                    FetchNextInstruction();
                    break;
                case (0xB8 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xB8 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xB8 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xB8 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xB8 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xB8 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDA abs,Y - Load Accumulator
                case (0xB9 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xB9 << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xB9 << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0xB9 << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0xB9 << 3) | 4:
                    A = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xB9 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xB9 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xB9 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // TSX  - Transfer Stack Pointer to X
                case (0xBA << 3) | 0:
                    Address = PC;
                    break;
                case (0xBA << 3) | 1:
                    X = P.SetZeroNegativeFlags(SP);
                    FetchNextInstruction();
                    break;
                case (0xBA << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xBA << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xBA << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xBA << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xBA << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xBA << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LAS abs,Y
                case (0xBB << 3) | 0:
                    Address = PC++;
                    break;
                case (0xBB << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xBB << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0xBB << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0xBB << 3) | 4:
                    A = (byte)(_data & SP);
                    X = A;
                    SP = A;
                    P.SetZeroNegativeFlags(A);
                    FetchNextInstruction();
                    break;
                case (0xBB << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xBB << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xBB << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDY abs,X - Load Y Register
                case (0xBC << 3) | 0:
                    Address = PC++;
                    break;
                case (0xBC << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xBC << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    IncrementTimingRegisterIfNoPageCrossing(X);
                    break;
                case (0xBC << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0xBC << 3) | 4:
                    Y = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xBC << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xBC << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xBC << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDA abs,X - Load Accumulator
                case (0xBD << 3) | 0:
                    Address = PC++;
                    break;
                case (0xBD << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xBD << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    IncrementTimingRegisterIfNoPageCrossing(X);
                    break;
                case (0xBD << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0xBD << 3) | 4:
                    A = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xBD << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xBD << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xBD << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LDX abs,Y - Load X Register
                case (0xBE << 3) | 0:
                    Address = PC++;
                    break;
                case (0xBE << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xBE << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0xBE << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0xBE << 3) | 4:
                    X = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xBE << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xBE << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xBE << 3) | 7:
                    Debug.Assert(false);
                    break;

                // LAX abs,Y - Load A and X Registers (undocumented)
                case (0xBF << 3) | 0:
                    Address = PC++;
                    break;
                case (0xBF << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xBF << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0xBF << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0xBF << 3) | 4:
                    A = P.SetZeroNegativeFlags(_data);
                    X = P.SetZeroNegativeFlags(_data);
                    FetchNextInstruction();
                    break;
                case (0xBF << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xBF << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xBF << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CPY # - Compare Y Register
                case (0xC0 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xC0 << 3) | 1:
                    P.SetZeroNegativeFlags((byte)(Y - _data));
                    P.C = Y >= _data;
                    FetchNextInstruction();
                    break;
                case (0xC0 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xC0 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xC0 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xC0 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xC0 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xC0 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CMP (zp,X) - Compare
                case (0xC1 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xC1 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xC1 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xC1 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0xC1 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xC1 << 3) | 5:
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    FetchNextInstruction();
                    break;
                case (0xC1 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xC1 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP #
                case (0xC2 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xC2 << 3) | 1:
                    FetchNextInstruction();
                    break;
                case (0xC2 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xC2 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xC2 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xC2 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xC2 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xC2 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // DCP (zp,X) - Decrement Memory then Compare (undocumented)
                case (0xC3 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xC3 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xC3 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xC3 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0xC3 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xC3 << 3) | 5:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xC3 << 3) | 6:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo - 1));
                    _rw = false;
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    break;
                case (0xC3 << 3) | 7:
                    FetchNextInstruction();
                    break;

                // CPY zp - Compare Y Register
                case (0xC4 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xC4 << 3) | 1:
                    Address = _data;
                    break;
                case (0xC4 << 3) | 2:
                    P.SetZeroNegativeFlags((byte)(Y - _data));
                    P.C = Y >= _data;
                    FetchNextInstruction();
                    break;
                case (0xC4 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xC4 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xC4 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xC4 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xC4 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CMP zp - Compare
                case (0xC5 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xC5 << 3) | 1:
                    Address = _data;
                    break;
                case (0xC5 << 3) | 2:
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    FetchNextInstruction();
                    break;
                case (0xC5 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xC5 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xC5 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xC5 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xC5 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // DEC zp - Decrement Memory
                case (0xC6 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xC6 << 3) | 1:
                    Address = _data;
                    break;
                case (0xC6 << 3) | 2:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xC6 << 3) | 3:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo - 1));
                    _rw = false;
                    break;
                case (0xC6 << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0xC6 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xC6 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xC6 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // DCP zp - Decrement Memory then Compare (undocumented)
                case (0xC7 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xC7 << 3) | 1:
                    Address = _data;
                    break;
                case (0xC7 << 3) | 2:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xC7 << 3) | 3:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo - 1));
                    _rw = false;
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    break;
                case (0xC7 << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0xC7 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xC7 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xC7 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // INY  - Increment Y Register
                case (0xC8 << 3) | 0:
                    Address = PC;
                    break;
                case (0xC8 << 3) | 1:
                    Y = P.SetZeroNegativeFlags((byte)(Y + 1));
                    FetchNextInstruction();
                    break;
                case (0xC8 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xC8 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xC8 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xC8 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xC8 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xC8 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CMP # - Compare
                case (0xC9 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xC9 << 3) | 1:
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    FetchNextInstruction();
                    break;
                case (0xC9 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xC9 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xC9 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xC9 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xC9 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xC9 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // DEX  - Decrement X Register
                case (0xCA << 3) | 0:
                    Address = PC;
                    break;
                case (0xCA << 3) | 1:
                    X = P.SetZeroNegativeFlags((byte)(X - 1));
                    FetchNextInstruction();
                    break;
                case (0xCA << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xCA << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xCA << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xCA << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xCA << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xCA << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SBX #
                case (0xCB << 3) | 0:
                    Address = PC++;
                    break;
                case (0xCB << 3) | 1:
                    tempInt32 = (A & X) - _data;
                    X = (byte)tempInt32;
                    P.C = tempInt32 >= 0;
                    P.SetZeroNegativeFlags(X);
                    FetchNextInstruction();
                    break;
                case (0xCB << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xCB << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xCB << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xCB << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xCB << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xCB << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CPY abs - Compare Y Register
                case (0xCC << 3) | 0:
                    Address = PC++;
                    break;
                case (0xCC << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xCC << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xCC << 3) | 3:
                    P.SetZeroNegativeFlags((byte)(Y - _data));
                    P.C = Y >= _data;
                    FetchNextInstruction();
                    break;
                case (0xCC << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xCC << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xCC << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xCC << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CMP abs - Compare
                case (0xCD << 3) | 0:
                    Address = PC++;
                    break;
                case (0xCD << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xCD << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xCD << 3) | 3:
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    FetchNextInstruction();
                    break;
                case (0xCD << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xCD << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xCD << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xCD << 3) | 7:
                    Debug.Assert(false);
                    break;

                // DEC abs - Decrement Memory
                case (0xCE << 3) | 0:
                    Address = PC++;
                    break;
                case (0xCE << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xCE << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xCE << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xCE << 3) | 4:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo - 1));
                    _rw = false;
                    break;
                case (0xCE << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0xCE << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xCE << 3) | 7:
                    Debug.Assert(false);
                    break;

                // DCP abs - Decrement Memory then Compare (undocumented)
                case (0xCF << 3) | 0:
                    Address = PC++;
                    break;
                case (0xCF << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xCF << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xCF << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xCF << 3) | 4:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo - 1));
                    _rw = false;
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    break;
                case (0xCF << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0xCF << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xCF << 3) | 7:
                    Debug.Assert(false);
                    break;

                // BNE #
                case (0xD0 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xD0 << 3) | 1:
                    Address = PC;
                    _ad = PC + (sbyte)_data;
                    if (P.Z != false)
                    {
                        FetchNextInstruction();
                    }
                    break;
                case (0xD0 << 3) | 2:
                    Address.Lo = _ad.Lo;
                    if (_ad.Hi == PC.Hi)
                    {
                        PC = _ad;
                        FetchNextInstruction();
                    }
                    break;
                case (0xD0 << 3) | 3:
                    PC = _ad;
                    FetchNextInstruction();
                    break;
                case (0xD0 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xD0 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xD0 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xD0 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CMP (zp),Y - Compare
                case (0xD1 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xD1 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xD1 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0xD1 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0xD1 << 3) | 4:
                    Address = _ad + Y;
                    break;
                case (0xD1 << 3) | 5:
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    FetchNextInstruction();
                    break;
                case (0xD1 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xD1 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JAM invalid
                case (0xD2 << 3) | 0:
                    Address = PC;
                    break;
                case (0xD2 << 3) | 1:
                    Address.Hi = 0xFF;
                    Address.Lo = 0xFF;
                    _data = 0xFF;
                    _ir--;
                    FetchNextInstruction();
                    break;
                case (0xD2 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xD2 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xD2 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xD2 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xD2 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xD2 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // DCP (zp),Y - Decrement Memory then Compare (undocumented)
                case (0xD3 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xD3 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xD3 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0xD3 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0xD3 << 3) | 4:
                    Address = _ad + Y;
                    break;
                case (0xD3 << 3) | 5:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xD3 << 3) | 6:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo - 1));
                    _rw = false;
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    break;
                case (0xD3 << 3) | 7:
                    FetchNextInstruction();
                    break;

                // NOP zp,X
                case (0xD4 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xD4 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xD4 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0xD4 << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0xD4 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xD4 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xD4 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xD4 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CMP zp,X - Compare
                case (0xD5 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xD5 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xD5 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0xD5 << 3) | 3:
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    FetchNextInstruction();
                    break;
                case (0xD5 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xD5 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xD5 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xD5 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // DEC zp,X - Decrement Memory
                case (0xD6 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xD6 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xD6 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0xD6 << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xD6 << 3) | 4:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo - 1));
                    _rw = false;
                    break;
                case (0xD6 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0xD6 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xD6 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // DCP zp,X - Decrement Memory then Compare (undocumented)
                case (0xD7 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xD7 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xD7 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0xD7 << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xD7 << 3) | 4:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo - 1));
                    _rw = false;
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    break;
                case (0xD7 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0xD7 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xD7 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CLD 
                case (0xD8 << 3) | 0:
                    Address = PC;
                    break;
                case (0xD8 << 3) | 1:
                    P.D = false;
                    FetchNextInstruction();
                    break;
                case (0xD8 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xD8 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xD8 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xD8 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xD8 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xD8 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CMP abs,Y - Compare
                case (0xD9 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xD9 << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xD9 << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0xD9 << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0xD9 << 3) | 4:
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    FetchNextInstruction();
                    break;
                case (0xD9 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xD9 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xD9 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP 
                case (0xDA << 3) | 0:
                    Address = PC;
                    break;
                case (0xDA << 3) | 1:
                    FetchNextInstruction();
                    break;
                case (0xDA << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xDA << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xDA << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xDA << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xDA << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xDA << 3) | 7:
                    Debug.Assert(false);
                    break;

                // DCP abs,Y - Decrement Memory then Compare (undocumented)
                case (0xDB << 3) | 0:
                    Address = PC++;
                    break;
                case (0xDB << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xDB << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0xDB << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0xDB << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xDB << 3) | 5:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo - 1));
                    _rw = false;
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    break;
                case (0xDB << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0xDB << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP abs,X
                case (0xDC << 3) | 0:
                    Address = PC++;
                    break;
                case (0xDC << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xDC << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    IncrementTimingRegisterIfNoPageCrossing(X);
                    break;
                case (0xDC << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0xDC << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0xDC << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xDC << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xDC << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CMP abs,X - Compare
                case (0xDD << 3) | 0:
                    Address = PC++;
                    break;
                case (0xDD << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xDD << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    IncrementTimingRegisterIfNoPageCrossing(X);
                    break;
                case (0xDD << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0xDD << 3) | 4:
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    FetchNextInstruction();
                    break;
                case (0xDD << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xDD << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xDD << 3) | 7:
                    Debug.Assert(false);
                    break;

                // DEC abs,X - Decrement Memory
                case (0xDE << 3) | 0:
                    Address = PC++;
                    break;
                case (0xDE << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xDE << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    break;
                case (0xDE << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0xDE << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xDE << 3) | 5:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo - 1));
                    _rw = false;
                    break;
                case (0xDE << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0xDE << 3) | 7:
                    Debug.Assert(false);
                    break;

                // DCP abs,X - Decrement Memory then Compare (undocumented)
                case (0xDF << 3) | 0:
                    Address = PC++;
                    break;
                case (0xDF << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xDF << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    break;
                case (0xDF << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0xDF << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xDF << 3) | 5:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo - 1));
                    _rw = false;
                    P.SetZeroNegativeFlags((byte)(A - _data));
                    P.C = A >= _data;
                    break;
                case (0xDF << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0xDF << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CPX # - Compare X Register
                case (0xE0 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xE0 << 3) | 1:
                    P.SetZeroNegativeFlags((byte)(X - _data));
                    P.C = X >= _data;
                    FetchNextInstruction();
                    break;
                case (0xE0 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xE0 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xE0 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xE0 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xE0 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xE0 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SBC (zp,X)
                case (0xE1 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xE1 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xE1 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xE1 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0xE1 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xE1 << 3) | 5:
                    Sbc();
                    FetchNextInstruction();
                    break;
                case (0xE1 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xE1 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP #
                case (0xE2 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xE2 << 3) | 1:
                    FetchNextInstruction();
                    break;
                case (0xE2 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xE2 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xE2 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xE2 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xE2 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xE2 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ISB (zp,X) - Increment Memory then Subtract (undocumented, also known as ISC)
                case (0xE3 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xE3 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xE3 << 3) | 2:
                    _ad.Lo += X;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xE3 << 3) | 3:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0xE3 << 3) | 4:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xE3 << 3) | 5:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xE3 << 3) | 6:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo + 1));
                    _rw = false;
                    Sbc();
                    break;
                case (0xE3 << 3) | 7:
                    FetchNextInstruction();
                    break;

                // CPX zp - Compare X Register
                case (0xE4 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xE4 << 3) | 1:
                    Address = _data;
                    break;
                case (0xE4 << 3) | 2:
                    P.SetZeroNegativeFlags((byte)(X - _data));
                    P.C = X >= _data;
                    FetchNextInstruction();
                    break;
                case (0xE4 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xE4 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xE4 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xE4 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xE4 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SBC zp
                case (0xE5 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xE5 << 3) | 1:
                    Address = _data;
                    break;
                case (0xE5 << 3) | 2:
                    Sbc();
                    FetchNextInstruction();
                    break;
                case (0xE5 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xE5 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xE5 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xE5 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xE5 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // INC zp - Increment Memory
                case (0xE6 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xE6 << 3) | 1:
                    Address = _data;
                    break;
                case (0xE6 << 3) | 2:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xE6 << 3) | 3:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo + 1));
                    _rw = false;
                    break;
                case (0xE6 << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0xE6 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xE6 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xE6 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ISB zp - Increment Memory then Subtract (undocumented, also known as ISC)
                case (0xE7 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xE7 << 3) | 1:
                    Address = _data;
                    break;
                case (0xE7 << 3) | 2:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xE7 << 3) | 3:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo + 1));
                    _rw = false;
                    Sbc();
                    break;
                case (0xE7 << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0xE7 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xE7 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xE7 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // INX  - Increment X Register
                case (0xE8 << 3) | 0:
                    Address = PC;
                    break;
                case (0xE8 << 3) | 1:
                    X = P.SetZeroNegativeFlags((byte)(X + 1));
                    FetchNextInstruction();
                    break;
                case (0xE8 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xE8 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xE8 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xE8 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xE8 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xE8 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SBC #
                case (0xE9 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xE9 << 3) | 1:
                    Sbc();
                    FetchNextInstruction();
                    break;
                case (0xE9 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xE9 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xE9 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xE9 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xE9 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xE9 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP 
                case (0xEA << 3) | 0:
                    Address = PC;
                    break;
                case (0xEA << 3) | 1:
                    FetchNextInstruction();
                    break;
                case (0xEA << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xEA << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xEA << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xEA << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xEA << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xEA << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SBC #
                case (0xEB << 3) | 0:
                    Address = PC++;
                    break;
                case (0xEB << 3) | 1:
                    Sbc();
                    FetchNextInstruction();
                    break;
                case (0xEB << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xEB << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xEB << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xEB << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xEB << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xEB << 3) | 7:
                    Debug.Assert(false);
                    break;

                // CPX abs - Compare X Register
                case (0xEC << 3) | 0:
                    Address = PC++;
                    break;
                case (0xEC << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xEC << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xEC << 3) | 3:
                    P.SetZeroNegativeFlags((byte)(X - _data));
                    P.C = X >= _data;
                    FetchNextInstruction();
                    break;
                case (0xEC << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xEC << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xEC << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xEC << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SBC abs
                case (0xED << 3) | 0:
                    Address = PC++;
                    break;
                case (0xED << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xED << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xED << 3) | 3:
                    Sbc();
                    FetchNextInstruction();
                    break;
                case (0xED << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xED << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xED << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xED << 3) | 7:
                    Debug.Assert(false);
                    break;

                // INC abs - Increment Memory
                case (0xEE << 3) | 0:
                    Address = PC++;
                    break;
                case (0xEE << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xEE << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xEE << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xEE << 3) | 4:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo + 1));
                    _rw = false;
                    break;
                case (0xEE << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0xEE << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xEE << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ISB abs - Increment Memory then Subtract (undocumented, also known as ISC)
                case (0xEF << 3) | 0:
                    Address = PC++;
                    break;
                case (0xEF << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xEF << 3) | 2:
                    Address.Hi = _data;
                    Address.Lo = _ad.Lo;
                    break;
                case (0xEF << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xEF << 3) | 4:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo + 1));
                    _rw = false;
                    Sbc();
                    break;
                case (0xEF << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0xEF << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xEF << 3) | 7:
                    Debug.Assert(false);
                    break;

                // BEQ #
                case (0xF0 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xF0 << 3) | 1:
                    Address = PC;
                    _ad = PC + (sbyte)_data;
                    if (P.Z != true)
                    {
                        FetchNextInstruction();
                    }
                    break;
                case (0xF0 << 3) | 2:
                    Address.Lo = _ad.Lo;
                    if (_ad.Hi == PC.Hi)
                    {
                        PC = _ad;
                        FetchNextInstruction();
                    }
                    break;
                case (0xF0 << 3) | 3:
                    PC = _ad;
                    FetchNextInstruction();
                    break;
                case (0xF0 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xF0 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xF0 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xF0 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SBC (zp),Y
                case (0xF1 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xF1 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xF1 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0xF1 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0xF1 << 3) | 4:
                    Address = _ad + Y;
                    break;
                case (0xF1 << 3) | 5:
                    Sbc();
                    FetchNextInstruction();
                    break;
                case (0xF1 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xF1 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // JAM invalid
                case (0xF2 << 3) | 0:
                    Address = PC;
                    break;
                case (0xF2 << 3) | 1:
                    Address.Hi = 0xFF;
                    Address.Lo = 0xFF;
                    _data = 0xFF;
                    _ir--;
                    FetchNextInstruction();
                    break;
                case (0xF2 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xF2 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xF2 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xF2 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xF2 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xF2 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ISB (zp),Y - Increment Memory then Subtract (undocumented, also known as ISC)
                case (0xF3 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xF3 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xF3 << 3) | 2:
                    Address.Lo = (byte)(_ad.Lo + 1);
                    _ad.Lo = _data;
                    break;
                case (0xF3 << 3) | 3:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0xF3 << 3) | 4:
                    Address = _ad + Y;
                    break;
                case (0xF3 << 3) | 5:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xF3 << 3) | 6:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo + 1));
                    _rw = false;
                    Sbc();
                    break;
                case (0xF3 << 3) | 7:
                    FetchNextInstruction();
                    break;

                // NOP zp,X
                case (0xF4 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xF4 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xF4 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0xF4 << 3) | 3:
                    FetchNextInstruction();
                    break;
                case (0xF4 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xF4 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xF4 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xF4 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SBC zp,X
                case (0xF5 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xF5 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xF5 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0xF5 << 3) | 3:
                    Sbc();
                    FetchNextInstruction();
                    break;
                case (0xF5 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xF5 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xF5 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xF5 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // INC zp,X - Increment Memory
                case (0xF6 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xF6 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xF6 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0xF6 << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xF6 << 3) | 4:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo + 1));
                    _rw = false;
                    break;
                case (0xF6 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0xF6 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xF6 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ISB zp,X - Increment Memory then Subtract (undocumented, also known as ISC)
                case (0xF7 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xF7 << 3) | 1:
                    _ad = _data;
                    Address = _ad;
                    break;
                case (0xF7 << 3) | 2:
                    Address = (byte)(_ad.Lo + X);
                    break;
                case (0xF7 << 3) | 3:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xF7 << 3) | 4:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo + 1));
                    _rw = false;
                    Sbc();
                    break;
                case (0xF7 << 3) | 5:
                    FetchNextInstruction();
                    break;
                case (0xF7 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xF7 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SED 
                case (0xF8 << 3) | 0:
                    Address = PC;
                    break;
                case (0xF8 << 3) | 1:
                    P.D = true;
                    FetchNextInstruction();
                    break;
                case (0xF8 << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xF8 << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xF8 << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xF8 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xF8 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xF8 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SBC abs,Y
                case (0xF9 << 3) | 0:
                    Address = PC++;
                    break;
                case (0xF9 << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xF9 << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    IncrementTimingRegisterIfNoPageCrossing(Y);
                    break;
                case (0xF9 << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0xF9 << 3) | 4:
                    Sbc();
                    FetchNextInstruction();
                    break;
                case (0xF9 << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xF9 << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xF9 << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP 
                case (0xFA << 3) | 0:
                    Address = PC;
                    break;
                case (0xFA << 3) | 1:
                    FetchNextInstruction();
                    break;
                case (0xFA << 3) | 2:
                    Debug.Assert(false);
                    break;
                case (0xFA << 3) | 3:
                    Debug.Assert(false);
                    break;
                case (0xFA << 3) | 4:
                    Debug.Assert(false);
                    break;
                case (0xFA << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xFA << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xFA << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ISB abs,Y - Increment Memory then Subtract (undocumented, also known as ISC)
                case (0xFB << 3) | 0:
                    Address = PC++;
                    break;
                case (0xFB << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xFB << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + Y);
                    break;
                case (0xFB << 3) | 3:
                    Address = _ad + Y;
                    break;
                case (0xFB << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xFB << 3) | 5:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo + 1));
                    _rw = false;
                    Sbc();
                    break;
                case (0xFB << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0xFB << 3) | 7:
                    Debug.Assert(false);
                    break;

                // NOP abs,X
                case (0xFC << 3) | 0:
                    Address = PC++;
                    break;
                case (0xFC << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xFC << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    IncrementTimingRegisterIfNoPageCrossing(X);
                    break;
                case (0xFC << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0xFC << 3) | 4:
                    FetchNextInstruction();
                    break;
                case (0xFC << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xFC << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xFC << 3) | 7:
                    Debug.Assert(false);
                    break;

                // SBC abs,X
                case (0xFD << 3) | 0:
                    Address = PC++;
                    break;
                case (0xFD << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xFD << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    IncrementTimingRegisterIfNoPageCrossing(X);
                    break;
                case (0xFD << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0xFD << 3) | 4:
                    Sbc();
                    FetchNextInstruction();
                    break;
                case (0xFD << 3) | 5:
                    Debug.Assert(false);
                    break;
                case (0xFD << 3) | 6:
                    Debug.Assert(false);
                    break;
                case (0xFD << 3) | 7:
                    Debug.Assert(false);
                    break;

                // INC abs,X - Increment Memory
                case (0xFE << 3) | 0:
                    Address = PC++;
                    break;
                case (0xFE << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xFE << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    break;
                case (0xFE << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0xFE << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xFE << 3) | 5:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo + 1));
                    _rw = false;
                    break;
                case (0xFE << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0xFE << 3) | 7:
                    Debug.Assert(false);
                    break;

                // ISB abs,X - Increment Memory then Subtract (undocumented, also known as ISC)
                case (0xFF << 3) | 0:
                    Address = PC++;
                    break;
                case (0xFF << 3) | 1:
                    Address = PC++;
                    _ad.Lo = _data;
                    break;
                case (0xFF << 3) | 2:
                    _ad.Hi = _data;
                    Address.Hi = _ad.Hi;
                    Address.Lo = (byte)(_ad.Lo + X);
                    break;
                case (0xFF << 3) | 3:
                    Address = _ad + X;
                    break;
                case (0xFF << 3) | 4:
                    _ad.Lo = _data;
                    _rw = false;
                    break;
                case (0xFF << 3) | 5:
                    _data = P.SetZeroNegativeFlags((byte)(_ad.Lo + 1));
                    _rw = false;
                    Sbc();
                    break;
                case (0xFF << 3) | 6:
                    FetchNextInstruction();
                    break;
                case (0xFF << 3) | 7:
                    Debug.Assert(false);
                    break;

                default:
                    throw new InvalidOperationException($"Unimplemented opcode 0x{_ir:X2} timing {_tr}");
            }
        }
    }
}
