using System;
using System.Collections.Generic;
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
using MySql.Data.MySqlClient;

namespace MarioPizzaKassaApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Order currentOrder;
        private decimal totalPrice;
        private int totalPizzaAmount;

        public MainWindow()
        {
            InitializeComponent();
            List<Pizza> pizzas = GetPizzasFromDatabase();
            CreatePizzaButtons(pizzas);
        }

        public List<Pizza> GetPizzasFromDatabase()
        {
            List<Pizza> pizzas = new List<Pizza>();

            string connectionString = "server=localhost;user=root;database=MarioPizzaTestDB;port=3306;password=";
            string query = "SELECT p.name, p.price, i.name as ingredient_name, i.purchase_price, i.finishing_ingredient " +
                           "FROM pizzas p " +
                           "JOIN pizzas_ingredients pi ON p.id = pi.pizzaID " +
                           "JOIN ingredients i ON pi.ingredientID = i.id " +
                           "WHERE p.sizeID = 1";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    Console.WriteLine("No rows found.");
                    return pizzas;
                }

                while (reader.Read())
                {
                    string pizzaName = reader["name"].ToString();
                    decimal pizzaPrice = reader.GetDecimal("price");
                    string ingredientName = reader["ingredient_name"].ToString();
                    decimal ingredientPrice = reader.GetDecimal("purchase_price");
                    bool finishingIngredient = reader.GetBoolean("finishing_ingredient");

                    Console.WriteLine($"Pizza: {pizzaName}, Price: {pizzaPrice}, Ingredient: {ingredientName}, Ingredient Price: {ingredientPrice}, Finishing: {finishingIngredient}");

                    List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient(ingredientName, ingredientPrice, finishingIngredient)
            };

                    // Check if the pizza already exists in the list
                    Pizza pizza = pizzas.FirstOrDefault(p => p._name == pizzaName);
                    if (pizza == null)
                    {
                        // Create a new Pizza object if it doesn't exist
                        pizza = new Pizza(pizzaName, pizzaPrice, ingredients);
                        pizzas.Add(pizza);
                    }
                    else
                    {
                        // Add the ingredient to the existing Pizza object
                        pizza._ingredients.AddRange(ingredients);
                    }
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
                    Content = pizza._name,
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
            PizzaDetailsWindow detailsWindow = new PizzaDetailsWindow(pizza);
            if (detailsWindow.ShowDialog() == true)
            {
                AddPizzaToOrder(detailsWindow.SelectedPizza);
            }
        }

        private void AddPizzaToOrder(Pizza pizza)
        {
            if (currentOrder == null)
            {
                currentOrder = new Order(1, DateTime.Now, new List<Pizza>());
            }
            currentOrder.AddPizza(pizza);
            UpdateOrderDetailsPanel();
        }

        private void UpdateOrderDetailsPanel()
        {
            OrderDetailsPanel.Children.Clear();

            totalPrice = 0;
            totalPizzaAmount = currentOrder._pizzas.Count;

            foreach (var pizza in currentOrder._pizzas)
            {
                totalPrice += pizza._price;

                Rectangle pizzaRect = new Rectangle
                {
                    //Height = 80,
                    Margin = new Thickness(2),
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = Brushes.LightGray
                };

                StackPanel rectContent = new StackPanel
                {
                    Margin = new Thickness(2)
                };

                TextBlock pizzaName = new TextBlock
                {
                    Text = $"Name: {pizza._name}",
                    FontSize = 20,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    //Width = 350,
                    TextWrapping = TextWrapping.Wrap
                };

                TextBlock pizzaSize = new TextBlock
                {
                    Text = $"Size: {pizza._size}",
                    Margin = new Thickness(5, 0, 0, 0),
                    FontSize = 15,
                    //Width = 350,
                    TextWrapping = TextWrapping.Wrap
                };

                TextBlock pizzaPrice = new TextBlock
                {
                    Text = $"Price: {pizza._price:C}",
                    Margin = new Thickness(5, 0, 0, 0),
                    FontSize = 15,
                    //Width = 350,
                    TextWrapping = TextWrapping.Wrap
                };

                rectContent.Children.Add(pizzaName);
                rectContent.Children.Add(pizzaSize);
                rectContent.Children.Add(pizzaPrice);

                Grid grid = new Grid();
                grid.Children.Add(pizzaRect);
                grid.Children.Add(rectContent);

                OrderDetailsPanel.Children.Add(grid);
            }

            totalAmount.Text = $"Total: {totalPrice:C}";
            pizzaCount.Text = $"Pizza Amount: {totalPizzaAmount}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OrderDetailsPanel.Children.Clear();
            currentOrder = null;
            totalPrice = 0;
            totalAmount.Text = $"Total: €0,-";
            totalPizzaAmount = 0;
            pizzaCount.Text = $"Pizza Amount: 0";
        }
    }
}
