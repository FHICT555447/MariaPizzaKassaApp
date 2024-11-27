using MariaPizzaKassaApp.classes;
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
using System.Windows.Shapes;

namespace MariaPizzaKassaApp
{
    /// <summary>
    /// Interaction logic for PizzaDetailsWindow.xaml
    /// </summary>
    public partial class PizzaDetailsWindow : Window
    {
        private Pizza _pizza;

        public PizzaDetailsWindow(Pizza pizza)
        {
            InitializeComponent();
            _pizza = pizza;
            PizzaNameTextBlock.Text = $"Pizza: {pizza._name}";
            PizzaPriceTextBlock.Text = $"Price: {pizza._price:C}";
            PizzaIngredientsTextBlock.Text = $"Ingredients: {string.Join(", ", pizza._ingredients.Select(i => i._name))}";
        }

        private void SizeButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            string selectedSize = clickedButton.Content.ToString();
            MessageBox.Show($"Selected Size: {selectedSize} for {_pizza._name}", "Size Selection");
            // You can add additional logic to handle the size selection
        }
    }
}
