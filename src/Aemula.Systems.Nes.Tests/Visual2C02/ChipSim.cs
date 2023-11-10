using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aemula.Systems.Nes.Tests.Visual2C02;

internal class ChipSim
{
    private readonly Wires _wires;

    //private bool _ctrace;
    //private int[] _traceTheseNodes;
    //private int _traceTheseTransistors;
    //private int _logLevel;
    private List<ushort> _recalcList;
    private HashSet<ushort> _recalcHash;
    private List<ushort> _group;

    public ChipSim(string state)
    {
        _wires = new Wires();
        SetState(state);
    }

    public bool IsNodeHigh(NodeName nn)
    {
        return _wires.Nodes[(int)nn].State;
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
        if (node == _wires.NGnd)
        {
            return;
        }

        if (node == _wires.NPwr)
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
            if (i == _wires.NPwr || i == _wires.NGnd) continue;
            var n = _wires.Nodes[i];
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
        if (nn == _wires.NGnd) return;
        if (nn == _wires.NPwr) return;
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

        if (i == _wires.NGnd) return;
        if (i == _wires.NPwr) return;

        foreach (var t in _wires.Nodes[i].C1C2s)
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
        var gnd = _group.Contains(_wires.NGnd);
        var pwr = _group.Contains(_wires.NPwr);
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
            if (nn == _wires.NGnd || nn == _wires.NPwr) continue;
            var n = _wires.Nodes[nn];
            if (n.Pullup) return true;
            if (n.Pulldown) return false;
            if (n.State) hi_area += n.Area;
            else lo_area += n.Area;
        }
        return hi_area > lo_area;
    }

    private void SetState(string str)
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
            if (_wires.Nodes[i] == null) continue;
            _wires.Nodes[i].State = state;
            foreach (var gate in _wires.Nodes[i].Gates)
            {
                gate.On = state;
            }
        }
    }

    public void SetNode(NodeName name, bool value)
    {
        var nn = (ushort)name;
        _wires.Nodes[nn].Pullup = value;
        _wires.Nodes[nn].Pulldown = !value;

        RecalcNodeList([nn]);
    }
}

public class Node
{
    //public readonly List<int> Segs = new List<int>();
    public ushort Num;
    public bool Pullup;
    public bool Pulldown;
    public bool State;
    public readonly List<Transistor> Gates = new List<Transistor>();
    public readonly List<Transistor> C1C2s = new List<Transistor>();
    public int Area;
}

public class Transistor
{
    public string Name;
    public bool On;
    public ushort Gate;
    public ushort C1;
    public ushort C2;
}
