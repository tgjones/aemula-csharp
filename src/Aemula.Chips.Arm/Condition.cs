namespace Aemula.Chips.Arm
{
    /// <summary>
    /// A8.3
    /// </summary>
    internal enum Condition : byte
    {
        EQ = 0b0000,
        NE = 0b0001,
        CS = 0b0010,
        CC = 0b0011,
        MI = 0b0100,
        PL = 0b0101,
        VS = 0b0110,
        VC = 0b0111,
        HI = 0b1000,
        LS = 0b1001,
        GE = 0b1010,
        LT = 0b1011,
        GT = 0b1100,
        LE = 0b1101,
        AL = 0b1110
    }
}
