using System;

namespace Aemula.Systems.Nes.Tests.Visual2C02;

internal class Macros
{
    private readonly ChipSim _sim;
    private int _cycle;

    public Macros()
    {
        _sim = new ChipSim(Configuration.ResetStatePreRenderEven);
    }

    // simulate a single clock phase, updating trace and highlighting layout
    public void Step()
    {
        HalfStep();
        _cycle++;
        //ChipStatus();
        //UpdateScope();
        //UpdateVideo();

        Console.WriteLine($"Halfcyc: {_cycle}");
        Console.WriteLine($"Clk: {ReadBit(NodeName.clk0)}");
        Console.WriteLine($"Scanline: {ReadVPos()}");
        Console.WriteLine($"Pixel: {ReadHPos()}");
    }

    // simulate a single clock phase with no update to graphics or trace
    private void HalfStep()
    {
        var clk = _sim.IsNodeHigh(NodeName.clk0);

        _sim.SetNode(NodeName.clk0, !clk);
        
        // TODO: Handle memory reads and writes.
        //HandleChrBus();
    }

    private int ReadBit(NodeName name)
    {
        return _sim.IsNodeHigh(name) ? 1 : 0;
    }

    public ushort ReadHPos()
    {
        Span<NodeName> names = stackalloc NodeName[]
        {
            NodeName.hpos0, 
            NodeName.hpos1, 
            NodeName.hpos2,
            NodeName.hpos3,
            NodeName.hpos4,
            NodeName.hpos5,
            NodeName.hpos6,
            NodeName.hpos7,
            NodeName.hpos8,
        };
        return (ushort)ReadBits(names);
    }

    public ushort ReadVPos()
    {
        Span<NodeName> names = stackalloc NodeName[]
        {
            NodeName.vpos0,
            NodeName.vpos1,
            NodeName.vpos2,
            NodeName.vpos3,
            NodeName.vpos4,
            NodeName.vpos5,
            NodeName.vpos6,
            NodeName.vpos7,
            NodeName.vpos8,
        };
        return (ushort)ReadBits(names);
    }

    private int ReadBits(Span<NodeName> names)
    {
        var res = 0;
        for (var i = 0; i < names.Length; i++)
        {
            res += (_sim.IsNodeHigh(names[i]) ? 1 : 0) << i;
        }
        return res;
    }
}
