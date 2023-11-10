using System;
using System.Collections.Generic;

namespace Aemula.Systems.Nes.Tests.Visual2C02;

internal class Wires
{
    public readonly Node[] Nodes;
    public readonly Dictionary<string, Transistor> Transistors = new Dictionary<string, Transistor>();

    public readonly ushort NGnd;
    public readonly ushort NPwr;

    public Wires()
    {
        NGnd = (int)NodeName.gnd;
        NPwr = (int)NodeName.pwr;

        // Check maximum node ID.
        var maximumId = 0;
        foreach (var segmentDefinition in Configuration.SegmentDefinitions)
        {
            maximumId = Math.Max(maximumId, segmentDefinition.Node);
        }

        Nodes = new Node[maximumId + 1];

        SetupNodes();
        SetupTransistors();
    }

    private void SetupNodes()
    {
        foreach (var seg in Configuration.SegmentDefinitions)
        {
            var w = seg.Node;
            var node = Nodes[w];
            if (node == null)
            {
                node = new Node();
                node.Num = w;
                node.Pullup = seg.Pullup;
                node.State = false;
                node.Area = 0;
                Nodes[w] = node;
            }
            if (w == NGnd) continue;
            if (w == NPwr && seg.Unknown == 4) continue;
            if (w == NPwr) continue;

            node.Area += seg.Area;
        }
    }

    private void SetupTransistors()
    {
        foreach (var tdef in Configuration.TransistorDefinitions)
        {
            var name = tdef.Name;
            var gate = tdef.Gate;
            var c1 = tdef.C1;
            var c2 = tdef.C2;
            if (c1 == NGnd) { c1 = c2; c2 = NGnd; }
            if (c1 == NPwr) { c1 = c2; c2 = NPwr; }
            var trans = new Transistor
            {
                Name = tdef.Name,
                On = false,
                Gate = tdef.Gate,
                C1 = c1,
                C2 = c2,
            };
            Nodes[gate].Gates.Add(trans);
            Nodes[c1].C1C2s.Add(trans);
            Nodes[c2].C1C2s.Add(trans);
            Transistors[name] = trans;
        }
    }
}
