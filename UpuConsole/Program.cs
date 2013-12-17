using CommandLine;
using System;
using System.Diagnostics;
using System.Reflection;
using UpuCore;

namespace UpuConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var upu = new UpuConsole();

            if (!Parser.Default.ParseArguments(args, upu))
            {
                if (args.Length > 0)
                {
                    upu.InputFile = args[0];
                }
                else
                {
                    Console.WriteLine("Parameter error"); // todo: display generated usage
                    Console.WriteLine(upu.GetUsage());
                    Environment.Exit(9);
                }
            }

            int errorCode = 0;

            try
            {
                upu.Start();
            }
            catch (Exception e)
            {
                string error = e.ToString();
                if (e is System.Reflection.ReflectionTypeLoadException)
                {
                    var typeLoadException = e as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                    error += "\n" + loaderExceptions.ToString();
                }

                Console.WriteLine(error);

                errorCode = -1;
                if (Debugger.IsAttached)
                    Debugger.Break();
            }

            Environment.Exit(errorCode);

        }
    }
}
