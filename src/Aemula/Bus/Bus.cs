using System;
using System.Collections.Generic;
using Aemula.Memory;

namespace Aemula.Bus
{
    public sealed class Bus<TAddress, TData> : IBus<TAddress, TData>
        where TAddress : struct
        where TData : struct
    {
        private static readonly IGenericCalculator<TAddress> Calculator = GenericCalculators.GetInstance<TAddress>();

        private readonly List<BusEntry> _entries;

        public Bus()
        {
            _entries = new List<BusEntry>();
        }

        public void Clear()
        {
            _entries.Clear();
        }

        public void Map(TAddress startAddress, TAddress endAddress, MemoryDevice<TAddress, TData> device, ulong? mask = null)
        {
            _entries.Add(new BusEntry(startAddress, endAddress, mask, device));
        }

        public TData Read(TAddress address)
        {
            return FindEntry(address)?.Read(address) ?? default;
        }

        public void Write(TAddress address, TData data)
        {
            FindEntry(address)?.Write(address, data);
        }

        private BusEntry FindEntry(TAddress address)
        {
            // NOTE: Later entries "override" earlier entries, if the address ranges overlap.
            for (var i = _entries.Count - 1; i >= 0; i--)
            {
                var entry = _entries[i];
                if (Calculator.IsInRange(address, entry.StartAddress, entry.EndAddress))
                {
                    return entry;
                }
            }

            return null;
        }

        private sealed class BusEntry
        {
            public readonly TAddress StartAddress;
            public readonly TAddress EndAddress;
            public readonly ulong? Mask;
            public readonly MemoryDevice<TAddress, TData> Device;

            internal BusEntry(TAddress startAddress, TAddress endAddress, ulong? mask, MemoryDevice<TAddress, TData> device)
            {
                StartAddress = startAddress;
                EndAddress = endAddress;
                Mask = mask;
                Device = device;
            }

            public TData Read(TAddress address)
            {
                return Device.Read(GetDecodedAddress(address));
            }

            public void Write(TAddress address, TData data)
            {
                Device.Write(GetDecodedAddress(address), data);
            }

            private TAddress GetDecodedAddress(TAddress address)
            {
                var relativeAddress = Calculator.Subtract(address, StartAddress);
                var decodedAddress = Mask != null
                    ? Calculator.And(relativeAddress, Mask.Value)
                    : relativeAddress;
                return decodedAddress;
            }
        }
    }
}
