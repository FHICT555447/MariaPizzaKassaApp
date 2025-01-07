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
using dotnet_pizza_protocol;
using MariaPizzaKassaApp.classes;
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
        private Sender dataSender;
        private int totalPizzaAmount;
        private string[] PizzaButtonColors = { "#FFCCCB", "#FFFFE0" };
        private int PizzaButtonColorIndex = 0;

        public MainWindow()
        {
            dataSender = new Sender();
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

                    pizza.AddIngredient(ingredient);
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
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(PizzaButtonColors[PizzaButtonColorIndex % 2])),
                    Margin = new Thickness(38, 30, 0, 38),
                    Height = 150,
                    Width = 250
                };
                button.Click += (sender, e) => ShowPizzaDetails(pizza);
                PizzaButtonsPanel.Children.Add(button);
                PizzaButtonColorIndex++;
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
                currentOrder = new Order();
            }

            currentOrder.AddPizza(pizza, addedIngredients, removedIngredients);
            UpdateOrderDetailsPanel(); // Method to update the UI with the current order details
        }

        private void UpdateOrderDetailsPanel()
        {
            OrderDetailsPanel.Children.Clear();

            totalPizzaAmount = currentOrder.GetPizzas().Count;

            foreach (var pizza in currentOrder.GetPizzas())
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
                if(currentOrder.GetPizzas().Count == 0)
                {
                    currentOrder = null;
                }
            }
        }

        private void CompleteOrder(object sender, RoutedEventArgs e)
        {
            if (currentOrder == null)
            {
                MessageBox.Show("No pizzas in the order to complete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (dataSender.OrderToDatabase(currentOrder))
            {
                OrderDetailsPanel.Children.Clear();
                currentOrder = null;
                totalAmount.Text = $"Total: €0,-";
                totalPizzaAmount = 0;
                pizzaCount.Text = $"Pizza Amount: 0";
            }
        }
    }
}
