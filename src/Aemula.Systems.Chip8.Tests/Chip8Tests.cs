using NUnit.Framework;

namespace Aemula.Systems.Chip8.Tests
{
    public class Chip8Tests
    {
        [Test]
        public void TestBCTest()
        {
            var system = new Chip8();
            system.LoadProgram("Assets/bc_test.ch8");

            var maxCycles = 1000000;
            var cycles = 0;
            while (cycles < maxCycles)
            {
                var lastPC = system.PC;

                system.Tick();

                if (lastPC == 0x030E && system.PC == 0x030E)
                {
                    Assert.Pass();
                    return;
                }

                cycles++;
            }

            Assert.Fail();
        }

        [Test]
        public void TestChip8TestRom()
        {
            var system = new Chip8();
            system.LoadProgram("Assets/test_opcode.ch8");

            var maxCycles = 1000000;
            var cycles = 0;
            while (cycles < maxCycles)
            {
                var lastPC = system.PC;

                system.Tick();

                if (lastPC == 0x03DC && system.PC == 0x03DC)
                {
                    Assert.Pass();
                    return;
                }

                cycles++;
            }

            Assert.Fail();
        }
    }
}
