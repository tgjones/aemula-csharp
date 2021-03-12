using static Aemula.BitUtility;
using static Aemula.Chips.Tia.TiaUtility;

namespace Aemula.Chips.Tia
{
    public sealed class Tia
    {
        public TiaPins Pins;

        private bool _vsync;
        private bool _vblank;

        private bool _hsync;

        private byte _horizontalCounter;
        private bool _horizontalReset;
        private bool _horizontalBlank;

        /// <summary>
        /// Controls whether latches I4..I5 are enabled.
        /// </summary>
        private bool _i45Enable;

        /// <summary>
        /// Controls whether latches I0..I3 are dumped to ground.
        /// </summary>
        private bool _i03DumpToGround;

        /// <summary>
        /// Stores combined values of PF0, PF1, PF2 registers.
        /// </summary>
        private ushort _playfield;

        internal byte ClockDiv4;

        private PlayerAndMissile _playerAndMissile0;
        private PlayerAndMissile _playerAndMissile1;

        private bool _playerCounterEnable;

        private byte _playfieldIndex;

        private bool _playfieldCanReflect;
        private bool _playfieldReflect;

        private byte _playfieldColor;
        private byte _playfieldLuminance;

        private byte _backgroundColor;
        private byte _backgroundLuminance;

        private bool _hmove;
        private bool _hmp0Latch;
        private bool _hmp1Latch;
        private byte _hmoveComparator;
        private bool _hmoveCounterEnabled;

        // Horizontal motion registers are stored with bit 3 inverted
        private byte _hmp0;
        private byte _hmp1;

        public Tia()
        {
            _playerAndMissile0 = new PlayerAndMissile();
            _playerAndMissile1 = new PlayerAndMissile();

            _hmp0 = 0b1000;
            _hmp1 = 0b1000;
        }

        public void Cycle()
        {
            // TODO

            ClockDiv4++;
            if (ClockDiv4 > 3)
            {
                ClockDiv4 = 0;

                _horizontalCounter = UpdatePolynomialCounter(_horizontalCounter);

                if (_horizontalCounter == 0b111111 || _horizontalReset)
                {
                    _playfieldCanReflect = false;
                    _playfieldIndex = 0;
                    _horizontalReset = false;
                    _horizontalCounter = 0;
                    _horizontalBlank = true;
                    Pins.Rdy = false;
                }

                ExecuteClockLogic();

                if (_hmoveCounterEnabled)
                {
                    if (NoneEqual(_hmoveComparator, _hmp0))
                    {
                        _hmp0Latch = false;
                    }

                    if (NoneEqual(_hmoveComparator, _hmp1))
                    {
                        _hmp1Latch = false;
                    }

                    _hmoveComparator = (byte)((_hmoveComparator - 1) & 0b1111);
                    if (_hmoveComparator == 0b1111)
                    {
                        _hmoveCounterEnabled = false;
                    }
                }

                if (_hmp0Latch)
                {
                    _playerAndMissile0.UpdatePlayerDiv4();
                }

                if (_hmp1Latch)
                {
                    _playerAndMissile1.UpdatePlayerDiv4();
                }
            }

            if (_playerCounterEnable)
            {
                _playerAndMissile0.UpdatePlayerDiv4();
                _playerAndMissile1.UpdatePlayerDiv4();
            }

            DoPlayfield();

            Pins.Blk = _vblank || _horizontalBlank;
            Pins.Sync = _vsync || _hsync;
        }

        private void ExecuteClockLogic()
        {
            switch (_horizontalCounter)
            {
                case 0b111100: // Set HSYNC
                    _hsync = true;
                    break;

                case 0b110111: // Reset HSYNC
                    _hsync = false;
                    break;

                case 0b001111: // ColorBurst
                    break;

                case 0b011100: // Reset HBLANK
                    _playfieldIndex = 0;
                    if (!_hmove)
                    {
                        _horizontalBlank = false;
                        _playerCounterEnable = true;
                    }
                    break;

                case 0b010111: // Late Reset HBLANK, if HMOVE activated
                    _playfieldIndex = 2;
                    if (_hmove)
                    {
                        _horizontalBlank = false;
                        _playerCounterEnable = true;
                    }
                    break;

                case 0b101100: // Center
                    _playfieldCanReflect = true;
                    _playfieldIndex = 0;
                    break;

                case 0b010100: // RESET
                    _playerCounterEnable = false;
                    _playfieldIndex = 0x14;
                    _horizontalReset = true;
                    _hmove = false;
                    // TODO: Tick audio
                    break;

                default:
                    _playfieldIndex++;
                    break;
            }
        }

