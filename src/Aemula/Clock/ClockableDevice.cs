namespace Aemula.Clock
{
    public readonly struct ClockableDevice
    {
        public ClockableDevice(IClockable device, byte clockSpeedDivider)
        {
            Device = device;
            ClockSpeedDivider = clockSpeedDivider;
        }

        public readonly IClockable Device;
        public readonly byte ClockSpeedDivider;
    }
}
