using Aemula.UI;
using ImGuiNET;

namespace Aemula.Clock.UI
{
    internal sealed class ClockWindow : DebuggerWindow
    {
        private readonly Clock _clock;

        public override string DisplayName => "Clock";

        public ClockWindow(Clock clock)
        {
            _clock = clock;
        }

        protected override void DrawOverride(EmulatorTime time)
        {
            ImGui.Text($"Clock: {_clock.MasterClock}");

            if (ImGui.Button(_clock.IsRunning ? "Pause" : "Run"))
            {
                _clock.IsRunning = !_clock.IsRunning;
            }

            ImGui.SameLine();

            if (_clock.IsRunning)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
            }
            if (ImGui.Button("Step Cycle") && !_clock.IsRunning)
            {
                _clock.Cycle();
            }
            if (_clock.IsRunning)
            {
                ImGui.PopStyleVar();
            }
        }
    }
}
