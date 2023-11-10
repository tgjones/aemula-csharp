using System;
using System.Collections.Immutable;
using System.IO;

namespace Aemula.Systems.Nes.Tests.Visual2C02;

partial class Configuration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Gate">Gate node</param>
    /// <param name="C1">Source node</param>
    /// <param name="C2">Drain node</param>
    public sealed record TransistorDefinition(
        ushort Gate,
        ushort C1,
        ushort C2);

    public static TransistorDefinition[] TransistorDefinitions;

    private static TransistorDefinition[] CreateTransistorDefinitions()
    {
        var lines = File.ReadAllLines("Visual2C02/Configuration.TransistorDefinitions.txt");

        var builder = ImmutableArray.CreateBuilder<TransistorDefinition>(lines.Length);

        foreach (var line in lines)
        {
            if (line == "" || line.StartsWith("//", StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            var splitLine = line.Split(',');

            var transistorDefinition = new TransistorDefinition(
                ushort.Parse(splitLine[1]),
                ushort.Parse(splitLine[2]),
                ushort.Parse(splitLine[3]));

            builder.Add(transistorDefinition);
        }

        return builder.ToArray();
    }
}
