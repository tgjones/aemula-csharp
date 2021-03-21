using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Aemula.Chips.Intel8080.Tests
{
    public class Intel8080Tests
    {
        [Test]
        public void Tst8080()
        {
            var programBytes = File.ReadAllBytes("Assets/TST8080.COM");

            var ram = new byte[0x10000];

            Array.Copy(programBytes, 0, ram, 0x100, programBytes.Length);

            var cpu = new Intel8080();
            cpu.PC.Value = 0x100;

            // Patch "CALL BDOS" with RET instruction.
            ram[0x0005] = 0xC9;

            var cycles = 0;

            var output = new StringBuilder();

            byte lastStatusWord = 0;

            //Func<ushort, byte> readMemoryCallback = x => ram[x];

            while (true)
            {
                cpu.Cycle();

                if (cpu.Pins.Sync)
                {
                    if (cpu.Pins.Data == Intel8080.StatusWordFetch)
                    {
                        //Intel8080.Disassemble(
                        //    cpu.Pins.Address,
                        //    readMemoryCallback,
                        //    OutputStringCallback);

                        switch (cpu.Pins.Address)
                        {
                            case 0x0000:
                                var outputText = output.ToString();
                                if (outputText.Contains("FAILED"))
                                {
                                    Assert.Fail(outputText);
                                }
                                else
                                {
                                    Assert.Pass(outputText);
                                }
                                break;

                            case 0x0005:
                                switch (cpu.BC.C)
                                {
                                    case 0x02: // Output single character stored in E
                                        output.Append((char)cpu.DE.E);
                                        break;

                                    case 0x09: // Print from characters stored at DE until we hit '$'
                                        var characterAddress = cpu.DE.Value;
                                        do
                                        {
                                            output.Append((char)ram[characterAddress++]);
                                        } while (ram[characterAddress] != '$');
                                        break;

                                    default:
                                        throw new InvalidOperationException();
                                }
                                break;
                        }
                    }

                    lastStatusWord = cpu.Pins.Data;
                }

                if (cpu.Pins.DBIn)
                {
                    switch (lastStatusWord)
                    {
                        case Intel8080.StatusWordFetch:
                        case Intel8080.StatusWordMemoryRead:
                        case Intel8080.StatusWordStackRead:
                            cpu.Pins.Data = ram[cpu.Pins.Address];
                            break;
                    }
                }

                if (!cpu.Pins.Wr)
                {
                    switch (lastStatusWord)
                    {
                        case Intel8080.StatusWordMemoryWrite:
                        case Intel8080.StatusWordStackWrite:
                            ram[cpu.Pins.Address] = cpu.Pins.Data;
                            break;
                    }
                }

                cycles++;
            }
        }

        private static readonly OutputStringDelegate OutputStringCallback = OutputString;

        private static void OutputString(ReadOnlySpan<byte> bytes)
        {
            var stringValue = Encoding.UTF8.GetString(bytes);
            Console.WriteLine(stringValue);
        }
    }
}