        private void DoPlayfield()
        {
            // TODO: Reflect playfield

            var shouldOutputPlayfield = (_playfieldCanReflect && _playfieldReflect)
                ? GetBitAsBoolean(_playfield, _playfieldIndex)
                : GetBitAsBoolean(_playfield, 19 - _playfieldIndex);

            if (shouldOutputPlayfield)
            {
                Pins.Lum = _playfieldLuminance;
                Pins.Col = _playfieldColor;
            }
            else
            {
                Pins.Lum = _backgroundLuminance;
                Pins.Col = _backgroundColor;
            }

            _playerAndMissile0.DoPlayer(this);
            _playerAndMissile1.DoPlayer(this);

            if (_horizontalBlank || _vblank)
            {
                Pins.Lum = 0;
                Pins.Col = 0;
            }
        }

        public void CpuCycle()
        {
            ref var pins = ref Pins;

            if (pins.RW)
            {
                // Read registers.
                //console.log(`TIA read register. Address = ${toHexString(pins.address, 2)}`);

                switch (pins.Address)
                {
                    // CXM0P - Read collision
                    case 0x00:
                        break;

                    // TODO

                    // Ignore invalid addresses
                    default:
                        break;
                }
            }
            else
            {
                // Write registers.
                //console.log(`TIA write register. Address = ${toHexString(pins.address, 2)}. Data67 = ${toHexString(pins.data67, 2)}, Data05 = ${toHexString(pins.data05, 2)}`);

                switch (pins.Address)
                {
                    // VSYNC - Vertical sync set/clear
                    case 0x00:
                        _vsync = GetBitAsBoolean(pins.Data05, 1);
                        break;

                    // VBLANK - Vertical blank set/clear
                    case 0x01:
                        _vblank = GetBitAsBoolean(pins.Data05, 1);
                        _i45Enable = GetBitAsBoolean(pins.Data67, 0);
                        _i03DumpToGround = GetBitAsBoolean(pins.Data67, 1);
                        break;

                    // WSYNC - Wait for sync. Halts microprocessor by clearing RDY latch to zero.
                    // RDY is set to false again by leading edge of horizontal blank.
                    case 0x02:
                        pins.Rdy = true;
                        break;

                    // RSYNC - Reset horizontal sync counter.
                    case 0x03:
                        _horizontalReset = true;
                        break;

                    // NUSIZ0 - Number-size player-missile 0
                    case 0x04:
                        _playerAndMissile0.NumberSizePlayer = (byte)(pins.Data05 & 0b111);
                        _playerAndMissile0.NumberSizeMissile = (byte)(pins.Data05 >> 3);
                        break;

                    // NUSIZ1 - Number-size player-missile 1
                    case 0x05:
                        _playerAndMissile1.NumberSizePlayer = (byte)(pins.Data05 & 0b111);
                        _playerAndMissile1.NumberSizeMissile = (byte)(pins.Data05 >> 3);
                        break;

                    // COLUP0 - Color-luminance player 0
                    case 0x06:
                        _playerAndMissile0.Color = (byte)((pins.Data05 >> 4) | (pins.Data67 << 2));
                        _playerAndMissile0.Luminance = (byte)((pins.Data05 >> 1) & 0b111);
                        break;

                    // COLUP1 - Color-luminance player 1
                    case 0x07:
                        _playerAndMissile1.Color = (byte)((pins.Data05 >> 4) | (pins.Data67 << 2));
                        _playerAndMissile1.Luminance = (byte)((pins.Data05 >> 1) & 0b111);
                        break;

                    // COLUPF - Color-luminance playfield
                    case 0x08:
                        _playfieldColor = (byte)((pins.Data05 >> 4) | (pins.Data67 << 2));
                        _playfieldLuminance = (byte)((pins.Data05 >> 1) & 0b111);
                        break;

                    // COLUBK - Color-luminance background
                    case 0x09:
                        _backgroundColor = (byte)((pins.Data05 >> 4) | (pins.Data67 << 2));
                        _backgroundLuminance = (byte)((pins.Data05 >> 1) & 0b111);
                        break;

                    // CTRLPF - Control playfield ball size and collisions
                    case 0x0A:
                        // TODO
                        _playfieldReflect = GetBitAsBoolean(pins.Data05, 0);
                        break;

                    // REFP0 - Reflect player 0
                    case 0x0B:
                        break;

                    // REFP1 - Reflect player 1
                    case 0x0C:
                        break;

                    // PF0 - Playfield register byte 0
                    case 0x0D:
                        {
                            var temp =
                                (GetBit(pins.Data05, 4) << 3) |
                                (GetBit(pins.Data05, 5) << 2) |
                                (GetBit(pins.Data67, 0) << 1) |
                                (GetBit(pins.Data67, 1) << 0);
                            _playfield = (byte)((temp << 16) | (_playfield & 0xFFFF));
                            break;
                        }

                    // PF1 - Playfield register byte 1
                    case 0x0E:
                        {
                            var temp = (byte)(pins.Data05 | (pins.Data67 << 6));
                            _playfield = (byte)((_playfield & 0xF0000) | (temp << 8) | (_playfield & 0xFF));
                            break;
                        }

                    // PF2 - Playfield register byte 2
                    case 0x0F:
                        {
                            var temp =
                                (GetBit(pins.Data05, 0) << 7) |
                                (GetBit(pins.Data05, 1) << 6) |
                                (GetBit(pins.Data05, 2) << 5) |
                                (GetBit(pins.Data05, 3) << 4) |
                                (GetBit(pins.Data05, 4) << 3) |
                                (GetBit(pins.Data05, 5) << 2) |
                                (GetBit(pins.Data67, 0) << 1) |
                                (GetBit(pins.Data67, 1) << 0);
                            _playfield = (byte)((_playfield & 0xFFF00) | temp);
                            break;
                        }

                    // RESP0 - Reset player 0
                    case 0x10:
                        _playerAndMissile0.Reset = true;
                        _playerAndMissile0.PlayerClockDiv4 = 0;
                        break;

                    // RESP1 - Reset player 1
                    case 0x11:
                        _playerAndMissile1.Reset = true;
                        _playerAndMissile1.PlayerClockDiv4 = 0;
                        break;

                    // RESM0 - Reset missile 0
                    case 0x12:
                        break;

                    // RESM1 - Reset missile 1
                    case 0x13:
                        break;

                    // RESBL - Reset ball
                    case 0x14:
                        break;

                    // AUDC0 - Audio control 0
                    case 0x15:
                        break;

                    // AUDC1 - Audio control 1
                    case 0x16:
                        break;

                    // AUDF0 - Audio frequency 0
                    case 0x17:
                        break;

                    // AUDF1 - Audio frequency 1
                    case 0x18:
                        break;

                    // AUDV0 - Audio volume 0
                    case 0x19:
                        break;

                    // AUDv1 - Audio volume 1
                    case 0x1A:
                        break;

                    // GRP0 - Graphics player 0
                    case 0x1B:
                        _playerAndMissile0.Graphics = (byte)(pins.Data05 | (pins.Data67 << 6));
                        break;

                    // GRP1 - Graphics player 1
                    case 0x1C:
                        _playerAndMissile1.Graphics = (byte)(pins.Data05 | (pins.Data67 << 6));
                        break;

                    // ENAM0 - Graphics (enable) missile 0
                    case 0x1D:
                        break;

                    // ENAM1 - Graphics (enable) missile 1
                    case 0x1E:
                        break;

                    // ENABL - Graphics (enable) ball
                    case 0x1F:
                        break;

                    // HMP0 - Horizontal motion player 0
                    case 0x20:
                        // Invert HM bit 3 to simplify counting
                        _hmp0 = (byte)
                            ((pins.Data05 >> 4) |
                            ((pins.Data67 & 1) << 2) |
                            ((pins.Data67 >> 1) == 1 ? 0b0000 : 0b1000));
                        break;

                    // HMP1 - Horizontal motion player 1
                    case 0x21:
                        _hmp1 = (byte)
                            ((pins.Data05 >> 4) |
                            ((pins.Data67 & 1) << 2) |
                            ((pins.Data67 >> 1) == 1 ? 0b0000 : 0b1000));
                        break;

                    // HMM0 - Horizontal motion missile 0
                    case 0x22:
                        break;

                    // HMM1 - Horizontal motion missile 1
                    case 0x23:
                        break;

                    // HMBL - Horizontal motion ball
                    case 0x24:
                        break;

                    // VDELP0 - Vertical delay player 0
                    case 0x25:
                        break;

                    // VDELP1 - Vertical delay player 1
                    case 0x26:
                        break;

                    // VDELBL - Vertical delay ball
                    case 0x27:
                        break;

                    // RESMP0 - Reset missile 0 to player 0
                    case 0x28:
                        break;

                    // RESMP1 - Reset missile 1 to player 1
                    case 0x29:
                        break;

                    // HMOVE - Apply horizontal motion
                    case 0x2A:
                        _hmove = true;
                        _hmp0Latch = true;
                        _hmp1Latch = true;
                        _hmoveComparator = 0b1111;
                        _hmoveCounterEnabled = true;
                        break;

                    // HMCLR - Clear horizontal motion registers
                    case 0x2B:
                        _hmp0 = 0b1000;
                        _hmp1 = 0b1000;
                        break;

                    // CXCLR - Clear collision latches
                    case 0x2C:
                        break;

                    // Ignore invalid addresses
                    default:
                        break;
                }
            }
        }
    }
}
