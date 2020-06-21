using System;

namespace Aemula
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class HandleAttribute : Attribute
    {
        public ChangeType[] ChangeTypes { get; }

        public HandleAttribute(params ChangeType[] changeTypes)
        {
            ChangeTypes = changeTypes;
        }
    }

    public enum ChangeType
    {
        Always,
        Changed,
        TransitionedLoToHi,
        TransitionedHiToLo,
    }
}
