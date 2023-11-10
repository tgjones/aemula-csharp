namespace Aemula.Systems.Nes.Tests.Visual2C02;

internal static partial class Configuration
{
    static Configuration()
    {
        SegmentDefinitions = CreateSegmentDefinitions();
        TransistorDefinitions = CreateTransistorDefinitions();
    }
}
