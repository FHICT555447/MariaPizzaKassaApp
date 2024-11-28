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
        public int _id { get; private set; }
        public string _name { get; private set; }
        public decimal _price { get; private set; }
        public List<Ingredient> _ingredients { get; private set; }
        public PizzaSize.Size _size { get; private set; }

        //constructor without size
        public Pizza(int id, string name, decimal price, List<Ingredient> ingredients)
        {
            _id = id;
            _name = name;
            _price = price;
            _ingredients = ingredients;
        }

        //constructor with size
        public Pizza(int id, string name, decimal price, List<Ingredient> ingredients, PizzaSize.Size size)
        {
            _id = id;
            _name = name;
            _price = price;
            _ingredients = ingredients;
            _size = size;
        }
    }
}
