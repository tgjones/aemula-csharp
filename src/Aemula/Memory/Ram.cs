namespace Aemula.Memory
{
    public sealed class Ram<TAddress, TData> : MemoryDevice<TAddress, TData>
        where TAddress : struct
        where TData : struct
    {
        private readonly TData[] _memory;

        public Ram(uint size)
            : base(size)
        {
            _memory = new TData[size];
        }

        /// <summary>
        /// Useful for testing.
        /// </summary>
        public Ram(TData[] memory)
            : base((uint)memory.Length)
        {
            _memory = memory;
        }

        public override TData Read(TAddress address)
        {
            return _memory[Calculator.GetArrayIndex(address)];
        }

        public override void Write(TAddress address, TData data)
        {
            _memory[Calculator.GetArrayIndex(address)] = data;
        }
    }
}
