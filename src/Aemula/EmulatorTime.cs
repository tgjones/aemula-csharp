using System;

namespace Aemula
{
    public readonly struct EmulatorTime
    {
        public readonly TimeSpan TotalTime;
        public readonly TimeSpan ElapsedTime;

        public EmulatorTime(TimeSpan totalTime, TimeSpan elapsedTime)
        {
            TotalTime = totalTime;
            ElapsedTime = elapsedTime;
        }
    }
}
