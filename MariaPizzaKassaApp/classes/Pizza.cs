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

        //constructor without size
        public Pizza(int id, string name, decimal price, List<Ingredient> ingredients)
        {
            ID = id;
            Name = name;
            Price = price;
            Ingredients = ingredients;
        }

        //constructor with size
        public Pizza(int id, string name, decimal price, List<Ingredient> ingredients, PizzaSize.Size size)
        {
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
