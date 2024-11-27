using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariaPizzaKassaApp.classes
{
    internal class Pizza
    {
        //fields & properties
        public string _name { get; private set; }
        public decimal _price { get; private set; }
        public List<Ingredient> _ingredients { get; private set; }

        //constructor
        public Pizza(string name, decimal price, List<Ingredient> ingredients)
        {
            _name = name;
            _price = price;
            _ingredients = ingredients;
        }
    }
}
