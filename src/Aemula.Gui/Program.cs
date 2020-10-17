using System;
using System.Diagnostics;
using System.Linq;
using Aemula.Consoles.Nes;
using Aemula.UI;
using ImGuiNET;
using Veldrid;
using Veldrid.StartupUtilities;

namespace Aemula.Gui
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(100, 100, 1024, 768, WindowState.Normal, "Aemula"),
                out var window,
                out var graphicsDevice);

            graphicsDevice.SyncToVerticalBlank = true;

            var imGuiRenderer = new ImGuiRenderer(
                graphicsDevice,
                graphicsDevice.SwapchainFramebuffer.OutputDescription,
                (int)graphicsDevice.SwapchainFramebuffer.Width,
                (int)graphicsDevice.SwapchainFramebuffer.Height);

            var commandList = graphicsDevice.ResourceFactory.CreateCommandList();

            var windowOpen = true;

            window.Closed += () => windowOpen = false;
            window.Resized += () =>
            {
                imGuiRenderer.WindowResized(window.Width, window.Height);
                graphicsDevice.ResizeMainWindow((uint)window.Width, (uint)window.Height);
            };

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var lastTime = stopwatch.Elapsed;

            var nesEmulator = new NesEmulator();

            nesEmulator.Initialize(graphicsDevice, imGuiRenderer);

            while (windowOpen)
            {
                var deltaTimeSpan = stopwatch.Elapsed - lastTime;
                lastTime = stopwatch.Elapsed;

                var deltaTime = (float)deltaTimeSpan.TotalSeconds;
                imGuiRenderer.Update(deltaTime, window.PumpEvents());

                var emulatorTime = new EmulatorTime(stopwatch.Elapsed, deltaTimeSpan);

                nesEmulator.Update(emulatorTime);

                nesEmulator.Draw(emulatorTime);

                commandList.Begin();
                commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
                commandList.ClearColorTarget(0, RgbaFloat.Clear);

                imGuiRenderer.Render(graphicsDevice, commandList);

                commandList.End();

                graphicsDevice.SubmitCommands(commandList);
                graphicsDevice.SwapBuffers();
            }

            stopwatch.Stop();

            commandList.Dispose();
            imGuiRenderer.Dispose();
            graphicsDevice.Dispose();
        }

        private sealed class NesEmulator
        {
            private readonly Nes _nes;
            private readonly DebuggerWindow[] _debuggerWindows;

            public NesEmulator()
            {
                _nes = new Nes();
                _nes.Reset();

                _debuggerWindows = _nes.CreateDebuggerWindows().ToArray();
            }

            public void Initialize(GraphicsDevice graphicsDevice, ImGuiRenderer renderer)
            {
                foreach (var debuggerWindow in _debuggerWindows)
                {
                    debuggerWindow.CreateGraphicsResources(graphicsDevice, renderer);
                }
            }

            public void Update(EmulatorTime time)
            {
                _nes.Update(time.TotalTime);
            }

            public void Draw(EmulatorTime time)
            {
                if (ImGui.BeginMainMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        if (ImGui.MenuItem("Open .nes..."))
                        {
                            // TODO
                            var cartridge = Cartridge.FromFile(@"C:\CodePersonal\Aemula\src\Aemula.Chips.Mos6502.Tests\Assets\nestest.nes");
                            _nes.InsertCartridge(cartridge);
                            _nes.Reset();
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Windows"))
                    {
                        foreach (var debuggerWindow in _debuggerWindows)
                        {
                            if (ImGui.MenuItem(debuggerWindow.DisplayName, null, debuggerWindow.IsVisible, true))
                            {
                                debuggerWindow.IsVisible = true;

                                ImGui.SetWindowFocus(debuggerWindow.Name);
                            }
                        }

                        ImGui.EndMenu();
                    }

                    ImGui.EndMainMenuBar();
                }

                foreach (var debuggerWindow in _debuggerWindows)
                {
                    debuggerWindow.Draw(time);
                }
            }
        }
    }
}
