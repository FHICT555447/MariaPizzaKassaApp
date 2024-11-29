using MarioPizzaKassaApp.classes;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using System;

namespace MarioPizzaKassaApp
{
    public partial class PizzaDetailsWindow : Window
    {
        private Pizza _pizza;
        public Pizza SelectedPizza;
        private PizzaSize.Size selectedSize;

        public PizzaDetailsWindow(Pizza pizza)
        {
            InitializeComponent();
            _pizza = pizza;
            selectedSize = PizzaSize.Size.Small;
            PizzaNameTextBlock.Text = $"Pizza: {pizza._name}";
            PizzaPriceTextBlock.Text = $"Price: {pizza._price:C}";
            PizzaIngredientsTextBlock.Text = $"Ingredients: {string.Join(", ", pizza._ingredients.Select(i => i._name))}";

            foreach (var size in Enum.GetValues(typeof(PizzaSize.Size)))
            {
                string sizeText = size.ToString();
                string additionalCostText = "";

                switch (size)
                {
                    case PizzaSize.Size.Medium:
                        additionalCostText = " + €1,-";
                        break;
                    case PizzaSize.Size.Large:
                        additionalCostText = " + €2,-";
                        break;
                    case PizzaSize.Size.ExtraLarge:
                        additionalCostText = " + €3,-";
                        break;
                }

                Button sizeButton = new Button
                {
                    Content = sizeText + additionalCostText,
                    Width = 105,
                    Margin = new Thickness(5)
                };
                sizeButton.Click += SizeButton_Click;
                SizeButtonsPanel.Children.Add(sizeButton);
            }
        }

        private void SizeButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            string selectedSizeText = clickedButton.Content.ToString().Split(' ')[0]; // Get the size text before the additional cost

            PizzaSize.Size size = PizzaSize.Size.Small; // Default to Small
            switch (selectedSizeText)
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
                case "ExtraLarge":
                    size = PizzaSize.Size.ExtraLarge;
                    break;
            }

            selectedSize = size; // Update the selected size

            SelectedPizza = new Pizza(_pizza._id, _pizza._name, _pizza._price, _pizza._ingredients, size);
            this.DialogResult = true;
            this.Close();
        }
    }
}