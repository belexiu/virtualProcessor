using PrototypeCore.Interfaces;
using PrototypeCore.Models;
using System.Collections.Generic;

namespace PrototypeCore.Services
{
    public class IntreruptQueue : IIntreruptQueue
    {
        long _cyclesElapsedSinceBeginning = 0;

        long _clockIntreruptCycleCountTarget;
        byte _clockIntreruptData;


        Queue<(IntreruptType intrType, byte intrData)> _intreruptQueue = new Queue<(IntreruptType intrType, byte intrData)>();


        public void EnqueueClockIntrerupt(byte nrOfCycles, byte data)
        {
            _clockIntreruptCycleCountTarget = _cyclesElapsedSinceBeginning + nrOfCycles;
            _clockIntreruptData = data;
        }

        public void EnqueueSoftIntrerupt(byte data)
        {
            _intreruptQueue.Enqueue((IntreruptType.Software, data));
        }

        public (byte intrType, byte intrData) GetIntrerupt()
        {
            if (_intreruptQueue.Count > 0)
            {
                var intrerupt = _intreruptQueue.Dequeue();

                var result = ((byte)intrerupt.intrType, intrerupt.intrData);

                return result;
            }

            return default;
        }

        public bool HasIntrerupt()
        {
            return _intreruptQueue.Count > 0;
        }

        public void ProcessSignal()
        {
            _cyclesElapsedSinceBeginning++;

            if (_cyclesElapsedSinceBeginning == _clockIntreruptCycleCountTarget)
            {
                _intreruptQueue.Enqueue((IntreruptType.Clock, _clockIntreruptData));
                _clockIntreruptCycleCountTarget = 0;
            }
        }
    }
}
