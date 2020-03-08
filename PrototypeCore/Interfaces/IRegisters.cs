using PrototypeCore.Models;

namespace PrototypeCore.Interfaces
{
    public interface IRegisters
    {
        int Read(byte registerType);

        int Read(RegisterType registerType);

        void Write(byte registerType, int value);

        void Write(RegisterType registerType, int value);

        void UpdateIpAndPip(int newLocation);
    }
}
