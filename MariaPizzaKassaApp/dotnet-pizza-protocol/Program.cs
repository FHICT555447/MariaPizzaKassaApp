using MarioPizzaKassaApp.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace dotnet_pizza_protocol
{
    public class Program
    {
        public static void MainFunction(Order order)
        {
            UdpSender sender = new("192.168.68.242", 8888);

            // Group pizzas by their properties including modifications
            var groupedPizzas = order.GetPizzas()
                .GroupBy(p => new
                {
                    p.ID,
                    Added = order.AddedIngredients[p],
                    Removed = order.RemovedIngredients[p]
                });

            foreach (var group in groupedPizzas)
            {
                var count = group.Count();
                var pizza = group.First();
                var addedIngredients = order.AddedIngredients[pizza].Select(i => new ExpandedModification(ModificationType.Add, i.Name)).ToList();
                var removedIngredients = order.RemovedIngredients[pizza].Select(i => new ExpandedModification(ModificationType.Remove, i.Name)).ToList();
                var modifications = addedIngredients.Concat(removedIngredients).ToList();

                var expanded = new PizzaOrderExpanded((byte)count, pizza.Name, pizza.Size.ToString(), modifications);
                sender.Send(expanded.Serialize());
            }

            //using (var udpListener = new UdpReceiver(9999))
            //{
            //    udpListener.OnDataReceived += ProcessReceivedData;

            //    Console.WriteLine("UDP Listener started. Press Enter to stop...");
            //    Console.ReadLine();
            //}
            //Console.WriteLine("UDP Listener stopped.");
        }

        static void ProcessReceivedData(byte[] bytes)
        {
            PizzaMessage m = PizzaMessage.Deserialize(bytes);

            Console.WriteLine(m.ToString());

            Thread.Sleep(1000);

            UdpSender sender = new("192.168.68.242", 8888);

            var maybeBytes = m.Serialize();
            if (maybeBytes is not null)
            {
                sender.Send(maybeBytes);
            }
        }
    }
}
