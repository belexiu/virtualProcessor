using PrototypeCore.Interfaces;
using PrototypeCore.Interfaces.Diagnostics;
using PrototypeCore.Models.Diagnostics;

namespace PrototypeCore.Services
{
    public class Memory : IMemory
    {
        byte[] _dataArray = new byte[256];
        
        private readonly IDiagnosticsService _diagnosticsService;

        public Memory(IDiagnosticsService diagnosticsService)
        {
            _diagnosticsService = diagnosticsService;
        }

        public int Capacity => _dataArray.Length;

        public byte Read(int location)
        {
            return _dataArray[location];
        }

        public (byte, byte) ReadTwo(int location)
        {
            return (_dataArray[location], _dataArray[location + 1]);
        }
        
        public (byte, byte, byte) ReadThree(int location)
        {
            return (_dataArray[location], _dataArray[location + 1], _dataArray[location + 2]);
        }


        public void Write(int location, byte data)
        {
            var writeEvent = new MemoryWriteEvent
            {
                OldValue = _dataArray[location],
                NewValue = data,
                Location = location
            };

            _dataArray[location] = data;

            _diagnosticsService.AddEvent(writeEvent);
        }
    }
}
