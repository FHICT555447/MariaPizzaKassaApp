using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioPizzaKassaApp.classes
{
    public class Pizza
    {
        //fields & properties
        public int ID { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        private List<Ingredient> Ingredients { get; set; }
        public PizzaSize.Size Size { get; private set; }

        // Constructor without size
        public Pizza(int id, string name, decimal price, List<Ingredient> ingredients)
        {
            if (id == 0)
            {
                throw new ArgumentException("ID cannot be zero.", nameof(id));
            }
            if (price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.", nameof(price));
            }
            ID = id;
            Name = name;
            Price = price;
            Ingredients = ingredients;
        }

        // Constructor with size
        public Pizza(int id, string name, decimal price, List<Ingredient> ingredients, PizzaSize.Size size)
        {
            if (id == 0)
            {
                throw new ArgumentException("ID cannot be zero.", nameof(id));
            }
            if (price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.", nameof(price));
            }
            ID = id;
            Name = name;
            Price = price;
            Ingredients = ingredients;
            Size = size;
        }

        //methods
        public IReadOnlyList<Ingredient> GetIngredients()
        {
            return Ingredients;
        }

        public void AddIngredient(Ingredient ingredient)
        {
            Ingredients.Add(ingredient);
        }
        public void RemoveIngredient(Ingredient ingredient)
        {
            Ingredients.Remove(ingredient);
        }
    }
}
