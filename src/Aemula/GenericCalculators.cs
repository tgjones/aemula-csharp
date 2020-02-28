using System;
using System.Collections.Generic;

namespace Aemula
{
    public static class GenericCalculators
    {
        private static readonly Dictionary<Type, object> Calculators = new Dictionary<Type, object>()
        {
            { typeof(ushort), new UInt16Calculator() },
        };

        public static IGenericCalculator<T> GetInstance<T>()
            where T : struct
        {
            return (IGenericCalculator<T>)Calculators[typeof(T)];
        }

        private sealed class UInt16Calculator : IGenericCalculator<ushort>
        {
            public ushort Add(ushort left, ulong right) => (ushort)(left + right);

            public ushort And(ushort left, ulong right) => (ushort)(left & right);

            public ushort Subtract(ushort left, ushort right) => (ushort)(left - right);

            public bool IsInRange(ushort value, ushort start, ushort end) => value >= start && value <= end;

            public ulong GetArrayIndex(ushort value) => value;
        }
    }
}
