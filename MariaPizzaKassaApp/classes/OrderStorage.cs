using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;

namespace MariaPizzaKassaApp.classes
{
    public class OrderStorage
    {
        //fields & properties
        private string ConnectionString { get; set; }

        //constructor
        public OrderStorage()
        {
            IConfigurationRoot configuration = LoadConfiguration();
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //methods
        private IConfigurationRoot LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            return builder.Build();
        }

        public bool SaveOrderToDatabase(Order order)
        {
            string insertOrderQuery = "INSERT INTO orders (total, date) VALUES (@total_price, @order_date)";
            string insertOrderPizzaQuery = "INSERT INTO orders_pizzas (pizzaID, orderID) VALUES (@pizza_id, @order_id)";

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                MySqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    MySqlCommand cmd = new MySqlCommand(insertOrderQuery, conn, transaction);
                    cmd.Parameters.AddWithValue("@total_price", order.GetTotalPrice());
                    cmd.Parameters.AddWithValue("@order_date", DateTime.Now);
                    cmd.ExecuteNonQuery();

                    long orderId = cmd.LastInsertedId;

                    foreach (var pizza in order.GetPizzas())
                    {
                        MySqlCommand pizzaCmd = new MySqlCommand(insertOrderPizzaQuery, conn, transaction);
                        pizzaCmd.Parameters.AddWithValue("@order_id", orderId);
                        pizzaCmd.Parameters.AddWithValue("@pizza_id", pizza.ID + pizza.Size); //adding size number to pizza id to indicate the correct pizza id in the DB
                        pizzaCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    conn.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    conn.Close();
                    MessageBox.Show($"An error occurred while completing the order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        public bool SaveModificationsToDatabase(Order order)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                // Get the last inserted order_pizzaID from the orders_pizzas table
                string lastInsertedIDQuery = "SELECT LAST_INSERT_ID()";
                int lastInsertedOrderPizzaID = 0;

                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(lastInsertedIDQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out lastInsertedOrderPizzaID))
                        {
                            Console.WriteLine($"Last inserted OrderPizzaID: {lastInsertedOrderPizzaID}");
                        }
                    }

                    foreach (var pizza in order.GetPizzas())
                    {
                        if (order.AddedIngredients != null && order.AddedIngredients.ContainsKey(pizza) && order.AddedIngredients[pizza] != null && order.AddedIngredients[pizza].Count > 0)
                        {
                            foreach (var ingredient in order.AddedIngredients[pizza])
                            {
                                string query = "INSERT INTO orders_pizza_customizations (order_pizzaID, ingredientID, modification_type) VALUES (@orderPizzaID, @ingredientID, @modificationType)";
                                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                                {
                                    // Use the last inserted order_pizzaID for the current pizza
                                    cmd.Parameters.AddWithValue("@orderPizzaID", lastInsertedOrderPizzaID);
                                    cmd.Parameters.AddWithValue("@ingredientID", ingredient.ID);
                                    cmd.Parameters.AddWithValue("@modificationType", 1); // 1 for addition
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        if (order.RemovedIngredients != null && order.RemovedIngredients.ContainsKey(pizza) && order.RemovedIngredients[pizza] != null && order.RemovedIngredients[pizza].Count > 0)
                        {
                            foreach (var ingredient in order.RemovedIngredients[pizza])
                            {
                                string query = "INSERT INTO orders_pizza_customizations (order_pizzaID, ingredientID, modification_type) VALUES (@orderPizzaID, @ingredientID, @modificationType)";
                                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                                {
                                    // Use the last inserted order_pizzaID for the current pizza
                                    cmd.Parameters.AddWithValue("@orderPizzaID", lastInsertedOrderPizzaID);
                                    cmd.Parameters.AddWithValue("@ingredientID", ingredient.ID);
                                    cmd.Parameters.AddWithValue("@modificationType", 0); // 0 for removal
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    conn.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    MessageBox.Show($"An error occurred while completing the order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }
    }
}
