using System;
using System.Collections.Generic;

namespace Aemula.Chips.Mos6502
{
    partial class Mos6502
    {
        public static SortedDictionary<ushort, DisassembledInstruction> Disassemble(Func<ushort, byte> readMemory)
        {
            var result = new SortedDictionary<ushort, DisassembledInstruction>();

            var queue = new Queue<ushort>();

            var startAddress = (ushort)(readMemory(0xFFFC) | (readMemory(0xFFFD) << 8));
            queue.Enqueue(startAddress);

            while (queue.Count > 0)
            {
                var address = queue.Dequeue();

                if (result.ContainsKey(address))
                {
                    continue;
                }

                var disassembledInstruction = DisassembleInstruction(address, readMemory);
                result[address] = disassembledInstruction;

                if (disassembledInstruction.Next0 != null)
                {
                    queue.Enqueue(disassembledInstruction.Next0.Value);
                }
                if (disassembledInstruction.Next1 != null)
                {
                    queue.Enqueue(disassembledInstruction.Next1.Value);
                }
            }

            return result;
        }
    }
}
