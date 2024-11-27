﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariaPizzaKassaApp.classes
{
    public class PizzaSize
    {
        //fields
        public enum Size
        {
            Small = 1,
            Medium = 2,
            Large = 3
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