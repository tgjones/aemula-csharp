using System;
using System.IO;
using NUnit.Framework;

namespace Aemula.Chips.Intel8080.Tests
{
    public class Intel8080Tests
    {
        [Test]
        public void CpuDiag()
        {
            var programBytes = File.ReadAllBytes("Assets/cpudiag.bin");

            var ram = new byte[0x10000];

            Array.Copy(programBytes, 0, ram, 0x100, programBytes.Length);

            var cpu = new Intel8080(ram);
            cpu.PC = 0x100;

            while (true)
            {
                cpu.Cycle();
            }
        }
    }
}
