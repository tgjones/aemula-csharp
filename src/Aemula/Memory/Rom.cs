using System;

namespace Aemula.Memory
{
    public sealed class Rom<TAddress, TData> : MemoryDevice<TAddress, TData>
        where TAddress : struct
        where TData : struct
    {
        private readonly TData[] _data;

        public Rom(TData[] data)
            : base((uint)data.Length)
        {
            _data = data;
        }

        public override TData Read(TAddress address)
        {
            return _data[Calculator.GetArrayIndex(address)];
        }

        public override void Write(TAddress address, TData data)
        {
            throw new InvalidOperationException();
        }
    }
}
