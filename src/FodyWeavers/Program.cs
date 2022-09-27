using FodyWeavers.Attributes;
using HelloWorld.Fody;
using JetBrains.Annotations;
using System.Reflection.Emit;

public class Program
{
    static async Task Main(string[] args)
    {
        //// Sync Method Logging
        //Add(1, 2);

        //// Async Method Logging 
        //await AddAsync(1, 2);

        //// Exception Handling
        //Divide(1, 1);

        ////// NullGuard
        ////Split(null);

        Echo();
    }

    [Logging]
    static int Add(int a, int b) => a + b;

    [Logging]
    static Task<int> AddAsync(int a, int b) => Task.FromResult(a + b);

    [Logging]
    static decimal Divide(decimal a, decimal b) => a / b;

    [HelloWorld]
    public static void Echo()
    {
        Console.WriteLine("Hello Fody.");
    }
}





