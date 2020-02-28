namespace Aemula.Bus
{
    public interface IBus<TAddress, TData>
        where TAddress : struct
        where TData : struct
    {
        TData Read(TAddress address);

        void Write(TAddress address, TData data);
    }
}
