namespace dotnet_pizza_protocol {
    class Program
    {
        static void Main(string[] args)
        {
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
            PizzaMessage m = PizzaMessage.Receive(bytes);

            Console.WriteLine(m.GetType());
        }
    }
}
