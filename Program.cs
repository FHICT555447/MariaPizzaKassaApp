namespace dotnet_pizza_protocol {
    class Program
    {
        static void Main(string[] args)
        {
            UdpSender sender = new("192.168.68.242", 8888);

            var expanded = new PizzaOrderExpanded(2, "Pepperoni", "Extra Large", [
                new ExpandedModification(ModificationType.Add, "Mushrooms"),
                new ExpandedModification(ModificationType.Add, "Jalapenos"),
                new ExpandedModification(ModificationType.Remove, "Pepperoni")
            ]);
            // var minimized = new PizzaOrderMinimized(11, 7);

            sender.Send(expanded.Serialize());
            // sender.Send(minimized.Serialize());

            using (var udpListener = new UdpReceiver(9999))
            {
                udpListener.OnDataReceived += ProcessReceivedData;

                Console.WriteLine("UDP Listener started. Press Enter to stop...");
                Console.ReadLine();
            }
            Console.WriteLine("UDP Listener stopped.");
        }

        static void ProcessReceivedData(byte[] bytes)
        {
            PizzaMessage m = PizzaMessage.Deserialize(bytes);

            Console.WriteLine(m.GetType());
            
            Thread.Sleep(1000);

            UdpSender sender = new("192.168.68.242", 8888);

            var maybeBytes = m.Serialize();
            if (maybeBytes is not null) {
                sender.Send(maybeBytes);
            }
        }
    }
}
