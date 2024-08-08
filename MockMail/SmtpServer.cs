using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MockMail
{
    public class SmtpServer
    {
        private readonly TcpListener _listener;
        private TcpClient _tcpClient;
        private NetworkStream reader;
        private StreamWriter writer;

        public SmtpServer()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 25);
            _listener = new TcpListener(endpoint);
        }
        public async Task StartAsync()
        {
            _listener.Start();
            try
            {
                while (_listener != null && _listener.Server.IsBound)
                {
                    _tcpClient = await _listener.AcceptTcpClientAsync();
                    if (_tcpClient is null || _tcpClient.Client is null || !_tcpClient.Client.Connected)
                    {
                        return;
                    }

                    reader = _tcpClient.GetStream();
                    writer = new StreamWriter(reader, new UTF8Encoding(false)) { AutoFlush = true, NewLine = "\r\n" };
                    Console.WriteLine($"Connection made {_tcpClient.Client.RemoteEndPoint}");
                    await writer.WriteLineAsync($"220 from Server!");

                    while (true)
                    {
                        var line = await ReadLine(reader);
                        Console.WriteLine(line);
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<string> ReadLine(NetworkStream reader)
        {
            byte[] buffer = new byte[1];
            MemoryStream ms = new MemoryStream();

            while(true)
            {
                if (reader.DataAvailable)
                {
                    while (reader != null && await reader.ReadAsync(buffer, 0, buffer.Length) == 1)
                    {
                        byte b = buffer[0];

                        switch (b)
                        {
                            case (byte)'\n':
                                reader = null;
                                break;

                            case (byte)'\r':
                                break;

                            default:
                                ms.WriteByte(b);
                                break;
                        }
                    }
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        public static async Task Main(string[] args)
        {
            SmtpServer server = new SmtpServer();
            await server.StartAsync();
        }
    }
}
