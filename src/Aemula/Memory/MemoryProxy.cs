using System;
using System.Collections.Generic;

namespace Aemula.Memory
{
    public abstract class MemoryProxy<TAddress, TData> : MemoryDevice<TAddress, TData>
        where TAddress : struct
        where TData : struct
    {
        private readonly List<MemoryMappedProxyEntry> _entries;

        protected MemoryProxy(uint size)
            : base(size)
        {
            _entries = new List<MemoryMappedProxyEntry>();
        }

        public void Map(TAddress startAddress, ulong size, ReadDelegate read, WriteDelegate write)
        {
            _entries.Add(new MemoryMappedProxyEntry(startAddress, size, read, write));
        }

        public sealed override TData Read(TAddress address)
        {
            return FindEntry(address).Read(address);
        }

        public sealed override void Write(TAddress address, TData data)
        {
            FindEntry(address).Write(address, data);
        }

        private MemoryMappedProxyEntry FindEntry(TAddress address)
        {
            // NOTE: Later entries "override" earlier entries, if the address ranges overlap.
            // TODO: We don't need this. Should be optimised.
            for (var i = _entries.Count - 1; i >= 0; i--)
            {
                var entry = _entries[i];
                if (Calculator.IsInRange(address, entry.StartAddress, entry.EndAddress))
                {
                    return entry;
                }
            }

            throw new ArgumentOutOfRangeException();
        }

        private sealed class MemoryMappedProxyEntry
        {
            public readonly TAddress StartAddress;
            public readonly TAddress EndAddress;
            public readonly ReadDelegate Read;
            public readonly WriteDelegate Write;

            public MemoryMappedProxyEntry(TAddress startAddress, ulong size, ReadDelegate read, WriteDelegate write)
            {
                StartAddress = startAddress;
                EndAddress = Calculator.Add(startAddress, size - 1);
                Read = read;
                Write = write;
            }
        }

        public delegate TData ReadDelegate(TAddress address);

        public delegate void WriteDelegate(TAddress address, TData data);
    }
}
