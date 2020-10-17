using System;
using System.Collections.Generic;
using Aemula.Chips.Ricoh2C02.UI;
using Aemula.UI;

namespace Aemula.Chips.Ricoh2C02
{
    // https://wiki.nesdev.com/w/index.php/PPU_registers
    public sealed partial class Ricoh2C02
    {
        private const byte PpuCtrlAddress = 0x0;
        private const byte PpuMaskAddress = 0x1;
        private const byte PpuStatusAddress = 0x2;
        private const byte OamAddrAddress = 0x3;
        private const byte OamDataAddress = 0x4;
        private const byte PpuScrollAddress = 0x5;
        private const byte PpuAddrAddress = 0x6;
        private const byte PpuDataAddress = 0x7;

        private readonly byte[] _objectAttributeMemory;
        private byte _oamAddress;

        private readonly Color[] _systemPalette;
        private readonly byte[] _paletteMemory;

        private byte _ppuScrollPositionX;
        private byte _ppuScrollPositionY;

        private ushort _ppuAddress;

        private byte _currentLatchData;

        // Registers
        private PpuCtrlRegister _ppuCtrlRegister;
        private PpuMaskRegister _ppuMaskRegister;
        private PpuStatusRegister _ppuStatusRegister;

        // Latch around two-bytes writes into 0x2005 and 0x2006
        private bool _firstWrite = true;

        public Ricoh2C02()
        {
            _objectAttributeMemory = new byte[256];

            _systemPalette = new[]
            {
                new Color(84, 84, 84),
                new Color(0, 30, 116),
                new Color(8, 16, 144),
                new Color(48, 0, 136),
                new Color(68, 0, 100),
                new Color(92, 0, 48),
                new Color(84, 4, 0),
                new Color(60, 24, 0),
                new Color(32, 42, 0),
                new Color(8, 58, 0),
                new Color(0, 64, 0),
                new Color(0, 60, 0),
                new Color(0, 50, 60),
                new Color(0, 0, 0),
                new Color(0, 0, 0),
                new Color(0, 0, 0),

                new Color(152, 150, 152),
                new Color(8, 76, 196),
                new Color(48, 50, 236),
                new Color(92, 30, 228),
                new Color(136, 20, 176),
                new Color(160, 20, 100),
                new Color(152, 34, 32),
                new Color(120, 60, 0),
                new Color(84, 90, 0),
                new Color(40, 114, 0),
                new Color(8, 124, 0),
                new Color(0, 118, 40),
                new Color(0, 102, 120),
                new Color(0, 0, 0),
                new Color(0, 0, 0),
                new Color(0, 0, 0),

                new Color(236, 238, 236),
                new Color(76, 154, 236),
                new Color(120, 124, 236),
                new Color(176, 98, 236),
                new Color(228, 84, 236),
                new Color(236, 88, 180),
                new Color(236, 106, 100),
                new Color(212, 136, 32),
                new Color(160, 170, 0),
                new Color(116, 196, 0),
                new Color(76, 208, 32),
                new Color(56, 204, 108),
                new Color(56, 180, 204),
                new Color(60, 60, 60),
                new Color(0, 0, 0),
                new Color(0, 0, 0),

                new Color(236, 238, 236),
                new Color(168, 204, 236),
                new Color(188, 188, 236),
                new Color(212, 178, 236),
                new Color(236, 174, 236),
                new Color(236, 174, 212),
                new Color(236, 180, 176),
                new Color(228, 196, 144),
                new Color(204, 210, 120),
                new Color(180, 222, 120),
                new Color(168, 226, 144),
                new Color(152, 226, 180),
                new Color(160, 214, 228),
                new Color(160, 162, 160),
                new Color(0, 0, 0),
                new Color(0, 0, 0),
            };

            _paletteMemory = new byte[32];
        }

        public void Cycle()
        {
            // TODO
        }

