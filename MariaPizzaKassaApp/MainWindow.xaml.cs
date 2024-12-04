using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MarioPizzaKassaApp.classes;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace MarioPizzaKassaApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Order currentOrder;
        private int totalPizzaAmount;

        public MainWindow()
        {
            InitializeComponent();
            List<Pizza> pizzas = GetPizzasFromDatabase();
            CreatePizzaButtons(pizzas);
        }

        private IConfigurationRoot LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            return builder.Build();
        }

        public List<Pizza> GetPizzasFromDatabase()
        {
            List<Pizza> pizzas = new List<Pizza>();
            Dictionary<int, Pizza> pizzaDictionary = new Dictionary<int, Pizza>();

            IConfigurationRoot configuration = LoadConfiguration();
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            string query = "SELECT p.id as pizza_id, p.name, p.price, i.id as ingredient_id, i.name as ingredient_name, i.purchase_price, i.finishing_ingredient " +
                           "FROM pizzas p " +
                           "JOIN pizzas_ingredients pi ON p.id = pi.pizzaID " +
                           "JOIN ingredients i ON pi.ingredientID = i.id " +
                           "WHERE p.sizeID = 1";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int pizzaID = (int)reader["pizza_id"];
                    int ingredientID = (int)reader["ingredient_id"];
                    string pizzaName = reader["name"].ToString();
                    decimal pizzaPrice = reader.GetDecimal("price");
                    string ingredientName = reader["ingredient_name"].ToString();
                    decimal ingredientPrice = reader.GetDecimal("purchase_price");
                    bool finishingIngredient = reader.GetBoolean("finishing_ingredient");

                    Ingredient ingredient = new Ingredient(ingredientID, ingredientName, ingredientPrice, finishingIngredient);

                    if (!pizzaDictionary.TryGetValue(pizzaID, out Pizza pizza))
                    {
                        pizza = new Pizza(pizzaID, pizzaName, pizzaPrice, new List<Ingredient>());
                        pizzaDictionary[pizzaID] = pizza;
                        pizzas.Add(pizza);
                    }

                    pizza.Ingredients.Add(ingredient);
                }
            }

            return pizzas;
        }

        private void CreatePizzaButtons(List<Pizza> pizzas)
        {
            foreach (var pizza in pizzas)
            {
                Button button = new Button
                {
                    Content = pizza.Name,
                    Margin = new Thickness(38, 30, 0, 38),
                    Height = 150,
                    Width = 250
                };
                button.Click += (sender, e) => ShowPizzaDetails(pizza);
                PizzaButtonsPanel.Children.Add(button);
            }
        }

        private void ShowPizzaDetails(Pizza pizza)
        {
            IConfigurationRoot configuration = LoadConfiguration();
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            List<Ingredient> allIngredients = Ingredient.GetAllIngredients(connectionString);
            PizzaDetailsWindow detailsWindow = new PizzaDetailsWindow(pizza, allIngredients);
            if (detailsWindow.ShowDialog() == true)
            {
                AddPizzaToOrder(detailsWindow.SelectedPizza, detailsWindow.AddedIngredients, detailsWindow.RemovedIngredients);
            }
        }


        private void AddPizzaToOrder(Pizza pizza, List<Ingredient> addedIngredients, List<Ingredient> removedIngredients)
        {
            if (currentOrder == null)
            {
                currentOrder = new Order(DateTime.Now, new List<Pizza>());
            }

            currentOrder.AddPizza(pizza, addedIngredients, removedIngredients);
            UpdateOrderDetailsPanel(); // Method to update the UI with the current order details
        }

        private void UpdateOrderDetailsPanel()
        {
            OrderDetailsPanel.Children.Clear();

            totalPizzaAmount = currentOrder.Pizzas.Count;

            foreach (var pizza in currentOrder.Pizzas)
            {
                Rectangle pizzaRect = new Rectangle
                {
                    Margin = new Thickness(2),
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = Brushes.LightGray,
                };

                pizzaRect.MouseEnter += (s, e) => pizzaRect.Fill = Brushes.LightBlue;
                pizzaRect.MouseLeave += (s, e) => pizzaRect.Fill = Brushes.LightGray;
                pizzaRect.MouseLeftButtonUp += (s, e) => RemovePizza(pizza);

                StackPanel rectContent = new StackPanel
                {
                    Margin = new Thickness(2)
                };

                TextBlock pizzaName = new TextBlock
                {
                    Text = $"Name: {pizza.Name}",
                    FontSize = 20,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap
                };

                TextBlock pizzaSize = new TextBlock
                {
                    Text = $"Size: {pizza.Size}",
                    Margin = new Thickness(5, 0, 0, 0),
                    FontSize = 15,
                    TextWrapping = TextWrapping.Wrap
                };

                TextBlock pizzaPrice = new TextBlock
                {
                    Text = $"Price: {(pizza.Price + (decimal)pizza.Size):C}",
                    Margin = new Thickness(5, 0, 0, 0),
                    FontSize = 15,
                    TextWrapping = TextWrapping.Wrap
                };

                TextBlock pizzaModification = new TextBlock
                {
                    Text = "Modifications:",
                    Margin = new Thickness(5, 0, 0, 0),
                    FontSize = 15,
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap
                };

                // Create a string for modifications
                StringBuilder modifications = new StringBuilder();

                // Check for added ingredients
                if (currentOrder.AddedIngredients != null && currentOrder.AddedIngredients.ContainsKey(pizza))
                {
                    foreach (var ingredient in currentOrder.AddedIngredients[pizza])
                    {
                        modifications.Append($"{ingredient.Name} +, ");
                    }
                }

                // Check for removed ingredients
                if (currentOrder.RemovedIngredients != null && currentOrder.RemovedIngredients.ContainsKey(pizza))
                {
                    foreach (var ingredient in currentOrder.RemovedIngredients[pizza])
                    {
                        modifications.Append($"{ingredient.Name} -, ");
                    }
                }

                // Remove trailing comma and space if there are modifications
                if (modifications.Length > 0)
                {
                    modifications.Length -= 2;
                }

                TextBlock modificationDetails = new TextBlock
                {
                    Text = modifications.ToString(),
                    Margin = new Thickness(10, 0, 0, 0),
                    FontSize = 15,
                    TextWrapping = TextWrapping.Wrap
                };

                rectContent.Children.Add(pizzaName);
                rectContent.Children.Add(pizzaSize);
                rectContent.Children.Add(pizzaPrice);
                rectContent.Children.Add(pizzaModification);
                rectContent.Children.Add(modificationDetails);

                rectContent.MouseEnter += (s, e) => pizzaRect.Fill = Brushes.LightBlue;
                rectContent.MouseLeave += (s, e) => pizzaRect.Fill = Brushes.LightGray;
                rectContent.MouseLeftButtonUp += (s, e) => RemovePizza(pizza);

                Grid grid = new Grid();
                grid.Children.Add(pizzaRect);
                grid.Children.Add(rectContent);

                OrderDetailsPanel.Children.Add(grid);
            }

            totalAmount.Text = $"Total: {currentOrder.GetTotalPrice():C}";
            pizzaCount.Text = $"Pizza Amount: {totalPizzaAmount}";
        }

        private void RemovePizza(Pizza pizza)
        {
            if (currentOrder != null)
            {
                currentOrder.RemovePizza(pizza);
                UpdateOrderDetailsPanel();
            }
        }

        private void CompleteOrder(object sender, RoutedEventArgs e)
        {

            if (currentOrder == null) return;

            SaveOrderToDatabase(currentOrder);
            SaveModificationsToDatabase(currentOrder);

            OrderDetailsPanel.Children.Clear();
            currentOrder = null;
            totalAmount.Text = $"Total: €0,-";
            totalPizzaAmount = 0;
            pizzaCount.Text = $"Pizza Amount: 0";
        }

        private void SaveOrderToDatabase(Order order)
        {
            IConfigurationRoot configuration = LoadConfiguration();
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            string insertOrderQuery = "INSERT INTO orders (total, date) VALUES (@total_price, @order_date)";
            string insertOrderPizzaQuery = "INSERT INTO orders_pizzas (pizzaID, orderID) VALUES (@pizza_id, @order_id)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    MySqlCommand cmd = new MySqlCommand(insertOrderQuery, conn, transaction);
                    cmd.Parameters.AddWithValue("@total_price", currentOrder.GetTotalPrice());
                    cmd.Parameters.AddWithValue("@order_date", currentOrder.OrderDate);
                    cmd.ExecuteNonQuery();

                    long orderId = cmd.LastInsertedId;

                    foreach (var pizza in currentOrder.Pizzas)
                    {
                        MySqlCommand pizzaCmd = new MySqlCommand(insertOrderPizzaQuery, conn, transaction);
                        pizzaCmd.Parameters.AddWithValue("@order_id", orderId);
                        pizzaCmd.Parameters.AddWithValue("@pizza_id", pizza.ID + pizza.Size); //adding size number to pizza id to indicate the correct pizza id in the DB
                        pizzaCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Order completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"An error occurred while completing the order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveModificationsToDatabase(Order order)
        {
            IConfigurationRoot configuration = LoadConfiguration();
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Get the last inserted order_pizzaID from the orders_pizzas table
                string lastInsertedIDQuery = "SELECT LAST_INSERT_ID()";
                int lastInsertedOrderPizzaID = 0;

                using (MySqlCommand cmd = new MySqlCommand(lastInsertedIDQuery, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out lastInsertedOrderPizzaID))
                    {
                        Console.WriteLine($"Last inserted OrderPizzaID: {lastInsertedOrderPizzaID}");
                    }
                }

                foreach (var pizza in order.Pizzas)
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
            }
        }
    }
}
