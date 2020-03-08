namespace PrototypeCore.Interfaces
{
    public interface IIntreruptQueue
    {
        void ProcessSignal();

        void EnqueueSoftIntrerupt(byte data);

        void EnqueueClockIntrerupt(byte nrOfCycles, byte data);

        (byte intrType, byte intrData) GetIntrerupt();

        bool HasIntrerupt();
    }
}
