//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Numerics;
//using Aemula.UI;
//using ImGuiNET;

//namespace Aemula.Chips.Mos6502.UI
//{
//    internal sealed class DisassemblyWindow : DebuggerWindow
//    {
//        private readonly Mos6502 _mos6502;
//        private readonly DisassembledInstructionCollection _disassembledInstructions;

//        private bool _forceScroll;

//        public override string DisplayName => "MOS6502 Disassembly";

//        public DisassemblyWindow(Mos6502 mos6502)
//        {
//            _mos6502 = mos6502;

//            _disassembledInstructions = new DisassembledInstructionCollection(address => _mos6502.Disassemble(address));

//            _mos6502.OnFetchingInstruction = () =>
//            {
//                _disassembledInstructions.MarkDirty(_mos6502.PC);
//                _forceScroll = true;
//            };
//        }

//        protected override unsafe void DrawOverride(EmulatorTime time)
//        {
//            ImGui.ShowDemoWindow();

//            if (ImGui.Button("Step Instruction"))
//            {

//            }

//            ImGui.Separator();

//            if (ImGui.BeginChild("DisassemblyListing", Vector2.Zero, false))
//            {
//                ImGui.Columns(2, "DisassemblyColumns");

//                _disassembledInstructions.UpdateDirty();

//                var lineHeight = ImGui.GetTextLineHeight();

//                if (_forceScroll)
//                {
//                    ImGui.SetScrollFromPosY(ImGui.GetCursorStartPos().Y + (_disassembledInstructions.GetIndexOfAddress(_mos6502.PC) * lineHeight));
//                    _forceScroll = false;
//                }

//                int displayStart, displayEnd;
//                ImGuiNative.igCalcListClipping(_disassembledInstructions.Count, lineHeight, &displayStart, &displayEnd);
//                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + (displayStart * lineHeight));

//                for (var i = displayStart; i < displayEnd; i++)
//                {
//                    var instruction = _disassembledInstructions[i];

//                    var addressLabel = instruction.Address.ToString("X4");
//                    if (instruction.Address == _mos6502.PC)
//                    {
//                        ImGui.Selectable(addressLabel, true, ImGuiSelectableFlags.SpanAllColumns);
//                    }
//                    else
//                    {
//                        ImGui.Text(addressLabel);
//                    }

//                    ImGui.NextColumn();

//                    ImGui.Text(instruction.Disassembly);
//                    ImGui.NextColumn();
//                }

//                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ((_disassembledInstructions.Count - displayEnd) * lineHeight));

//                ImGui.Columns(1);
//            }
//            ImGui.EndChild();
//        }

//        private delegate DisassembledInstruction DisassembleDelegate(ushort address);

//        private sealed class DisassembledInstructionCollection
//        {
//            private readonly DisassembleDelegate _disassembleDelegate;

//            private readonly DisassembledInstruction[] _storage;
//            private readonly List<int> _list;
//            private readonly HashSet<ushort> _dirty;

//            public int Count => _list.Count;

//            public DisassembledInstruction this[int index] => _storage[_list[index]];

//            public DisassembledInstructionCollection(DisassembleDelegate disassembleDelegate)
//            {
//                _disassembleDelegate = disassembleDelegate;

//                const int numAddresses = 0x10000;

//                _storage = new DisassembledInstruction[numAddresses];
//                for (var i = 0u; i < numAddresses; i++)
//                {
//                    _storage[i] = new DisassembledInstruction(i, 0, "", "-");
//                }

//                // Initially, all memory entries are blank. As we execute instructions, we start to fill
//                // in the blanks. We remove entries from this list for memory addresses that are instruction operands.
//                _list = new List<int>();
//                for (var i = 0; i < numAddresses; i++)
//                {
//                    _list.Add(i);
//                }

//                _dirty = new HashSet<ushort>();
//            }

//            public int GetIndexOfAddress(ushort address)
//            {
//                return _list.BinarySearch(address);
//            }

//            public void MarkDirty(ushort address)
//            {
//                _dirty.Add(address);
//            }

//            public void UpdateDirty()
//            {
//                foreach (var dirtyAddress in _dirty)
//                {
//                    var disassembledInstruction = _disassembleDelegate(dirtyAddress);

//                    _storage[dirtyAddress] = disassembledInstruction;

//                    var index = _list.BinarySearch(dirtyAddress);

//                    _list[index] = dirtyAddress;

//                    // TODO: Handle negative indices.

//                    var numToRemove = 0;
//                    for (var i = 1; i < disassembledInstruction.InstructionSizeInBytes; i++)
//                    {
//                        if (_list[index + i] < dirtyAddress + disassembledInstruction.InstructionSizeInBytes)
//                        {
//                            numToRemove++;
//                        }
//                    }

//                    _list.RemoveRange(index + 1, numToRemove);
//                }
//                _dirty.Clear();
//            }
//        }
//    }
//}