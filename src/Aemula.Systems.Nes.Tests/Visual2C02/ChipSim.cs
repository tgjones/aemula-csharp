using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aemula.Systems.Nes.Tests.Visual2C02;

internal class ChipSim
{
    private readonly Node[] _nodes;

    private readonly ushort _nodeGnd;
    private readonly ushort _nodePwr;

    //private bool _ctrace;
    //private int[] _traceTheseNodes;
    //private int _traceTheseTransistors;
    //private int _logLevel;
    private List<ushort> _recalcList;
    private HashSet<ushort> _recalcHash;
    private List<ushort> _group;

    public ChipSim(string state)
    {
        _nodeGnd = (int)NodeName.gnd;
        _nodePwr = (int)NodeName.pwr;

        // Check maximum node ID.
        var maximumId = 0;
        foreach (var segmentDefinition in Configuration.SegmentDefinitions)
        {
            maximumId = Math.Max(maximumId, segmentDefinition.Node);
        }

        _nodes = new Node[maximumId + 1];
        for (var i = 0; i < _nodes.Length; i++)
        {
            _nodes[i].Num = ushort.MaxValue;
        }

        SetupNodes();
        SetupTransistors();

        SetState(state);
    }

    private void SetupNodes()
    {
        foreach (var seg in Configuration.SegmentDefinitions)
        {
            var w = seg.Node;
            ref var node = ref _nodes[w];
            if (node.Num == ushort.MaxValue)
            {
                node = new Node();
                node.Num = w;
                node.Pullup = seg.Pullup;
                node.State = false;
                node.Area = 0;
                node.Gates = new List<Transistor>();
                node.C1C2s = new List<Transistor>();
            }
            if (w == _nodeGnd) continue;
            if (w == _nodePwr && seg.Unknown == 4) continue;
            if (w == _nodePwr) continue;

            node.Area += seg.Area;
        }
    }

    private void SetupTransistors()
    {
        foreach (var tdef in Configuration.TransistorDefinitions)
        {
            var gate = tdef.Gate;
            var c1 = tdef.C1;
            var c2 = tdef.C2;
            if (c1 == _nodeGnd) { c1 = c2; c2 = _nodeGnd; }
            if (c1 == _nodePwr) { c1 = c2; c2 = _nodePwr; }
            var trans = new Transistor
            {
                On = false,
                Gate = tdef.Gate,
                C1 = c1,
                C2 = c2,
            };
            _nodes[gate].Gates.Add(trans);
            _nodes[c1].C1C2s.Add(trans);
            _nodes[c2].C1C2s.Add(trans);
        }
    }

    public bool IsNodeHigh(NodeName nn)
    {
        return _nodes[(int)nn].State;
    }

    public void RecalcNodeList(List<ushort> list)
    {
        var n = list[0];

        _recalcList = new List<ushort>();
        _recalcHash = new HashSet<ushort>();

        for (var j = 0; j < 100; j++) // loop limiter
        {
            if (j == 99) Debug.WriteLine("Encountered loop!");
            if (list.Count == 0) return;
            //if (_ctrace)
            //{
            //    var i;
            //    for (i = 0; i < traceTheseNodes.length; i++)
            //    {
            //        if (list.indexOf(traceTheseNodes[i]) != -1) break;
            //    }
            //    if ((traceTheseNodes.length == 0) || (list.indexOf(traceTheseNodes[i]) == -1))
            //    {
            //        console.log('recalcNodeList iteration: ', j, ' ', list.length, ' nodes');
            //    }
            //    else
            //    {
            //        console.log('recalcNodeList iteration: ', j, ' ', list.length, ' nodes ', list);
            //    }
            //}
            foreach (var item in list)
            {
                RecalcNode(item);
            }
            list = _recalcList;
            _recalcList = new List<ushort>();
            _recalcHash = new HashSet<ushort>();
        }
        throw new Exception("Encountered loop while updating " + n + " - " + list + " still pending");
        //if (_ctrace) Debug.WriteLine(n, " looping...");
    }

    private void RecalcNode(ushort node)
    {
        if (node == _nodeGnd)
        {
            return;
        }

        if (node == _nodePwr)
        {
            return;
        }

        GetNodeGroup(node);

        var newState = GetNodeValue();

        //if (_ctrace)
        //{a
        //    var i;
        //    for (i = 0; i < group.length; i++)
        //    {
        //        if (traceTheseNodes.indexOf(group[i]) != -1) break;
        //    }
        //    if ((traceTheseNodes.indexOf(node) != -1) || (i != group.length))
        //        console.log('recalc ', node, ' ', group, ' to ', newState);
        //}

        foreach (var i in _group)
        { 
            if (i == _nodePwr || i == _nodeGnd) continue;
            ref var n = ref _nodes[i];
            if (n.State == newState) continue;
            n.State = newState;
            foreach (var t in n.Gates)
            {
                if (n.State) TurnTransistorOn(t);
                else TurnTransistorOff(t);
            }
        }
    }

