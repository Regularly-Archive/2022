using Microsoft.AspNetCore.Connections;
using System.Buffers;
using System.Text;

public class TcpConnectionHandler : ConnectionHandler
{
    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        Console.WriteLine(connection.ConnectionId + " connected");

        while (true)
        {
            var result = await connection.Transport.Input.ReadAsync();
            var buffer = result.Buffer;

            Console.WriteLine("Receive -> {0}", Encoding.UTF8.GetString(result.Buffer.ToArray()));

            foreach (var segment in buffer)
            {
                await connection.Transport.Output.WriteAsync(segment);
            }
            Console.WriteLine("Send -> {0}", Encoding.UTF8.GetString(result.Buffer.ToArray()));

            if (result.IsCompleted)
            {
                break;
            }

            connection.Transport.Input.AdvanceTo(buffer.End);
        }

        Console.WriteLine(connection.ConnectionId + " disconnected");
    }
}