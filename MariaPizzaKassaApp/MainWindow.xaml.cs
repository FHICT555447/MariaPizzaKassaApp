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

                if (!reader.HasRows)
                {
                    Console.WriteLine("No rows found.");
                    return pizzas;
                }

                while (reader.Read())
                {
                    int pizzaID = (int)reader["pizza_id"];
                    int ingredientID = (int)reader["ingredient_id"];
                    string pizzaName = reader["name"].ToString();
                    decimal pizzaPrice = reader.GetDecimal("price");
                    string ingredientName = reader["ingredient_name"].ToString();
                    decimal ingredientPrice = reader.GetDecimal("purchase_price");
                    bool finishingIngredient = reader.GetBoolean("finishing_ingredient");

                    Console.WriteLine($"Pizza: {pizzaName}, Price: {pizzaPrice}, Ingredient: {ingredientName}, Ingredient Price: {ingredientPrice}, Finishing: {finishingIngredient}");

                    List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient(ingredientID, ingredientName, ingredientPrice, finishingIngredient)
            };

                    // Check if the pizza already exists in the list
                    Pizza pizza = pizzas.FirstOrDefault(p => p._name == pizzaName);
                    if (pizza == null)
                    {
                        pizza = new Pizza(pizzaID, pizzaName, pizzaPrice, ingredients);
                        pizzas.Add(pizza);
                    }
                    else
                    {
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
                totalPrice += pizza._price + (decimal)pizza._size; //adding size to price to indicate the correct price in the DB

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
                    Text = $"Name: {pizza._name}",
                    FontSize = 20,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap
                };

                TextBlock pizzaSize = new TextBlock
                {
                    Text = $"Size: {pizza._size}",
                    Margin = new Thickness(5, 0, 0, 0),
                    FontSize = 15,
                    TextWrapping = TextWrapping.Wrap
                };

                TextBlock pizzaPrice = new TextBlock
                {
                    Text = $"Price: {pizza._price + (decimal)pizza._size:C}",
                    Margin = new Thickness(5, 0, 0, 0),
                    FontSize = 15,
                    TextWrapping = TextWrapping.Wrap
                };

                rectContent.Children.Add(pizzaName);
                rectContent.Children.Add(pizzaSize);
                rectContent.Children.Add(pizzaPrice);

                rectContent.MouseEnter += (s, e) => pizzaRect.Fill = Brushes.LightBlue;
                rectContent.MouseLeave += (s, e) => pizzaRect.Fill = Brushes.LightGray;
                rectContent.MouseLeftButtonUp += (s, e) => RemovePizza(pizza);

                Grid grid = new Grid();
                grid.Children.Add(pizzaRect);
                grid.Children.Add(rectContent);

                OrderDetailsPanel.Children.Add(grid);
            }

            totalAmount.Text = $"Total: {totalPrice:C}";
            pizzaCount.Text = $"Pizza Amount: {totalPizzaAmount}";
        }

        private void RemovePizza(Pizza pizza)
        {
            if (currentOrder != null)
            {
                currentOrder._pizzas.Remove(pizza);
                UpdateOrderDetailsPanel();
            }
        }

        private void CompleteOrder(object sender, RoutedEventArgs e)
        {
            if (currentOrder == null || !currentOrder._pizzas.Any())
            {
                MessageBox.Show("No pizzas in the order to complete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string connectionString = "server=localhost;user=root;database=MarioPizzaTestDB;port=3306;password=";
            string insertOrderQuery = "INSERT INTO orders (total, date) VALUES (@total_price, @order_date)";
            string insertOrderPizzaQuery = "INSERT INTO orders_pizzas (pizzaID, orderID) VALUES (@pizza_id, @order_id)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    MySqlCommand cmd = new MySqlCommand(insertOrderQuery, conn, transaction);
                    cmd.Parameters.AddWithValue("@total_price", totalPrice);
                    cmd.Parameters.AddWithValue("@order_date", currentOrder._date);
                    cmd.ExecuteNonQuery();

                    long orderId = cmd.LastInsertedId;

                    foreach (var pizza in currentOrder._pizzas)
                    {
                        MySqlCommand pizzaCmd = new MySqlCommand(insertOrderPizzaQuery, conn, transaction);
                        pizzaCmd.Parameters.AddWithValue("@order_id", orderId);
                        pizzaCmd.Parameters.AddWithValue("@pizza_id", pizza._id + pizza._size); //adding size number to pizza id to indicate the correct pizza id in the DB
                        pizzaCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Order completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    OrderDetailsPanel.Children.Clear();
                    currentOrder = null;
                    totalPrice = 0;
                    totalAmount.Text = $"Total: €0,-";
                    totalPizzaAmount = 0;
                    pizzaCount.Text = $"Pizza Amount: 0";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"An error occurred while completing the order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
