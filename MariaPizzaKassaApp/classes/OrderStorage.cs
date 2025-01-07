using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;

namespace MariaPizzaKassaApp.classes
{
    public class OrderStorage
    {
        //fields & properties
        private string APIString { get; set; }

        //constructor
        public OrderStorage()
        {
            IConfigurationRoot configuration = LoadConfiguration();
            APIString = configuration.GetConnectionString("APIString");
        }

        //methods
        private IConfigurationRoot LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            return builder.Build();
        }

        public bool SaveOrderToDatabase(Order order, string endpoint)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIString);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var pizzasJson = order.GetPizzas().Select(pizza => new
                    {
                        id = pizza.ID + pizza.Size,
                        name = pizza.Name,
                        price = pizza.Price,
                        ingredients = pizza.GetIngredients().Select(ingredient => new
                        {
                            id = ingredient.ID,
                            name = ingredient.Name,
                            purchasePrice = ingredient.PurchasePrice,
                            isFinishingIngredient = ingredient.IsFinishingIngredient
                        }),
                        size = new
                        {
                            id = (int)pizza.Size,
                            name = pizza.Size.ToString()
                        }
                    });

                    var content = new StringContent(JsonSerializer.Serialize(pizzasJson), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync(endpoint, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show($"An error occurred while sending the order to the API: {response.ReasonPhrase}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                MessageBox.Show($"An exception occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
