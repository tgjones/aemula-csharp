using Aemula.Memory;
using System.IO;
using Xunit;

namespace Aemula.Chips.Mos6502.Tests
{
    public class Mos6502Tests
    {
        [Fact]
        public void AllSuiteA()
        {
            var testBytes = File.ReadAllBytes(Path.Combine("Assets", "AllSuiteA.bin"));

            var bus = new Bus.Bus<ushort, byte>();
            bus.Map(0x0000, 0x3FFF, new Ram<ushort, byte>(0x4000));
            bus.Map(0x4000, (ushort)(0x4000 + testBytes.Length - 1), new Rom<ushort, byte>(testBytes));

            var cpu = new Mos6502(bus);
            cpu.Reset();

            while (cpu.PC != 0x45c2)
            {
                cpu.Cycle();
            }

            Assert.Equal(0xFF, bus.Read(0x0210));
        }

        [Fact]
        public void DormannFunctionalTest()
        {
            var testBytes = File.ReadAllBytes(Path.Combine("Assets", "6502_functional_test.bin"));

            var bus = new Bus.Bus<ushort, byte>();
            bus.Map(0x0000, (ushort)(testBytes.Length - 1), new Ram<ushort, byte>(testBytes));

            var cpu = new Mos6502(bus);
            cpu.Reset();

            // Run CPU until it's finished RESETting.
            while (cpu.Resetting)
            {
                cpu.Cycle();
            }

            cpu.PC = 0x0400;

            while (cpu.PC != 0x3399 && cpu.PC != 0xD0FE)
            {
                cpu.Cycle();
            }

            Assert.Equal(0x3399, cpu.PC);
        }

        [Fact]
        public void NesTest()
        {
            byte[] testBytes;
            using (var reader = new BinaryReader(File.OpenRead(Path.Combine("Assets", "nestest.nes"))))
            {
                reader.BaseStream.Seek(16, SeekOrigin.Current);
                testBytes = reader.ReadBytes(16384);
            }

            var bus = new Bus.Bus<ushort, byte>();
            bus.Map(0x0000, 0x1FFF, new Ram<ushort, byte>(0x0800), 0x07FF);
            bus.Map(0x4000, 0x4017, new Ram<ushort, byte>(0x018)); // APU and I/O registers
            bus.Map(0x8000, 0xFFFF, new Rom<ushort, byte>(testBytes), 0x3FFF);

            var cpu = new Mos6502(bus);
            cpu.Reset();

            // Run CPU until it's finished RESETting.
            while (cpu.Resetting)
            {
                cpu.Cycle();
            }

            // TODO: Do this somewhere else.
            cpu.SupportsDecimalMode = false;

            using (var streamWriter = new StreamWriter("nestest_aemula.log"))
            {
                cpu.OnFetchingInstruction = () => streamWriter.WriteLine($"{cpu.PC.ToString("X4")}  A:{cpu.A.ToString("X2")} X:{cpu.X.ToString("X2")} Y:{cpu.Y.ToString("X2")} P:{cpu.P.AsByte(false).ToString("X2")} SP:{cpu.SP.ToString("X2")} CPUC:{cpu.Cycles - 7}");
                cpu.OnMemoryRead = (address, data) => streamWriter.WriteLine($"      READ      ${address.ToString("X4")} => ${data.ToString("X2")}");
                cpu.OnMemoryWrite = (address, data) => streamWriter.WriteLine($"      WRITE     ${address.ToString("X4")} <= ${data.ToString("X2")}");

                cpu.PC = 0xC000;

                while (cpu.PC != 0xC66E)
                {
                    cpu.Cycle();
                }

                streamWriter.Flush();
                streamWriter.Dispose();
            }

            Assert.Equal(0x000, bus.Read(0x02));
            Assert.Equal(0x000, bus.Read(0x03));

            Assert.Equal(
                File.ReadAllText(Path.Combine("Assets", "nestest.log")), 
                File.ReadAllText("nestest_aemula.log"));
        }
    }
}
