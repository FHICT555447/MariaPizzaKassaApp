using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariaPizzaKassaApp.classes
{
    public class Ingredient
    {
        //fields & properties
        public string _name {  get; private set; }
        public decimal _purchasePrice { get; private set; }
        public bool _ifFinishingIngredient { get; private set; }

        //constructor
        public Ingredient(string name, decimal purchasePrice, bool ifFinishingIngredient)
        {
            _name = name;
            _purchasePrice = purchasePrice;
            _ifFinishingIngredient = ifFinishingIngredient;
        }
    }
}
