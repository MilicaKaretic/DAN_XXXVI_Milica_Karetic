using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_XXXVI_Milica_Karetic
{
    class Program
    {
        private static object locker = new object();
        public static List<int> list = new List<int>(10000);
        public static int[,] matrix;      
        static Random rnd = new Random();
        public static string fileName = "FileNumbers.txt";

        /// <summary>
        /// Method for generate 10000 random numbers and add them into list
        /// </summary>
        public static void GenerateNumbers()
        {
            int num;
            lock (locker)
            {
                for (int i = 0; i < 10000; i++)
                {
                    num = rnd.Next(10, 100);
                    list.Add(num);
                }
                Monitor.Pulse(locker);
            }
        }

        /// <summary>
        /// Method for generate matrix with numbers from list
        /// </summary>
        public static void GenerateMatrix()
        {
            lock (locker)
            {
                matrix = new int[100, 100];

                //Wait until list count is 10000
                while (list.Count < 10000)
                {
                    Monitor.Wait(locker);
                }

                //numbers from list to matrix
                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        matrix[i, j] = list[j + i * 100];
                    }
                }
            }
        }

        /// <summary>
        /// Method for read numbers from file and write to console
        /// </summary>
        public static void ReadFromFile()
        {
            lock (fileName)
            {
                Monitor.Wait(fileName);
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.Write(line + " ");
                    }
                }
            }
        }

        /// <summary>
        /// Method that generate array with only odd numbers from matrix
        /// </summary>
        /// <remarks>
        /// Write numbers from array to file
        /// </remarks>
        public static void LogIntoFile()
        {
            //odd numbers
            int[] arr = list.Where(i => i % 2 == 1).ToArray();

            lock (fileName)
            {
                using (StreamWriter sw = File.CreateText(fileName))
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        sw.WriteLine(arr[i]);
                    }
                }
                Monitor.Pulse(fileName);
            }
        }



        static void Main(string[] args)
        {
            Thread t1 = new Thread(GenerateMatrix)
            {
                Name = "GenerateMatrix"
            };
            Thread t2 = new Thread(GenerateNumbers)
            {
                Name = "GenerateNumbers"
            };
            
            //start 1. i 2. threads
            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            Thread t3 = new Thread(LogIntoFile)
            {
                Name = "OddNumbersToFile"
            };
            Thread t4 = new Thread(ReadFromFile)
            {
                Name = "ReadNumbers"
            };

            //start 3. and 4. threads
            t3.Start();
            t4.Start();
            
            t3.Join();
            t4.Join();

            Console.ReadKey();
        }
    }
}
