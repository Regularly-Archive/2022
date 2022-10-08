// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

var tcpClient = new TcpClient();
await tcpClient.ConnectAsync("localhost", 6000);
var stream = tcpClient.GetStream();

var bufferSize = 64;

while (true)
{
    while (!tcpClient.Connected)
    {
        await tcpClient.ConnectAsync("localhost", 6000);
    }

    //if (Console.ReadKey().Key == ConsoleKey.Q) break;
    await RunWrite(stream);
    var bytes = await RunRead(stream);

    var response = Encoding.UTF8.GetString(bytes);
    Console.WriteLine("Receive -> {0}", response);

    await Task.Delay(500);
}


async Task RunWrite(NetworkStream stream)
{
    var payload = JsonConvert.SerializeObject(new { Time = DateTime.Now, Id = Guid.NewGuid().ToString() });
    await stream.WriteAsync(Encoding.UTF8.GetBytes(payload));
    Console.WriteLine("Send -> {0}", payload);
}

async Task<byte[]> RunRead(NetworkStream stream)
{
    var IncomingData = new List<byte>();
    byte[] buffer = new byte[bufferSize];
    int bytesRead;
    int auxBuffer = bufferSize;

    //TODO: add the cancellation token
    //using (var readCts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))

    while ((auxBuffer == bufferSize) && (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) != 0)
    {
        auxBuffer = bytesRead;
        byte[] tempData = new byte[bytesRead];
        Array.Copy(buffer, 0, tempData, 0, bytesRead);
        IncomingData.AddRange(tempData);
    }


    return IncomingData.ToArray();
}
