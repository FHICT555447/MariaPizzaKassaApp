using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariaPizzaKassaApp.classes
{
    internal class Order
    {
        //fields & properties
        public int _id { get; private set; }
        public DateTime _date { get; private set; }
        public List<Pizza> _pizzas { get; private set; }

        //constructor
        public Order(int id, DateTime date, List<Pizza> pizzas)
        {
            _id = id;
            _date = date;
            _pizzas = pizzas;
        }
    }
}
