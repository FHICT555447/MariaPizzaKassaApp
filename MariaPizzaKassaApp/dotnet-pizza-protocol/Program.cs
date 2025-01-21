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
                    p.Size,
                    Ingredients = p.GetIngredients()
                });

            foreach (var group in groupedPizzas)
            {
                var count = group.Count();
                var pizza = group.First();

                // Retrieve added and removed ingredients for the current group
                var addedIngredients = group.SelectMany(p => order.GetAddedIngredients().ContainsKey(p) ? order.GetAddedIngredients()[p] : new List<Ingredient>()).Distinct().ToList();
                var removedIngredients = group.SelectMany(p => order.GetRemovedIngredients().ContainsKey(p) ? order.GetAddedIngredients()[p] : new List<Ingredient>()).Distinct().ToList();

                var modifications = addedIngredients.Select(ingredient => new ExpandedModification(ModificationType.Add, ingredient.Name))
                    .Concat(removedIngredients.Select(ingredient => new ExpandedModification(ModificationType.Remove, ingredient.Name)))
                    .ToList();

                var expanded = new PizzaOrderExpanded((byte)count, pizza.Name, pizza.Size.ToString(), modifications);
                System.Diagnostics.Debug.WriteLine($"PizzaGroup {group}");
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
