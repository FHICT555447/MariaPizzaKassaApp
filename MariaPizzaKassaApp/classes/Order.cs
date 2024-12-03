using MarioPizzaKassaApp.classes;
using System.Collections.Generic;
using System;
using MariaPizzaKassaApp.classes;

public class Order
{
    public DateTime OrderDate { get; private set; }
    public List<Pizza> Pizzas { get; private set; }
    public Dictionary<Pizza, List<Ingredient>> AddedIngredients { get; private set; }
    public Dictionary<Pizza, List<Ingredient>> RemovedIngredients { get; private set; }
    public Customer OrderCustomer { get; private set; }

    public Order(DateTime orderDate, List<Pizza> pizzas)
    {
        OrderDate = orderDate;
        Pizzas = pizzas;
        AddedIngredients = new Dictionary<Pizza, List<Ingredient>>();
        RemovedIngredients = new Dictionary<Pizza, List<Ingredient>>();
    }

    public void AddPizza(Pizza pizza, List<Ingredient> addedIngredients, List<Ingredient> removedIngredients)
    {
        Pizzas.Add(pizza);
        AddedIngredients[pizza] = addedIngredients;
        RemovedIngredients[pizza] = removedIngredients;
    }

    public void RemovePizza(Pizza pizza)
    {
        if (Pizzas.Contains(pizza))
        {
            Pizzas.Remove(pizza);
            AddedIngredients.Remove(pizza);
            RemovedIngredients.Remove(pizza);
        }
    }

    public void AddCustomer(Customer customer)
    {
        if (OrderCustomer == null)
        {
            OrderCustomer = customer;
        }
    }

    public void RemoveCustomer(Customer customer)
    {
        if (OrderCustomer == customer)
        {
            OrderCustomer = null;
        }
    }
}