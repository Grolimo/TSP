// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System;
using System.IO;
using System.Reflection;
using Language;

namespace tslcli
{
    class Program
    {
        private static string Filename;
        private static Version version;

        private static bool Execute = false;

        private static void ShowUsage()
        {
            Console.WriteLine("tslcli <option>");
            Console.WriteLine("Options:");
            Console.WriteLine("-e <filename>   Execute <filename>.");
            Console.WriteLine("-h              Show this page.");
            Console.WriteLine("-v              Show version.");
        }

        private static void SwitchArgs(string[] args)
        {
            if (args == null)
            {
                ShowUsage();
                return;
            }
            var i = 0;
            var l = args.Length;
            if (l == 0)
            {
                ShowUsage();
                return;
            }
            while (i < l)
            {
                if (args[i] == "-e")
                {
                    Execute = true;
                    Filename = args[i + 1];
                    i += 2;
                }
                else if (args[i] == "-v")
                {
                    version = Assembly.GetEntryAssembly().GetName().Version;
                    Console.WriteLine($"tslcli Version: {version}");
                    i += 1;
                }
                else
                {
                    ShowUsage();
                    break;
                }
            }

        }

        private static string LoadFile(string fileName)
        {
            string result = string.Empty;
            if (File.Exists(fileName))
            {
                using var sr = new StreamReader(fileName);
                result = sr.ReadToEnd();
            }
            else
            {
                Console.WriteLine($"File not found: {fileName}");
            }
            return result;
        }

        private static void Run()
        {
            var program = LoadFile(Filename);
            if (program != string.Empty)
            {
                Interpreter Interpreter = new Interpreter();
                try
                {
                    Interpreter.Execute(program);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static void Main(string[] args)
        {
            SwitchArgs(args);
            if (Execute)
            {
                Run();
            }
        }
    }
}
