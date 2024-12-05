using System.Net;
using System.Net.Sockets;

namespace dotnet_pizza_protocol
{
    class Program {
        // private static void SendCustomMessage(string ipAddress, int port, byte[] data)
        //     {
        //     // Create UDP client
        //     using var udpClient = new UdpClient();
        //     // Combine command and data into a single byte array

        //     // Create endpoint
        //     var endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

        //     try
        //     {
        //         // Send the message
        //         udpClient.Send(data, data.Length, endpoint);
        //         Console.WriteLine("Message sent successfully!");
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error sending message: {ex.Message}");
        //     }
        // }

        // static void Main() {
        //     // class ArduinoUDPSender
        //     string arduinoIpAddress = "192.168.68.242";  // Replace with actual IP
        //     int port = 8888;

        //     var minimized = new PizzaOrderMinimized(9, 4, 14);
        //     var minimizedMsg = minimized.Serialize();
        //     var minimizedModsMsg = new PizzaOrderMinimizedModifications(minimized, [
        //         new MinimizedModification(Constants.ModBit | 14),
        //         new MinimizedModification(47)
        //     ]).Serialize();
        //     var expandedMsg = new PizzaOrderExpanded(10, "Pepperoni", "Extra large", [
        //         new ExpandedModification(ModificationType.Add, "mushrooms"),
        //         new ExpandedModification(ModificationType.Add, "jalapenos"),
        //         new ExpandedModification(ModificationType.Remove, "pepperoni")
        //     ]).Serialize();

        //     var expanded = PizzaOrderExpanded.Deserialize(expandedMsg, expandedMsg.Length);

        //     // Console.WriteLine(expanded.GetCount());

        //     // SendCustomMessage(arduinoIpAddress, port, minimizedMsg);
        //     // SendCustomMessage(arduinoIpAddress, port, minimizedModsMsg);
        //     // SendCustomMessage(arduinoIpAddress, port, expandedMsg);
        //     // SendCustomMessage(arduinoIpAddress, port, expanded.Serialize());
        //     SendCustomMessage(arduinoIpAddress, port, [(byte)0b01000000]);
        //     SendCustomMessage(arduinoIpAddress, port, [(byte)0b00110000]);

        //     // // Example of sending a message
        //     // // Command 0x01, with data bytes [0x02, 0x03, 0x04]
        //     // byte[] dataBytes = [0b00000010, 0b00010101, 0b00010000];
        //     // SendCustomMessage(arduinoIpAddress, port, dataBytes);
            
        // }

        private static void Main()
        {
            // Listen on all network interfaces
            UdpClient listener = new(9999);  // Same port as in Arduino
            
            Console.WriteLine("UDP Listener started. Waiting for messages...");

            try 
            {
                while (true)
                {
                    // Receive data from any source
                    IPEndPoint remoteEP = new(IPAddress.Any, 0);
                    byte[] receivedBytes = listener.Receive(ref remoteEP);

                    // First byte is the command
                    byte command = receivedBytes[0];
                    
                    // Rest of the bytes are data
                    byte[] data = new byte[receivedBytes.Length - 1];
                    Array.Copy(receivedBytes, 1, data, 0, data.Length);

                    // Process the received message
                    ProcessMessage(command, data, remoteEP);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving message: {ex.Message}");
            }
            finally 
            {
                listener.Close();
            }
        }

        private static void ProcessMessage(byte command, byte[] data, IPEndPoint source)
        {
            Console.WriteLine($"Received from {source.Address}:");
            Console.WriteLine($"Command: 0x{command:X2}");
            
            Console.Write("Data: ");
            foreach (byte b in data)
            {
                Console.Write($"0x{b:X2} ");
            }
            Console.WriteLine();
        }
    }
}