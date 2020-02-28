namespace Aemula.Memory
{
    public abstract class MemoryDevice<TAddress, TData>
        where TAddress : struct
        where TData : struct
    {
        protected static readonly IGenericCalculator<TAddress> Calculator = GenericCalculators.GetInstance<TAddress>();

        public readonly ulong Size;

        protected MemoryDevice(ulong size)
        {
            Size = size;
        }

        public abstract TData Read(TAddress address);

        public abstract void Write(TAddress address, TData data);
    }
}
