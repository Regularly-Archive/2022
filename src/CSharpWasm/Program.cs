using System;

namespace CSharpWasm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    public class Greeter
    {
        public static string SayHello(string name)
        {
            return string.Format("Hello {0}", name);
        }
    }
}
