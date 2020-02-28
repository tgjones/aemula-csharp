using System;

namespace Aemula.Chips.Arm
{
    public sealed class Registers
    {
        public const byte R0 = 0;
        public const byte R1 = 1;
        public const byte R2 = 2;
        public const byte R3 = 3;
        public const byte R4 = 4;
        public const byte R5 = 5;
        public const byte R6 = 6;
        public const byte R7 = 7;
        public const byte R8 = 8;
        public const byte R9 = 9;
        public const byte R10 = 10;
        public const byte R11 = 11;
        public const byte R12 = 12;
        public const byte SP = 13;
        public const byte LR = 14;
        public const byte PC = 15;

        private readonly uint[] _storage;

        public uint this[uint index]
        {
            get
            {
                var result = _storage[index];
                if (index == PC)
                {
                    result += 8;
                }
                return result;
            }
            set
            {
                if (index >= 15)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                _storage[index] = value;
            }
        }

        public uint CurrentInstructionAddress
        {
            get => _storage[PC];
            internal set => _storage[PC] = value;
        }

        public Registers()
        {
            _storage = new uint[16];
        }
    }
}
