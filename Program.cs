using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace dotnet_pizza_protocol {
    public class UdpReceiver : IDisposable
    {
        private readonly UdpClient _listener;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task _receiverTask = Task.CompletedTask;
        private bool _disposed = false;

        public BlockingCollection<UdpMessageEventArgs> MessageQueue { get; } 
            = new BlockingCollection<UdpMessageEventArgs>();

        public event EventHandler<UdpMessageEventArgs>? MessageReceived;

        public UdpReceiver(int port = 9999)
        {
            _listener = new UdpClient(port);
            _cancellationTokenSource = new CancellationTokenSource();

            // Start listening in the constructor
            StartListening();
        }

        private void StartListening()
        {
            Console.WriteLine($"Starting listening");

            _receiverTask = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        UdpReceiveResult result = await _listener.ReceiveAsync();

                        byte command = result.Buffer[0];
                        
                        byte[] data = new byte[result.Buffer.Length - 1];
                        Array.Copy(result.Buffer, 1, data, 0, data.Length);

                        var messageArgs = new UdpMessageEventArgs(command, data, result.RemoteEndPoint);

                        MessageQueue.Add(messageArgs);
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

        // Implement IDisposable
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
                    _listener.Close();
                    MessageQueue.Dispose();
                    _cancellationTokenSource.Dispose();
                }

                _disposed = true;
            }
        }

        // Finalizer
        ~UdpReceiver()
        {
            Dispose(false);
            Console.WriteLine("Disposed");
        }
    }

    // Usage example
    class Program
    {
        static async Task Main()
        {
            // Using statement ensures proper disposal
            using var receiver = new UdpReceiver();
            // Process messages
            await Task.Run(() => {
                foreach (var message in receiver.MessageQueue.GetConsumingEnumerable())
                {
                    ProcessMessage(message);
                }
            });
            // Automatically calls Dispose() here
        }

        static void ProcessMessage(UdpMessageEventArgs message)
        {
            Console.WriteLine(message.Data);
            // Message processing logic
        }
    }
}
