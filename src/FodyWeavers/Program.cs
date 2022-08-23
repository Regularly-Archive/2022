using FodyWeavers.Attributes;
using HelloWorld.Fody;
using JetBrains.Annotations;

[assembly: PublicAPI]
[module: PublicAPI]

[UsedImplicitly]
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

        //// NullGuard
        //Split(null);

        Echo();
    }

    [Logging]
    static int Add(int a, int b) => a + b;

    [Logging]
    static Task<int> AddAsync(int a, int b) => Task.FromResult(a + b);

    [Logging]
    static decimal Divide(decimal a, decimal b) => a / b;

    [NotNull]
    public static string[] Split([NotNull][System.Diagnostics.CodeAnalysis.NotNull] Foo foo)
    {
        return foo.Colors.Split(new char[] { ',' });
    }

    [HelloWorld]
    static void Echo()
    {
        Console.WriteLine("Hello Fody.");
    }

    [UsedImplicitly]
    public class Foo
    {
        [NotNull]
        public string Colors { get; set; }
    }
}





