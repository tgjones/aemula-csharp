using System;
using System.Linq;
using System.Numerics;
using ImGuiNET;

namespace Aemula.UI
{
    public sealed class DisassemblyWindow : DebuggerWindow
    {
        private readonly IDisassemblable _system;
        private DisassembledInstruction[] _disassembly;

        private bool _forceScroll;

        public override string DisplayName => "Disassembly";

        public DisassemblyWindow(IDisassemblable system)
        {
            _system = system;

            system.ProgramLoaded += (sender, e) =>
            {
                // TODO: Slow...
                _disassembly = _system.Disassemble().Values.ToArray();
            };
        }

        protected override unsafe void DrawOverride(EmulatorTime time)
        {
            ImGui.ShowDemoWindow();

            if (ImGui.Button("Step Instruction"))
            {

            }

            ImGui.Separator();

            if (ImGui.BeginChild("DisassemblyListing", Vector2.Zero, false))
            {
                ImGui.Columns(2, "DisassemblyColumns");

                var lineHeight = ImGui.GetTextLineHeight();

                var lastPC = _system.LastPC;

                if (_forceScroll)
                {
                    var index = Array.FindIndex(_disassembly, x => x.AddressNumeric == lastPC);
                    ImGui.SetScrollFromPosY(ImGui.GetCursorStartPos().Y + (index * lineHeight));
                    _forceScroll = false;
                }

                int displayStart, displayEnd;
                ImGuiNative.igCalcListClipping(_disassembly.Length, lineHeight, &displayStart, &displayEnd);
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + (displayStart * lineHeight));

                for (var i = displayStart; i < displayEnd; i++)
                {
                    ref readonly var instruction = ref _disassembly[i];

                    var addressLabel = instruction.Address;
                    if (instruction.AddressNumeric == lastPC)
                    {
                        ImGui.Selectable(addressLabel, true, ImGuiSelectableFlags.SpanAllColumns);
                    }
                    else
                    {
                        ImGui.Text(addressLabel);
                    }

                    ImGui.NextColumn();

                    ImGui.Text(instruction.Disassembly);
                    ImGui.NextColumn();
                }

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ((_disassembly.Length - displayEnd) * lineHeight));

                ImGui.Columns(1);
            }
            ImGui.EndChild();
        }
    }
}