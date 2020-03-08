using PrototypeCore.Interfaces;
using PrototypeCore.Interfaces.Diagnostics;
using PrototypeCore.Models;
using PrototypeCore.Models.Diagnostics;
using System;
using System.Collections.Generic;

namespace PrototypeCore.Services
{
    public class Registers : IRegisters
    {
        Dictionary<RegisterType, int> _registerDict = new Dictionary<RegisterType, int>
        {
            [RegisterType.R1] = 0,
            [RegisterType.R2] = 0,
            [RegisterType.R3] = 0,
            [RegisterType.R4] = 0,
            [RegisterType.It] = 0,
            [RegisterType.Id] = 0,
            [RegisterType.Ip] = 0,
            [RegisterType.Pip] = 0
        };


        private readonly IDiagnosticsService _diagnosticsService;

        public Registers(IDiagnosticsService diagnosticsService)
        {
            _diagnosticsService = diagnosticsService;
        }

        public void UpdateIpAndPip(int newLocation)
        {
            var ip = Read(RegisterType.Ip);

            Write(RegisterType.Pip, ip);

            Write(RegisterType.Ip, newLocation);
        }

        public int Read(byte registerType)
        {
            var typeEnum = GetRegisterType(registerType);

            return Read(typeEnum);
        }

        public int Read(RegisterType registerType)
        {
            var value = _registerDict[registerType];
            
            return value;
        }

        public void Write(byte registerType, int value)
        {
            var typeEnum = GetRegisterType(registerType);

            Write(typeEnum, value);
        }

        public void Write(RegisterType registerType, int value)
        {
            var writeEvent = new RegisterWriteEvent
            {
                RegisterType = registerType.ToString(),
                NewValue = value,
                OldValue = _registerDict[registerType]
            };

            _diagnosticsService.AddEvent(writeEvent);

            _registerDict[registerType] = value;
        }

        private RegisterType GetRegisterType(byte registerTypeByte)
        {
            if (Enum.IsDefined(typeof(RegisterType), registerTypeByte))
            {
                return (RegisterType)registerTypeByte;
            }

            return RegisterType.R1;
        }
    }
}
