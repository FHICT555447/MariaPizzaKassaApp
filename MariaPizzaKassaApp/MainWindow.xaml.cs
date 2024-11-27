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

namespace MarioPizzaKassaApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Order currentOrder;

        public MainWindow()
        {
            InitializeComponent();
            List<Pizza> pizzas = GenerateDummyData();
            CreatePizzaButtons(pizzas);
        }

        private List<Pizza> GenerateDummyData()
        {
            List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient("Tomato Sauce", 0.50m, false),
                new Ingredient("Cheese", 1.00m, false),
                new Ingredient("Pepperoni", 1.50m, false),
                new Ingredient("Mushrooms", 0.75m, false),
                new Ingredient("Onions", 0.50m, false),
                new Ingredient("Sausage", 1.50m, false),
                new Ingredient("Bacon", 1.75m, false),
                new Ingredient("Black Olives", 0.75m, false),
                new Ingredient("Green Peppers", 0.50m, false),
                new Ingredient("Pineapple", 1.00m, false),
                new Ingredient("Spinach", 0.75m, false)
            };

            List<Pizza> pizzas = new List<Pizza>
            {
                new Pizza("Margherita", 8.00m, new List<Ingredient> { ingredients[0], ingredients[1] }),
                new Pizza("Pepperoni", 9.00m, new List<Ingredient> { ingredients[0], ingredients[1], ingredients[2] }),
                new Pizza("BBQ Chicken", 10.00m, new List<Ingredient> { ingredients[0], ingredients[1], ingredients[5] }),
                new Pizza("Hawaiian", 9.50m, new List<Ingredient> { ingredients[0], ingredients[1], ingredients[9] }),
                new Pizza("Veggie", 8.50m, new List<Ingredient> { ingredients[0], ingredients[1], ingredients[3], ingredients[4], ingredients[7], ingredients[8], ingredients[10] }),
                new Pizza("Meat Lovers", 11.00m, new List<Ingredient> { ingredients[0], ingredients[1], ingredients[2], ingredients[5], ingredients[6] }),
                new Pizza("Supreme", 12.00m, new List<Ingredient> { ingredients[0], ingredients[1], ingredients[2], ingredients[3], ingredients[4], ingredients[5], ingredients[6], ingredients[7], ingredients[8] }),
                new Pizza("Buffalo Chicken", 10.50m, new List<Ingredient> { ingredients[0], ingredients[1], ingredients[5] }),
                new Pizza("Four Cheese", 9.00m, new List<Ingredient> { ingredients[0], ingredients[1] }),
                new Pizza("Spinach Alfredo", 9.50m, new List<Ingredient> { ingredients[0], ingredients[1], ingredients[10] })
            };

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

            foreach (var pizza in currentOrder._pizzas)
            {
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OrderDetailsPanel.Children.Clear();
            currentOrder = null;
        }
    }
}
