//using System.Collections.Generic;
//using Aemula.Bus;
//using Aemula.Chips.Ricoh2A03;
//using Aemula.Chips.Ricoh2C02;
//using Aemula.Memory;
//using Aemula.UI;

//namespace Aemula.Consoles.Nes
//{
//    public sealed class Nes : EmulatedSystem
//    {
//        public readonly Clock.Clock Clock;

//        public readonly Bus<ushort, byte> CpuBus;
//        public readonly Ricoh2A03 Cpu;

//        public readonly Bus<ushort, byte> PpuBus;
//        public readonly Ricoh2C02 Ppu;

//        public readonly Ram<ushort, byte> Ram;
//        public readonly Ram<ushort, byte> VRam;

//        public Nes()
//        {
//            Clock = new Clock.Clock(21477272);

//            CpuBus = new Bus<ushort, byte>();
//            Cpu = new Ricoh2A03(CpuBus);

//            PpuBus = new Bus<ushort, byte>();
//            Ppu = new Ricoh2C02(PpuBus);

//            Ram = new Ram<ushort, byte>(0x0800);

//            VRam = new Ram<ushort, byte>(0x0800);

//            Clock = new Clock.Clock(
//                21477272,
//                new Clock.ClockableDevice(Cpu, 12),
//                new Clock.ClockableDevice(Ppu, 4));
//        }

//        public void InsertCartridge(Cartridge cartridge)
//        {
//            CpuBus.Clear();
//            PpuBus.Clear();

//            void ConfigureCpuBus()
//            {
//                // 2KB RAM, mirrored 3 times.
//                CpuBus.Map(0x0000, 0x1FFF, Ram, 0x7FF);

//                // PPU, mirrored 1023 times.
//                CpuBus.Map(0x2000, 0x3FFF, Ppu, 0x7);

//                // $4000-$401F is mapped internally on 2A03 chip.

//                // TODO: Mapper implementations.
//                // What follows is NROM-128, mapper 0.

//                CpuBus.Map(0x8000, 0xFFFF, new Rom<ushort, byte>(cartridge.PrgRom), 0x3FFF);
//            }

//            void ConfigurePpuBus()
//            {
//                PpuBus.Map(0x0000, 0x1FFF, new Rom<ushort, byte>(cartridge.ChrRom));

//                // VRAM.
//                PpuBus.Map(0x2000, 0x3EFF, VRam, 0x7FF);

//                // $3F00-$3FFF is mapped internally in the PPU.
//            }

//            ConfigureCpuBus();
//            ConfigurePpuBus();
//        }

//        public override void Reset()
//        {
//            Cpu.Reset();
//        }

//        public override IEnumerable<DebuggerWindow> CreateDebuggerWindows()
//        {
//            foreach (var debuggerWindow in Clock.CreateDebuggerWindows())
//            {
//                yield return debuggerWindow;
//            }

//            foreach (var debuggerWindow in Cpu.CreateDebuggerWindows())
//            {
//                yield return debuggerWindow;
//            }

//            foreach (var debuggerWindow in Ppu.CreateDebuggerWindows())
//            {
//                yield return debuggerWindow;
//            }
//        }
//    }
//}
