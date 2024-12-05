using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace dotnet_pizza_protocol {
    public class DataSender(string ipAddress, int port) {
        private readonly UdpClient Client = new();
        private readonly IPEndPoint Endpoint = new(IPAddress.Parse(ipAddress), port);

        void Send(byte[] data) {
            try
            {
                // Send the message
                Client.Send(data, data.Length, Endpoint);
                Console.WriteLine("Message sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }
    }

    public class UdpMessageEventArgs(byte command, byte[] data, IPEndPoint source) : EventArgs
    {
        public byte Command { get; } = command;
        public byte[] Data { get; } = data;
        public IPEndPoint Source { get; } = source;
    }

    public class DataReceiver : IDisposable {
        private readonly UdpClient Listener;
        // private readonly IPEndPoint Endpoint = new(IPAddress.Parse(ipAddress), port);
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task _receiverTask = Task.CompletedTask;
        private bool _disposed = false;

        // Thread-safe queue to store received messages
        public BlockingCollection<UdpMessageEventArgs> MessageQueue { get; } = [];

        // Event that can be used for more immediate processing
        public event EventHandler<UdpMessageEventArgs>? MessageReceived;

        public DataReceiver(int port) {
            Listener = new(port);

            StartListening();
        }

        private void StartListening()
        {
            _receiverTask = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        UdpReceiveResult result = await Listener.ReceiveAsync();

                        // First byte is the command
                        byte command = result.Buffer[0];
                        
                        // Rest of the bytes are data
                        byte[] data = new byte[result.Buffer.Length - 1];
                        Array.Copy(result.Buffer, 1, data, 0, data.Length);

                        // Create message event args
                        var messageArgs = new UdpMessageEventArgs(command, data, result.RemoteEndPoint);

                        // Add to thread-safe queue
                        MessageQueue.Add(messageArgs);

                        // Trigger event for immediate processing if needed
                        MessageReceived?.Invoke(this, messageArgs);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error receiving message: {ex.Message}");
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Cancel and wait for the task
                    _cancellationTokenSource.Cancel();
                    
                    // Wait for the receiver task to complete
                    _receiverTask.Wait();

                    // Close resources
                    Listener.Close();
                    MessageQueue.Dispose();
                    _cancellationTokenSource.Dispose();
                }

                _disposed = true;
            }
        }

        // Finalizer
        ~DataReceiver()
        {
            Dispose(false);
        }
    }
}