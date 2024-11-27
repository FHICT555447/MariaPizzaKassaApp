using MarioPizzaKassaApp.classes;
using System.Windows.Controls;
using System.Windows;
using System.Linq;

namespace MarioPizzaKassaApp
{
    public partial class PizzaDetailsWindow : Window
    {
        private Pizza _pizza;
        public Pizza SelectedPizza { get; private set; }

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

            PizzaSize.Size size = PizzaSize.Size.Small;
            switch (selectedSize)
            {
                case "Small":
                    size = PizzaSize.Size.Small;
                    break;
                case "Medium":
                    size = PizzaSize.Size.Medium;
                    break;
                case "Large":
                    size = PizzaSize.Size.Large;
                    break;
            }

            SelectedPizza = new Pizza(_pizza._name, _pizza._price, _pizza._ingredients, size);
            this.DialogResult = true;
            this.Close();
        }
    }
}