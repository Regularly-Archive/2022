using System.Collections;

foreach (DictionaryEntry env in Environment.GetEnvironmentVariables())
{
    if (env.Key.ToString()?.StartsWith("DOTNET_") == false) continue;
    Console.WriteLine($"{env.Key}={env.Value}");
}

throw new Exception("Manual Crash");
