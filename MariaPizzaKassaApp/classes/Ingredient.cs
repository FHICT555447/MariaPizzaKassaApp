using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioPizzaKassaApp.classes
{
    public class Ingredient
    {
        //fields & properties
        public int _id { get; private set; }
        public string _name {  get; private set; }
        public decimal _purchasePrice { get; private set; }
        public bool _ifFinishingIngredient { get; private set; }

        //constructor
        public Ingredient(int id, string name, decimal purchasePrice, bool ifFinishingIngredient)
        {
            _id = id;
            _name = name;
            _purchasePrice = purchasePrice;
            _ifFinishingIngredient = ifFinishingIngredient;
        }
    }
}
