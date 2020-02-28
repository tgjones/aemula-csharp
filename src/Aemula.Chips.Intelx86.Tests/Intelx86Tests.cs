using System;
using System.IO;
using Aemula.Bus;
using Aemula.Memory;
using Xunit;

namespace Aemula.Chips.Intelx86.Tests
{
    public class Intelx86Tests
    {
        [Fact]
        public void RunTest386()
        {
            var testRomPath = Path.Combine("TestSuite", "test386.bin");
            var testRomBytes = File.ReadAllBytes(testRomPath);
            var testRom = new Rom<uint, byte>(testRomBytes);

            var bus = new Bus<uint, byte>();
            bus.Map(0x00000000, 0x000FFFFF, new Ram<uint, byte>(1024 * 1024)); // 1 MB
            bus.Map(0x000F0000, 0x000FFFFF, testRom);
            bus.Map(0xFFFF0000, 0xFFFFFFFF, testRom);

            var cpu = new Intelx86(bus);

            cpu.Reset();

            cpu.Run();
        }

        [Fact]
        public void RunCodeGolf()
        {
            // https://codegolf.stackexchange.com/questions/4732/emulate-an-intel-8086-cpu

            var testRomPath = Path.Combine("TestSuite", "codegolf", "codegolf.bin");
            var testRomBytes = File.ReadAllBytes(testRomPath);

            var ramBytes = new byte[0xFFFF];
            testRomBytes.CopyTo(ramBytes, 0);

            var ram = new Ram<uint, byte>(ramBytes);

            var bus = new Bus<uint, byte>();
            bus.Map(0x0000, 0xFFFF, ram);

            var cpu = new Intelx86(bus);

            //cpu.Reset();
            cpu.IP = 0;
            cpu.SP = 0x100;

            cpu.Run();

            // Print output to console.
            var videoRamStart = 0x8000u;
            for (var y = 0u; y < 25; y++)
            {
                for (var x = 0u; x < 80; x++)
                {
                    Console.SetCursorPosition((int)x, (int)y);

                    var data = ram.Read(videoRamStart + (y * 80) + x);
                    Console.Write((char)data);
                }
            }
        }
    }
}
