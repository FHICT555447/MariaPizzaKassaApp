using MariaPizzaKassaApp.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MarioPizzaKassaApp.classes
{
    internal class Sender
    {
        private OrderStorage _orderStorage;

        public Sender()
        {
            _orderStorage = new OrderStorage();
        }

        public bool OrderToDatabase(Order order)
        {
            if (_orderStorage.SaveOrderToDatabase(order, "/api/order/create"))
            {
                MessageBox.Show("Order completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            else
            {
                MessageBox.Show("Order failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
