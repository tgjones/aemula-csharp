using System;
using System.Collections;
using System.Collections.Generic;

namespace Aemula.Systems.Nes.Tests.Visual2C02;

internal class ChipSim
{
    private readonly Node[] _nodes;

    private readonly ushort _nodeGnd;
    private readonly ushort _nodePwr;

    private readonly BitArray _recalcListOutBitmap;

    private List<ushort> _recalcListIn;
    private List<ushort> _recalcListOut;

    private readonly List<ushort> _group;

    private GroupState _groupState;

    private enum GroupState
    {
        ContainsNothing,
        ContainsHi,
        ContainsPulldown,
        ContainsPullup,
        ContainsPwr,
        ContainsGnd,
        ContainsGndAndPwr,
    }

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

        _recalcListOutBitmap = new BitArray(_nodes.Length);

        _recalcListIn = new List<ushort>();
        _recalcListOut = new List<ushort>();

        _group = new List<ushort>();

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

    public void RecalcNodeList()
    {
        for (var j = 0; j < 100; j++) // Prevent infinite loops
        {
            var tmp = _recalcListIn;
            _recalcListIn = _recalcListOut;
            _recalcListOut = tmp;

            if (_recalcListIn.Count == 0)
            {
                return;
            }

            _recalcListOut.Clear();
            _recalcListOutBitmap.SetAll(false);

            foreach (var item in _recalcListIn)
            {
                RecalcNode(item);
            }
        }

        throw new Exception("Encountered loop while updating");
    }

    private void RecalcNode(ushort node)
    {
        GetNodeGroup(node);

        var newState = GetNodeValue();

        foreach (var i in _group)
        {
            ref var n = ref _nodes[i];

            if (n.State == newState)
            {
                continue;
            }

            n.State = newState;

            foreach (var t in n.Gates)
            {
                if (n.State)
                {
                    TurnTransistorOn(t);
                }
                else
                {
                    TurnTransistorOff(t);
                }
            }
        }
    }

    private void TurnTransistorOn(Transistor t)
    {
        if (t.On)
        {
            return;
        }

        t.On = true;

        AddRecalcNode(t.C1);
    }

    private void TurnTransistorOff(Transistor t)
    {
        if (!t.On)
        {
            return;
        }

        t.On = false;

        AddRecalcNode(t.C1);
        AddRecalcNode(t.C2);
    }

    private void AddRecalcNode(ushort nn)
    {
        if (nn == _nodeGnd) return;
        if (nn == _nodePwr) return;

        if (_recalcListOutBitmap[nn])
        {
            return;
        }

        _recalcListOut.Add(nn);
        _recalcListOutBitmap[nn] = true;
    }

    private void GetNodeGroup(ushort i)
    {
        _group.Clear();
        _groupState = GroupState.ContainsNothing;
        AddNodeToGroup(i);
    }

    private void AddNodeToGroup(ushort i)
    {
        if (i == _nodeGnd)
        {
            _groupState = _groupState == GroupState.ContainsPwr || _groupState == GroupState.ContainsGndAndPwr
                ? GroupState.ContainsGndAndPwr
                : GroupState.ContainsGnd;
            return;
        }

        if (i == _nodePwr)
        {
            _groupState = _groupState == GroupState.ContainsGnd || _groupState == GroupState.ContainsGndAndPwr
                ? GroupState.ContainsGndAndPwr
                : GroupState.ContainsPwr;
            return;
        }

        if (_group.Contains(i))
        {
            return;
        }

        _group.Add(i);

        ref readonly var node = ref _nodes[i];

        if (_groupState < GroupState.ContainsPullup && node.Pullup)
        {
            _groupState = GroupState.ContainsPullup;
        }

        if (_groupState < GroupState.ContainsPulldown && node.Pulldown)
        {
            _groupState = GroupState.ContainsPulldown;
        }

        if (_groupState < GroupState.ContainsHi && node.State)
        {
            _groupState = GroupState.ContainsHi;
        }

        foreach (var t in node.C1C2s)
        {
            if (!t.On)
            {
                continue;
            }

            ushort other;
            if (t.C1 == i) other = t.C2;
            else if (t.C2 == i) other = t.C1;
            else throw new InvalidOperationException();
            AddNodeToGroup(other);
        }
    }

    private bool GetNodeValue()
    {
        switch (_groupState)
        {
            case GroupState.ContainsGndAndPwr:
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
                {
                    goto case GroupState.ContainsHi;
                }
                else
                {
                    return false;
                }

            case GroupState.ContainsNothing:
            case GroupState.ContainsPulldown:
            case GroupState.ContainsGnd:
                return false;

            case GroupState.ContainsPullup:
            case GroupState.ContainsPwr:
                return true;

            case GroupState.ContainsHi:
                var areaHi = 0;
                var areaLo = 0;
                foreach (var nn in _group)
                {
                    ref readonly var n = ref _nodes[nn];

                    // If we get here, we know that none of the nodes
                    // in the group are gnd, pwr, pullup, or pulldown.

                    if (n.State)
                    {
                        areaHi += n.Area;
                    }
                    else
                    {
                        areaLo += n.Area;
                    }
                }
                return areaHi > areaLo;

            default:
                throw new InvalidOperationException();
        }
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

        _recalcListOut.Add(nn);

        RecalcNodeList();
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
