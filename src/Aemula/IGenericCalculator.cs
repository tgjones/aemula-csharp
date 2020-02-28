namespace Aemula
{
    public interface IGenericCalculator<T>
        where T : struct
    {
        T Add(T left, ulong right);
        T And(T left, ulong right);
        T Subtract(T left, T right);
        bool IsInRange(T value, T start, T end);
        ulong GetArrayIndex(T value);
    }
}
