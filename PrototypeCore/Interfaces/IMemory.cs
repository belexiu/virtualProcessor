namespace PrototypeCore.Interfaces
{
    public interface IMemory
    {
        int Capacity { get; }

        byte Read(int location);

        (byte, byte) ReadTwo(int location);

        (byte, byte, byte) ReadThree(int location);

        void Write(int location, byte data);
    }
}
