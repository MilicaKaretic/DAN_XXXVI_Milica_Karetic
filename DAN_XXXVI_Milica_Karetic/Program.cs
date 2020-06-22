using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Method for generate numbers
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
        /// Method for generate matrix
        /// </summary>
        public static void GenerateMatrix()
        {
            lock (locker)
            {
                matrix = new int[100, 100];
                while (list.Count < 10000)
                {
                    Monitor.Wait(locker);
                }
                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        matrix[i, j] = list[j + i * 100];
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            
        }
    }
}
