using MarioPizzaKassaApp.classes;
using System.Collections.Generic;
using System;
using MariaPizzaKassaApp.classes;

public class Order
{
    private List<Pizza> Pizzas { get; set; }
    public Dictionary<Pizza, List<Ingredient>> AddedIngredients { get; private set; }
    public Dictionary<Pizza, List<Ingredient>> RemovedIngredients { get; private set; }
    public Customer OrderCustomer { get; private set; }

    public Order()
    {
        Pizzas = new List<Pizza>();
        AddedIngredients = new Dictionary<Pizza, List<Ingredient>>();
        RemovedIngredients = new Dictionary<Pizza, List<Ingredient>>();
    }

    public IReadOnlyList<Pizza> GetPizzas()
    {
        return Pizzas;
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

    public decimal GetTotalPrice()
    {
        decimal totalPrice = 0;
        foreach (Pizza pizza in Pizzas)
        {
            totalPrice += pizza.Price + (decimal)pizza.Size;
        }
        return totalPrice;
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