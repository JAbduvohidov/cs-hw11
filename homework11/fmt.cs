using System;

namespace homework11
{
    internal static class fmt
    {
        public static void Print(string value)
        {
            Console.Write(value);
        }

        public static void Print(string value, ConsoleColor color)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(value);
            Console.ForegroundColor = foregroundColor;
        }

        /// <summary>
        /// Alternative for Console.WriteLine()
        /// Prints line
        /// </summary>
        public static void Println()
        {
            Console.WriteLine();
        }

        /// <summary>
        /// Alternative for Console.WriteLine("hello")
        /// Prints text
        /// </summary>
        public static void Println(string value)
        {
            Console.WriteLine(value);
        }

        /// <summary>
        /// Alternative for Console.WriteLine("hello")
        /// Prints colorful text
        /// </summary>
        public static void Println(string value, ConsoleColor color)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ForegroundColor = foregroundColor;
        }

        public static string Scan()
        {
            return Console.ReadLine();
        }
    }
}