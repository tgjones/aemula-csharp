using System;

namespace Aemula
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class PinAttribute : Attribute
    {
        public PinDirection Direction { get; }

        public PinAttribute(PinDirection direction)
        {
            Direction = direction;
        }
    }

    public enum PinDirection
    {
        In,
        Out,
        Bidirectional,
    }
}
