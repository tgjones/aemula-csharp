using static Aemula.BitUtility;
using static Aemula.Chips.Tia.TiaUtility;

namespace Aemula.Chips.Tia;

internal sealed class PlayerAndMissile
{
    // Registers
    public byte Graphics;
    public byte Color;
    public byte Luminance;
    public byte NumberSizePlayer;
    public byte NumberSizeMissile;

    // State
    public byte PlayerClockDiv4;
    private byte _counter;
    public bool Reset;
    private bool _draw;
    private byte _graphicsDelay;
    private byte _scanCounter;

    public void UpdatePlayerDiv4()
    {
        PlayerClockDiv4++;

        if (PlayerClockDiv4 > 3)
        {
            PlayerClockDiv4 = 0;

            _counter = UpdatePolynomialCounter(_counter);
            if (_counter == 0b111111 || Reset)
            {
                Reset = false;
                _counter = 0;
            }

            ExecutePlayerLogic();
        }
    }

    private void ExecutePlayerLogic()
    {
        switch (_counter)
        {
            case 0b111000:
                // TODO
                // _player0Draw = true;
                // _player0Bit = 0b111;
                break;

            case 0b101101: // RESET
                Reset = true;
                _draw = true;
                _scanCounter = 0b111;
                break;
        }
    }

    public void DoPlayer(Tia tia)
    {
        // Rotate-left graphicsDelay
        // const leftBit = this.graphicsDelay >> 5;

        if (_graphicsDelay == 1)
        {
            tia.Pins.Lum = Luminance;
            tia.Pins.Col = Color;
        }

        // Handle stretching.
        switch (NumberSizePlayer)
        {
            case 0b101: // Double size player
                if (tia.ClockDiv4 != 0 && tia.ClockDiv4 != 2)
                {
                    return;
                }
                break;

            case 0b111: // Quad size player
                if (tia.ClockDiv4 != 0)
                {
                    return;
                }
                break;
        }

        if (_graphicsDelay == 1)
        {
            _graphicsDelay = 0;
        }
        //_graphicsDelay <<= 1;

        if (_draw)
        {
            if (_scanCounter == 0b000)
            {
                _draw = false;
            }
            // TODO: Handle reflection.
            // Set bottom bit of graphicsDelay.
            //this.player0GraphicsDelay = (this.graphicsDelay & 0b111110) | getBit(this.p0Graphics, this.player0Bit);
            _graphicsDelay = GetBit(Graphics, _scanCounter);
            if (_scanCounter != 0b000)
            {
                _scanCounter--;
            }
        }
    }
}
