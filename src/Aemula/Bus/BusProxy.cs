namespace Aemula.Bus
{
    public sealed class WrappedBus<TAddress, TData> : IBus<TAddress, TData>
        where TAddress : struct
        where TData : struct
    {
        private readonly IBus<TAddress, TData> _original;
        private readonly FilterDelegate _filter;
        private readonly ReadDelegate _read;
        private readonly WriteDelegate _write;

        public WrappedBus(IBus<TAddress, TData> original, FilterDelegate filter, ReadDelegate read, WriteDelegate write)
        {
            _original = original;
            _filter = filter;
            _read = read;
            _write = write;
        }

        public TData Read(TAddress address)
        {
            if (_filter(address))
            {
                return _read(address);
            }
            return _original.Read(address);
        }

        public void Write(TAddress address, TData data)
        {
            if (_filter(address))
            {
                _write(address, data);
                return;
            }
            _original.Write(address, data);
        }

        public delegate bool FilterDelegate(TAddress address);

        public delegate TData ReadDelegate(TAddress address);

        public delegate void WriteDelegate(TAddress address, TData data);
    }
}
