using System.Collections.Generic;
using Aemula.Debugging;

namespace Aemula.Chips.Mos6502.Debugging;

public class Mos6502Disassembler : Disassembler
{
    private static readonly Dictionary<ushort, string> StartAddresses = new()
    {
        { 0xFFFA, "NMI" },
        { 0xFFFC, "RESET" },
        { 0xFFFE, "IRQ / BRK" }
    };

    private readonly Dictionary<ushort, string> _equates;

    public Mos6502Disassembler(DebuggerMemoryCallbacks memoryCallbacks, Dictionary<ushort, string> equates)
        : base(memoryCallbacks)
    {
        _equates = equates;
    }

    protected override void OnReset(List<ushort> startAddresses, Dictionary<ushort, string> labels)
    {
        foreach (var startAddress in StartAddresses)
        {
            var targetAddress = MemoryCallbacks.ReadWord(startAddress.Key);

            // We assume that a vector set to 0x0000 or 0xFFFF is invalid.
            // It might actually be valid - but that's okay,
            // we'll disassemble it when it's actually executed.
            if (targetAddress == 0x0000 || targetAddress == 0xFFFF)
            {
                continue;
            }

            startAddresses.Add(targetAddress);

            if (labels.TryGetValue(targetAddress, out var label))
            {
                label += $", {startAddress.Value}";
            }
            else
            {
                label = startAddress.Value;
            }
            labels[targetAddress] = label;
        }
    }

    protected override DisassembledInstruction DisassembleInstruction(ushort address)
    {
        return Mos6502.DisassembleInstruction(
            address,
            MemoryCallbacks.Read,
            _equates);
    }
}
