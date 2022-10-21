using System;
using System.Numerics;
using ImGuiNET;
using Veldrid;

namespace Aemula.UI
{
    public sealed class ScreenDisplayWindow : DebuggerWindow
    {
        private readonly DisplayBuffer _displayBuffer;

        private GraphicsDevice _graphicsDevice;
        private Texture _texture;
        private IntPtr _textureBinding;

        public override string DisplayName => "Display";

        public ScreenDisplayWindow(DisplayBuffer displayBuffer)
        {
            _displayBuffer = displayBuffer;
        }

        public override void CreateGraphicsResources(GraphicsDevice graphicsDevice, ImGuiRenderer renderer)
        {
            base.CreateGraphicsResources(graphicsDevice, renderer);

            _graphicsDevice = graphicsDevice;

            _texture = graphicsDevice.ResourceFactory.CreateTexture(
                TextureDescription.Texture2D(
                    _displayBuffer.Width,
                    _displayBuffer.Height,
                    1,
                    1,
                    PixelFormat.R8_G8_B8_A8_UNorm,
                    TextureUsage.Sampled));

            _textureBinding = renderer.GetOrCreateImGuiBinding(
                graphicsDevice.ResourceFactory, 
                _texture);
        }

        protected override void DrawOverride(EmulatorTime time)
        {
            _graphicsDevice.UpdateTexture(
                _texture,
                _displayBuffer.Data,
                0, 0, 0,
                _displayBuffer.Width,
                _displayBuffer.Height,
                1, 0, 0);

            ImGui.Image(
                _textureBinding,
                new Vector2(_texture.Width, _texture.Height));
        }

        public override void Dispose()
        {
            base.Dispose();

            _texture.Dispose();
        }
    }
}
