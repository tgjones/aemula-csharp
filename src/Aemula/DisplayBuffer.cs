using Veldrid;

namespace Aemula;

public sealed class DisplayBuffer
{
    public readonly uint Width;
    public readonly uint Height;

    public readonly RgbaByte[] Data;

    public DisplayBuffer(uint width, uint height)
    {
        Width = width;
        Height = height;

        Data = new RgbaByte[width * height];

        for (var i = 0; i < Data.Length; i++)
        {
            Data[i] = new RgbaByte(0, 0, 0, 255);
        }
    }
}
