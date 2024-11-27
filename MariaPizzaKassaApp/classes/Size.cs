using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariaPizzaKassaApp.classes
{
    internal class Size
    {
        //fields
        public enum PizzaSize
        {
            Small = 1,
            Medium = 2,
            Large = 3
        }

        //properties
        public PizzaSize SizeType { get; private set; }

        //constructor
        public Size(PizzaSize size)
        {
            SizeType = size;
        }
    }
}
