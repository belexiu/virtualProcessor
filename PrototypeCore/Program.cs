using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PrototypeCore.Interfaces;
using PrototypeCore.Interfaces.Diagnostics;
using PrototypeCore.Models;
using PrototypeCore.Services;
using PrototypeCore.Services.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PrototypeCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var sp = GetServiceProvider();

            var processor = sp.GetService<IProcessor>();

            var memory = sp.GetService<IMemory>();

            var registers = sp.GetService<IRegisters>();

            var diagService = sp.GetService<IDiagnosticsService>();



            // value to jump for address
            memory.Write(0, (byte)InstructionType.MovLiteral);
            memory.Write(1, (byte)RegisterType.R4);
            memory.Write(2, 12);

            // increment for register
            memory.Write(3, (byte)InstructionType.MovLiteral);
            memory.Write(4, (byte)RegisterType.R3);
            memory.Write(5, 1);


            // r1 - value
            memory.Write(6, (byte)InstructionType.MovLiteral);
            memory.Write(7, (byte) RegisterType.R1);
            memory.Write(8, 0);

            // r2 - address
            memory.Write(9, (byte)InstructionType.MovLiteral);
            memory.Write(10, (byte)RegisterType.R2);
            memory.Write(11, 100);

            // write to address
            memory.Write(12, (byte)InstructionType.MovToMemory);
            memory.Write(13, (byte)RegisterType.R2);
            memory.Write(14, (byte)RegisterType.R1);

            // increment value and address
            memory.Write(15, (byte)InstructionType.Add);
            memory.Write(16, (byte)RegisterType.R1);
            memory.Write(17, (byte)RegisterType.R1);
            memory.Write(18, (byte)RegisterType.R3);

            memory.Write(19, (byte)InstructionType.Add);
            memory.Write(20, (byte)RegisterType.R2);
            memory.Write(21, (byte)RegisterType.R2);
            memory.Write(22, (byte)RegisterType.R3);
            // reapeat

            memory.Write(23, (byte)InstructionType.Jump);
            memory.Write(24, (byte)RegisterType.R4);

            while (true)
            {
                Console.WriteLine(JsonConvert.SerializeObject(diagService.GetAllEvents(), Formatting.Indented));
                PrintProcessorState(memory, registers);
                processor.Step();
                Console.ReadKey();
            }


        }

        static IServiceProvider GetServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IIntreruptQueue, IntreruptQueue>();
            serviceCollection.AddSingleton<IRegisters, Registers>();
            serviceCollection.AddSingleton<IMemory, Memory>();
            serviceCollection.AddSingleton<IProcessor, Processor>();
            serviceCollection.AddSingleton<IDiagnosticsService, DiagnosticsService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }

        static void PrintMemory(IMemory memory, IRegisters registers)
        {
            for (int i = 0; i < 256; i++)
            {

                if (registers.Read(RegisterType.Ip) == i)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                }
                if (registers.Read(RegisterType.Pip) == i)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                }

                if (i % 16 == 0)
                {
                    Console.Write($"\n|");
                }
                
                Console.Write($" {memory.Read(i) : 000} |");

                Console.BackgroundColor = ConsoleColor.Black;
            }
            Console.WriteLine();
        }


        static void PrintProcessorState(IMemory memory, IRegisters registers)
        {
            var registerPointingColors = new Dictionary<RegisterType, ConsoleColor>() 
            { 
                [RegisterType.Ip] = ConsoleColor.Blue,
                [RegisterType.Pip] = ConsoleColor.DarkYellow
            };

            var regDict = new Dictionary<RegisterType, int>();

            ConsoleColor GetConsoleColor(RegisterType registerType)
            {
                if (registerPointingColors.ContainsKey(registerType))
                {
                    return registerPointingColors[registerType];
                }
                return ConsoleColor.Black;
            }


            // print registers
            foreach (var regType in Enum.GetValues(typeof(RegisterType)))
            {
                var typedRegisterType = (RegisterType)regType;

                var regValue = registers.Read(typedRegisterType);

                regDict[typedRegisterType] = regValue;

                PrintInColor(GetConsoleColor(typedRegisterType), $"{typedRegisterType} = {regValue}\n");
            }

            Console.Write("\n");

            for (int i = 0; i < 256; i++)
            {
                if (i % 16 == 0)
                {
                    Console.Write($"\n|");
                }

                var regMatching = regDict.Where(x => x.Value == i && registerPointingColors.ContainsKey(x.Key)).ToList();

                var color = regMatching.Any() ? GetConsoleColor(regMatching.First().Key) : ConsoleColor.Black;

                Console.Write(" ");

                PrintInColor(color, $"{memory.Read(i): 000}");
   
                Console.Write(" |");
            }
            Console.WriteLine();




            // print memory



        }

        static void PrintInColor(ConsoleColor color, string str)
        {
            Console.BackgroundColor = color;
            Console.Write(str);
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
