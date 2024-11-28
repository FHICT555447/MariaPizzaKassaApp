using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioPizzaKassaApp.classes
{
    public class PizzaSize
    {
        //fields
        public enum Size
        {
            Small = 0,
            Medium = 1,
            Large = 2,
            ExtraLarge = 3
        }

        //properties
        public Size SizeType { get; private set; }

        //constructor
        public PizzaSize(Size size)
        {
            SizeType = size;
        }
    }
}
