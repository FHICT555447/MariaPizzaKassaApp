using System.Net;
using System.Net.Sockets;

namespace dotnet_pizza_protocol {
    public class UdpSender(string ipAddress, int port) {
        private readonly UdpClient Client = new();
        private readonly IPEndPoint Endpoint = new(IPAddress.Parse(ipAddress), port);

        public void Send(byte[] data) {
            try
            {
                Client.Send(data, data.Length, Endpoint);
                Console.WriteLine("Message sent successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending message: {e.Message}");
            }
        }
    }

    public class UdpReceiver : IDisposable
    {
        public delegate void DataReceivedHandler(byte[] bytes);
        public event DataReceivedHandler? OnDataReceived;

        private readonly int _port;
        private UdpClient? _udpClient;
        private bool _isListening;

        public UdpReceiver(int port)
        {
            _port = port;
            
            if (_isListening)
                throw new InvalidOperationException("Listener is already running.");

            _udpClient = new UdpClient(_port);
            _isListening = true;

            Task.Run(() => Listen());
        }

        public void Stop()
        {
            _isListening = false;
            _udpClient?.Close();
            _udpClient = null;
        }

        private void Listen()
        {
            IPEndPoint remoteEndPoint = new(IPAddress.Any, _port);

            while (_isListening && _udpClient is not null)
            {
                try
                {
                    byte[] receivedBytes = _udpClient.Receive(ref remoteEndPoint);
                    OnDataReceived?.Invoke(receivedBytes);
                }
                catch (SocketException) when (!_isListening)
                {
                    Console.WriteLine("Listener stopped");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}