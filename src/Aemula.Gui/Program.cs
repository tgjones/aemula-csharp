using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Aemula.Consoles.Nes;
using Aemula.Systems.Atari2600;
using Aemula.Systems.Chip8;
using Aemula.UI;
using ImGuiNET;
using Veldrid;
using Veldrid.StartupUtilities;

namespace Aemula.Gui
{
    public static class Program
    {
        private static readonly Dictionary<string, Func<EmulatedSystem>> Systems = new Dictionary<string, Func<EmulatedSystem>>
        {
            { "atari2600", () => new Atari2600() },
            { "chip8", () => new Chip8() },
            { "nes", () => new Nes() }
        };

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

            var systemArg = args[0];
            var system = Systems[systemArg]();

            system.LoadProgram(args[1]);

            var emulator = new Emulator(system);

            var debuggerWindows = emulator.CreateDebuggerWindows().ToArray();
            foreach (var debuggerWindow in debuggerWindows)
            {
                debuggerWindow.CreateGraphicsResources(graphicsDevice, imGuiRenderer);
            }

            while (windowOpen)
            {
                var deltaTimeSpan = stopwatch.Elapsed - lastTime;
                lastTime = stopwatch.Elapsed;

                var deltaTime = (float)deltaTimeSpan.TotalSeconds;
                imGuiRenderer.Update(deltaTime, window.PumpEvents());

                var emulatorTime = new EmulatorTime(stopwatch.Elapsed, deltaTimeSpan);

                emulator.Update(emulatorTime);

                DrawMainMenu(system, debuggerWindows);

                emulator.Draw(emulatorTime);

                foreach (var debuggerWindow in debuggerWindows)
                {
                    debuggerWindow.Draw(emulatorTime);
                }

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

        private static void DrawMainMenu(EmulatedSystem system, DebuggerWindow[] debuggerWindows)
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Windows"))
                {
                    foreach (var debuggerWindow in debuggerWindows)
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
        }
    }
}
