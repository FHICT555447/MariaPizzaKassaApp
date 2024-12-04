using MarioPizzaKassaApp.classes;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using System;
using System.Collections.Generic;

namespace MarioPizzaKassaApp
{
    public partial class PizzaDetailsWindow : Window
    {
        private Pizza _pizza;
        public Pizza SelectedPizza;
        private PizzaSize.Size selectedSize;
        private IReadOnlyList<Ingredient> _allIngredients;

        public List<Ingredient> AddedIngredients { get; private set; } = new List<Ingredient>();
        public List<Ingredient> RemovedIngredients { get; private set; } = new List<Ingredient>();

        public PizzaDetailsWindow(Pizza pizza, IReadOnlyList<Ingredient> allIngredients)
        {
            InitializeComponent();
            _pizza = pizza;
            selectedSize = PizzaSize.Size.Small;
            _allIngredients = allIngredients;

            PizzaNameTextBlock.Text = $"Pizza: {pizza.Name}";
            PizzaPriceTextBlock.Text = $"Price: {pizza.Price:C}";

            foreach (var ingredient in _allIngredients)
            {
                CheckBox ingredientCheckBox = new CheckBox
                {
                    Width = 100,
                    Content = ingredient.Name,
                    Margin = new Thickness(5),
                    IsChecked = _pizza.GetIngredients().Any(pizzaIngredient => pizzaIngredient.ID == ingredient.ID),
                    Tag = ingredient,
                };
                ingredientCheckBox.Checked += IngredientCheckBox_Checked;
                ingredientCheckBox.Unchecked += IngredientCheckBox_Unchecked;
                PizzaIngredientsPanel.Children.Add(ingredientCheckBox);
            }

            foreach (var size in Enum.GetValues(typeof(PizzaSize.Size)))
            {
                string sizeText = size.ToString();

                switch (size)
                {
                    case PizzaSize.Size.Medium:
                        break;
                    case PizzaSize.Size.Large:
                        break;
                    case PizzaSize.Size.ExtraLarge:
                        break;
                }

                Button sizeButton = new Button
                {
                    Content = sizeText,
                    Width = 90,
                    Height = 30,
                    Margin = new Thickness(5)
                };
                sizeButton.Click += SizeButton_Click;
                SizeButtonsPanel.Children.Add(sizeButton);
            }
        }

        private void IngredientCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            Ingredient ingredient = checkBox.Tag as Ingredient;

            // Check if the ingredient is part of the pizza
            if (_pizza.GetIngredients().Contains(ingredient))
            {
                // If the ingredient is already part of the pizza, it should not be added to AddedIngredients
                RemovedIngredients.Remove(ingredient); // In case it was previously marked for removal
            }
            else
            {
                AddedIngredients.Add(ingredient);
            }
        }

        private void IngredientCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            Ingredient ingredient = checkBox.Tag as Ingredient;

            Console.WriteLine(_pizza.GetIngredients());
            Console.WriteLine(ingredient);

            // Check if the ingredient is part of the pizza
            if (_pizza.GetIngredients().Contains(ingredient))
            {
                RemovedIngredients.Add(ingredient);
            }
            else
            {
                // If the ingredient is not part of the pizza, it should not be added to RemovedIngredients
                AddedIngredients.Remove(ingredient); // In case it was previously marked for addition
            }
        }

        private void SizeButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            string selectedSizeText = clickedButton.Content.ToString().Split(' ')[0]; // Get the size text

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

            selectedSize = size;

            List<Ingredient> selectedIngredients = new List<Ingredient>();
            foreach (var child in PizzaIngredientsPanel.Children)
            {
                if (child is CheckBox checkBox && checkBox.IsChecked == true)
                {
                    if (checkBox.Tag is Ingredient ingredient)
                    {
                        selectedIngredients.Add(ingredient);
                    }
                }
            }

            SelectedPizza = new Pizza(_pizza.ID, _pizza.Name, _pizza.Price, selectedIngredients, size);
            this.DialogResult = true;
            this.Close();
        }
    }
}