        public void CpuCycle(ref Ricoh2C02Pins pins)
        {
            if (pins.CpuRW)
            {
                var result = _currentLatchData;

                switch (pins.CpuAddress)
                {
                    case PpuCtrlAddress: // Write-only
                        break;

                    case PpuMaskAddress: // Write-only
                        break;

                    case PpuStatusAddress:
                        _ppuStatusRegister.VBlankStarted = true; // HACK: Remove this.
                        _ppuStatusRegister.Unused = _currentLatchData;
                        result = _ppuStatusRegister.Data.Value;
                        _ppuStatusRegister.VBlankStarted = false;
                        _firstWrite = true;
                        break;

                    case OamAddrAddress: // Write-only
                        break;

                    case OamDataAddress:
                        result = _objectAttributeMemory[_oamAddress];
                        break;

                    case PpuScrollAddress: // Write-only
                        break;

                    case PpuAddrAddress: // Write-only
                        break;

                    case PpuDataAddress:
                        result = PpuRead(_ppuAddress, ref pins);
                        IncrementPpuAddress();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _currentLatchData = result;

                pins.CpuData = result;
            }
            else
            {
                _currentLatchData = pins.CpuData;

                switch (pins.CpuAddress)
                {
                    case PpuCtrlAddress:
                        _ppuCtrlRegister.Data.Value = pins.CpuData;
                        // TODO: If we're in vblank, and _ppuStatusRegister.VBlankStarted is set, changing NMI flag from 0 to 1 should trigger NMI.
                        break;

                    case PpuMaskAddress:
                        _ppuMaskRegister.Data.Value = pins.CpuData;
                        break;

                    case PpuStatusAddress: // Read-only
                        break;

                    case OamAddrAddress:
                        _oamAddress = pins.CpuData;
                        break;

                    case OamDataAddress:
                        _objectAttributeMemory[_oamAddress] = pins.CpuData;
                        _oamAddress++;
                        break;

                    case PpuScrollAddress:
                        if (_firstWrite)
                        {
                            _ppuScrollPositionX = pins.CpuData;
                            _firstWrite = false;
                        }
                        else
                        {
                            _ppuScrollPositionY = pins.CpuData;
                            _firstWrite = true;
                        }
                        break;

                    case PpuAddrAddress:
                        if (_firstWrite)
                        {
                            // Write high byte.
                            _ppuAddress = (ushort)((pins.CpuData << 8) | (_ppuAddress & 0xFF));
                            _firstWrite = false;
                        }
                        else
                        {
                            // Write low byte.
                            _ppuAddress = (ushort)((_ppuAddress & 0xFF00) | pins.CpuData);
                            _firstWrite = true;
                        }
                        break;

                    case PpuDataAddress:
                        PpuWrite(_ppuAddress, pins.CpuData, ref pins);
                        IncrementPpuAddress();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void IncrementPpuAddress()
        {
            if (_ppuCtrlRegister.VRamAddressIncrementMode == VRamAddressIncrementMode.Add32)
            {
                _ppuAddress += 32;
            }
            else
            {
                _ppuAddress++;
            }
        }

        private byte ReadPaletteMemory(ushort address)
        {
            if (_ppuMaskRegister.Grayscale)
            {
                address &= 0x30;
            }
            return _paletteMemory[GetPaletteAddress(address)];
        }

        private byte PpuRead(ushort address, ref Ricoh2C02Pins pins)
        {
            var result = pins.PpuData;

            if (address >= 0x3F00 && address <= 0x3FFF)
            {
                result = pins.PpuData = ReadPaletteMemory(address);
            }
            else
            {
                // Return the contents of the internal read buffer,
                // and set the PPU pins so we'll have the actual data available
                // in the next cycle.
                pins.PpuAddress = address;
                pins.PpuRW = true;
            }

            return result;
        }

        private void PpuWrite(ushort address, byte data, ref Ricoh2C02Pins pins)
        {
            if (address >= 0x3F00 && address <= 0x3FFF)
            {
                _paletteMemory[GetPaletteAddress(address)] = data;
            }

            // TODO: This currently means we'll write to the VRAM range between 0x3F00..0x3FFF.
            pins.PpuAddress = address;
            pins.PpuData = data;
            pins.PpuRW = false;
        }

        private static ushort GetPaletteAddress(ushort address)
        {
            address &= 0x1F;
            switch (address)
            {
                case 0x10:
                    address = 0x00;
                    break;
                case 0x14:
                    address = 0x04;
                    break;
                case 0x18:
                    address = 0x08;
                    break;
                case 0x1C:
                    address = 0x0C;
                    break;
            }
            return address;
        }

        public IEnumerable<DebuggerWindow> CreateDebuggerWindows()
        {
            yield return new PaletteWindow(this);
        }

        internal Color GetColor(ushort address)
        {
            var paletteId = ReadPaletteMemory(address);
            return _systemPalette[paletteId];
        }
    }
}
