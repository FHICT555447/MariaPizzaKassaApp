using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariaPizzaKassaApp.classes
{
    public class Customer
    {
        //fields & properties
        public int ID { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public int Points { get; private set; }

        //constructor
        public Customer(int id, string firstName, string lastName, string email, int points)
        {
            ID = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Points = points;
        }

        //methods
        public void AddPoints(int points)
        {
            Points += points;
        }

        public void RemovePoints(int points)
        {
            Points -= points;
        }
    }
}
