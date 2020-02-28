using Aemula.Memory;
using System.IO;
using Xunit;

namespace Aemula.Chips.Arm.Tests
{
    public class ArmTests
    {
        [Theory(Skip = "Not implemented yet")]
        [InlineData("add")]
        //[InlineData("push")]
        public void CanExecuteTestSuite(string testFile)
        {
            var binFilePath = Path.Combine("TestSuite", "v7", testFile + ".bin");
            var binBytes = File.ReadAllBytes(binFilePath);

            var bus = new Bus.Bus<uint, byte>();
            bus.Map(0, (ushort)(binBytes.Length - 1), new Rom<uint, byte>(binBytes));
            bus.Map(0xCFFF0001, 0xCFFFFFFF, new Ram<uint, byte>(0xFFFF)); // 64K of stack space

            var cpu = new Arm(bus);

            const uint returnAddress = 0xFEFEFEFE;
            cpu.LR = returnAddress;

            while (cpu.R.CurrentInstructionAddress != returnAddress)
            {
                cpu.Step();
            }

            Assert.Equal(0u, cpu.R[Registers.R0]);
        }
    }
}
