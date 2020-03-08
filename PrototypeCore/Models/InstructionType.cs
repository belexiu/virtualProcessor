namespace PrototypeCore.Models
{
    public enum InstructionType : byte
    {
        Jump = 1,
        JumpZero,
        JumpPositive,
        JumpNegative,
        MovLiteral,
        MovRegisters,
        MovFromMemory,
        MovToMemory,
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        IntreruptClock,
        IntreruptSoft
    }
}
