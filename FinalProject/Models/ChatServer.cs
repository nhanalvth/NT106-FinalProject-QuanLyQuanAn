using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FinalProject
{
    public class ChatServer
    {
        private TcpListener listener;
        private List<TcpClient> clients = new List<TcpClient>();
        private bool isRunning = false;
        private Thread acceptThread;

        public void Start(int port = 8888)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            isRunning = true;

            acceptThread = new Thread(() =>
            {
                try
                {
                    while (isRunning)
                    {
                        TcpClient client = listener.AcceptTcpClient(); // Blocking call
                        lock (clients)
                        {
                            clients.Add(client);
                        }

                        Thread clientThread = new Thread(() => HandleClient(client))
                        {
                            IsBackground = true
                        };
                        clientThread.Start();
                    }
                }
                catch (SocketException ex)
                {
                    // Chỉ log lỗi nếu server chưa dừng
                    if (isRunning)
                        Console.WriteLine("Lỗi socket khi Accept: " + ex.Message);
                }
            });

            acceptThread.IsBackground = true;
            acceptThread.Start();
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                while (isRunning)
                {
                    int length = stream.Read(buffer, 0, buffer.Length);
                    if (length == 0)
                        break;

                    string message = Encoding.UTF8.GetString(buffer, 0, length);
                    BroadcastMessage(message, client);
                }
            }
            catch
            {
                // Client đã đóng kết nối hoặc lỗi stream
            }

            lock (clients)
            {
                clients.Remove(client);
            }

            client.Close();
        }

        private void BroadcastMessage(string message, TcpClient sender)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            lock (clients)
            {
                foreach (var client in clients)
                {
                    if (client != sender)
                    {
                        try
                        {
                            client.GetStream().Write(data, 0, data.Length);
                        }
                        catch { /* Bỏ qua lỗi ghi */ }
                    }
                }
            }
        }

        public void Stop()
        {
            isRunning = false;

            try
            {
                listener?.Stop(); // Sẽ gây SocketException trong AcceptTcpClient → đã bắt
            }
            catch { }

            lock (clients)
            {
                foreach (var client in clients)
                {
                    try
                    {
                        client.Close();
                    }
                    catch { }
                }

                clients.Clear();
            }

            if (acceptThread != null && acceptThread.IsAlive)
            {
                acceptThread.Join(1000); // chờ thread kết thúc
            }
        }
    }
}
