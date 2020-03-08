using PrototypeCore.Interfaces;
using PrototypeCore.Interfaces.Diagnostics;
using PrototypeCore.Models;
using PrototypeCore.Models.Diagnostics;
using System;
using System.Collections.Generic;

namespace PrototypeCore.Services
{
    public class Processor : IProcessor
    {
        private readonly IRegisters _registers;
        private readonly IIntreruptQueue _intreruptQueue;
        private readonly IMemory _memory;
        private readonly IDiagnosticsService _diagnosticsService;

        public Processor(IRegisters registers, IIntreruptQueue intreruptQueue, IMemory memory, IDiagnosticsService diagnosticsService)
        {
            _registers = registers;
            _intreruptQueue = intreruptQueue;
            _memory = memory;
            _diagnosticsService = diagnosticsService;
        }

        public void Step()
        {
            _diagnosticsService.AddEvent(new ProcessorCycleStartEvent());

            _intreruptQueue.ProcessSignal();

            JumpToStartIfIntrerupt();

            ExecuteNextInstruction();

            _diagnosticsService.AddEvent(new ProcessorCycleEndEvent());
        }

        void JumpToStartIfIntrerupt()
        {
            var ip = _registers.Read(RegisterType.Ip);

            if (ip > 63 && _intreruptQueue.HasIntrerupt())
            {
                _registers.UpdateIpAndPip(0);

                var (intreruptType, intreruptData) = _intreruptQueue.GetIntrerupt();

                _registers.Write(RegisterType.It, intreruptType);

                _registers.Write(RegisterType.Id, intreruptData);
            }
        }


        void ExecuteNextInstruction()
        {
            var instructionTypeToActionMapping = new Dictionary<InstructionType, Action<int>>()
            {
                [InstructionType.Jump] = Jump,
                [InstructionType.JumpZero] = JumpZero,
                [InstructionType.JumpPositive] = JumpPositive,
                [InstructionType.JumpNegative] = JumpNegative,
                [InstructionType.MovLiteral] = MovLiteral,
                [InstructionType.MovRegisters] = MovRegisters,
                [InstructionType.MovFromMemory] = MovFromMemory,
                [InstructionType.MovToMemory] = MoveToMemory,
                [InstructionType.IntreruptClock] = IntreruptClock,
                [InstructionType.IntreruptSoft] = IntreruptSoft,
                [InstructionType.Add] = Add,
                [InstructionType.Sub] = Sub,
                [InstructionType.Mul] = Mul,
                [InstructionType.Div] = Div,
                [InstructionType.Mod] = Mod
            };

            var ip = _registers.Read(RegisterType.Ip);

            var instructionOpCode = _memory.Read(ip);

            if (!Enum.IsDefined(typeof(InstructionType), instructionOpCode))
            {
                return;
            }

            var instructionType = (InstructionType)instructionOpCode;

            instructionTypeToActionMapping[instructionType](ip);
        }


        void Jump(int ip)
        {
            var registerWithNewLocation = _memory.Read(ip + 1);

            var value = _registers.Read(registerWithNewLocation);

            _registers.UpdateIpAndPip(value);
        }

        void JumpZero(int ip) => ConditionalJump(ip, x => x == 0);

        void JumpPositive(int ip) => ConditionalJump(ip, x => x > 0);

        void JumpNegative(int ip) => ConditionalJump(ip, x => x < 0);

        void MovLiteral(int ip) => ExecuteAndMove(3, () =>
        {
            var (regDst, literal) = _memory.ReadTwo(ip + 1);

            _registers.Write(regDst, literal);
        });


        void MovRegisters(int ip) => ExecuteAndMove(3, () =>
        {
            var (regDst, regSrc) = _memory.ReadTwo(ip + 1);

            var value = _registers.Read(regSrc);

            _registers.Write(regDst, value);
        });


        void MovFromMemory(int ip) => ExecuteAndMove(3, () =>
        {
            var (dstReg, regContainingMemLocation) = _memory.ReadTwo(ip + 1);

            var memLocation = _registers.Read(regContainingMemLocation);

            var value = _memory.Read(memLocation);

            _registers.Write(dstReg, value);
        });


        void MoveToMemory(int ip) => ExecuteAndMove(3, () =>
        {
            var (regContainingMemLocation, srcReg) = _memory.ReadTwo(ip + 1);

            var value = (byte)_registers.Read(srcReg);

            var memLocation = _registers.Read(regContainingMemLocation);

            _memory.Write(memLocation, value);
        });


        void IntreruptClock(int ip) => ExecuteAndMove(3, () =>
        {
            var (regNrOfCycles, regIntreruptData) = _memory.ReadTwo(ip + 1);

            var nrOfCycles = (byte)_registers.Read(regNrOfCycles);

            var intrData = (byte)_registers.Read(regIntreruptData);

            _intreruptQueue.EnqueueClockIntrerupt(nrOfCycles, intrData);
        });

        void IntreruptSoft(int ip) => ExecuteAndMove(2, () =>
        {
            var regIntreruptData = _memory.Read(ip + 1);

            var intrData = (byte)_registers.Read(regIntreruptData);

            _intreruptQueue.EnqueueSoftIntrerupt(intrData);
        });


        void Add(int ip) => ArithMeticOp(ip, (l, r) => l + r);

        void Sub(int ip) => ArithMeticOp(ip, (l, r) => l - r);

        void Mul(int ip) => ArithMeticOp(ip, (l, r) => l * r);

        void Div(int ip) => ArithMeticOp(ip, (l, r) => l / r);

        void Mod(int ip) => ArithMeticOp(ip, (l, r) => l % r);

        void ConditionalJump(int ip, Func<int, bool> condition)
        {
            var (regNewLocation, regToTest) = _memory.ReadTwo(ip + 1);

            var newLocation = _registers.Read(regNewLocation);

            var value = _registers.Read(regToTest);

            if (condition(value))
            {
                _registers.UpdateIpAndPip(newLocation);
            }
        }

        void ArithMeticOp(int ip, Func<int, int, int> arithmeticFn) => ExecuteAndMove(4, () =>
        {
            var (regResult, regOp1, regOp2) = _memory.ReadThree(ip + 1);

            var op1 = _registers.Read(regOp1);

            var op2 = _registers.Read(regOp2);

            try
            {
                var result = arithmeticFn(op1, op2);
                _registers.Write(regResult, result);
            }
            catch (Exception e)
            {

            }
        });

        private void ExecuteAndMove(int nrBytes, Action executeAction)
        {
            var ip = _registers.Read(RegisterType.Ip);

            executeAction();

            _registers.UpdateIpAndPip(ip + nrBytes);
        }
    }
}