    private void TurnTransistorOn(Transistor t)
    {
        if (t.On) return;
        //if (_ctrace && ((traceTheseTransistors.indexOf(t.name) != -1) || (traceTheseNodes.indexOf(t.c1) != -1) || (traceTheseNodes.indexOf(t.c2) != -1)))
        //    console.log(t.name, ' on ', t.gate, ' ', t.c1, ' ', t.c2);
        t.On = true;
        AddRecalcNode(t.C1);
    }

    private void TurnTransistorOff(Transistor t)
    {
        if (!t.On) return;
        //if (ctrace && ((traceTheseTransistors.indexOf(t.name) != -1) || (traceTheseNodes.indexOf(t.c1) != -1) || (traceTheseNodes.indexOf(t.c2) != -1)))
        //    console.log(t.name, ' off ', t.gate, ' ', t.c1, ' ', t.c2);
        t.On = false;
        AddRecalcNode(t.C1);
        AddRecalcNode(t.C2);
    }

    private void AddRecalcNode(ushort nn)
    {
        if (nn == _nodeGnd) return;
        if (nn == _nodePwr) return;
        if (_recalcHash.Contains(nn)) return;
        _recalcList.Add(nn);
        _recalcHash.Add(nn);
    }

    private void GetNodeGroup(ushort i)
    {
        _group = new List<ushort>();
        AddNodeToGroup(i);
    }

    private void AddNodeToGroup(ushort i)
    {
        if (_group.Contains(i))
        {
            return;
        }

        _group.Add(i);

        if (i == _nodeGnd) return;
        if (i == _nodePwr) return;

        ref readonly var node = ref _nodes[i];

        foreach (var t in node.C1C2s)
        {
            if (!t.On) continue;
            ushort other;
            if (t.C1 == i) other = t.C2;
            else if (t.C2 == i) other = t.C1;
            else throw new InvalidOperationException();
            AddNodeToGroup(other);
        }
    }

    private bool GetNodeValue()
    {
        var gnd = _group.Contains(_nodeGnd);
        var pwr = _group.Contains(_nodePwr);
        if (pwr && gnd)
        {
            // spr_d0 thru spr_d7 sometimes get conflicts,
            // so suppress them here
            if (_group.Contains(359) ||
                _group.Contains(566) ||
                _group.Contains(691) ||
                _group.Contains(871) ||
                _group.Contains(870) ||
                _group.Contains(864) ||
                _group.Contains(856) ||
                _group.Contains(818))
                gnd = pwr = false;
        }
        if (gnd) return false;
        if (pwr) return true;
        var hi_area = 0;
        var lo_area = 0;
        foreach (var nn in _group)
        {
            // In case we hit one of the special cases above
            if (nn == _nodeGnd || nn == _nodePwr)
            {
                continue;
            }
            
            ref readonly var n = ref _nodes[nn];

            if (n.Pullup)
            {
                return true;
            }

            if (n.Pulldown)
            {
                return false;
            }

            if (n.State)
            {
                hi_area += n.Area;
            }
            else
            {
                lo_area += n.Area;
            }
        }
        return hi_area > lo_area;
    }

    public void SetState(string str)
    {
        var codes = new Dictionary<char, bool>
        {
            { 'g', false },
            { 'h', true },
            { 'v', true },
            { 'l', false },
        };

        for (var i = 0; i < str.Length; i++)
        {
            var c = str[i];
            if (c == 'x') continue;
            var state = codes[c];

            ref var node = ref _nodes[i];

            if (node.Num == ushort.MaxValue)
            {
                continue;
            }

            node.State = state;
            foreach (var gate in node.Gates)
            {
                gate.On = state;
            }
        }
    }

    public void SetNode(NodeName name, bool value)
    {
        var nn = (ushort)name;
        ref var node = ref _nodes[nn];

        node.Pullup = value;
        node.Pulldown = !value;

        RecalcNodeList([nn]);
    }
}

public struct Node
{
    public ushort Num;
    public bool Pullup;
    public bool Pulldown;
    public bool State;
    public List<Transistor> Gates;
    public List<Transistor> C1C2s;
    public int Area;
}

public class Transistor
{
    public bool On;
    public ushort Gate;
    public ushort C1;
    public ushort C2;
}
