using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Zeiss.PublicationManager.Data.DataSet;
using Zeiss.PublicationManager.Data.DataSet.IO.Write;


namespace Zeiss.PublicationManager.Data.Excel.IO.ComponentTest
{
    public class Program
    {
        //string[] args
        public static void Main(string[] args)
        {
            Console.WriteLine("Initialize DataSet IO Component Test");

            int[] modes = null;
            if (args?.Length >= 1)
            {
                modes = new int[args.Length];

                Console.Write("Mode Arguments:");

                for (int i = 0; i < modes.Length; i++)
                {
                    modes[i] = Convert.ToInt32(args[i]);

                    Console.Write(" {0}", modes[i]);
                }

                Console.WriteLine();
            }

            SelectIOMode(modes);

            Console.WriteLine("\nTask complete. Press any key to close\n");
            Console.ReadKey();
        }

        public static void SelectIOMode(int[] modes)
        {
            Console.WriteLine("\nSelect Mode:\n" +
                "(0) Test Writing\n" +
                "(1) Test Reading:\n");

            int commandMode;
            if (modes?.Length > 0)
                commandMode = modes[0];
            else
                commandMode = ReadMode();

            SelectIOMode(commandMode, modes);
        }

        private static int ReadMode()
        {
            int mode;
            while (!Int32.TryParse(Console.ReadLine(), out mode))
            {
                Console.WriteLine("Mode must be an Integer!\n");
            }

            return mode;
        }


        private static void SelectIOMode(int commandMode, int[] modes)
        {
            Console.WriteLine();
            switch (commandMode)
            {
                case 0:
                    Console.WriteLine("(0) Test Writing selected\n");
                    Write.TestWriting writeTest = new();

                    Console.WriteLine("Enter number of to generating DataSets:\n");
                    int count = modes?.Length > 1 ? modes[1] : ReadMode();
                    Console.WriteLine("{0} DataSets will be generated.\n", count);
                    writeTest.WriteRandomDataSet(count);
                    break;

                case 1:
                    Console.WriteLine("(1) Test Reading selected\n");
                    break;

                default:
                    Console.WriteLine("Invalid mode entered!\n" +
                        "Please select valid mode:\n");
                    SelectIOMode(ReadMode(), modes);
                    break;
            }
            Console.WriteLine();
        }
    }
}
