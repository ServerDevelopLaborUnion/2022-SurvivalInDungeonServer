using System.Net;
using System.Net.Sockets;

TcpClient tcpClient = new TcpClient("127.0.0.1", 3000);

List<byte> bytes = new List<byte>();
bytes.AddRange(BitConverter.GetBytes((uint)1));
bytes.AddRange(BitConverter.GetBytes((uint)1));
bytes.AddRange(new Byte[1]);

Console.WriteLine("Connected to server");

int cnt = 0;

while (true)
{
    tcpClient.Client.Send(bytes.ToArray());
    Console.WriteLine($"{cnt++}");

    if (!tcpClient.Connected)
    {

        tcpClient.Close();
        tcpClient = new TcpClient("127.0.0.1", 3000);
        Console.WriteLine("Reconnected to server");
    }
    Thread.Sleep(1000);
}