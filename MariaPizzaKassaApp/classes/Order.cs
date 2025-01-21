using MarioPizzaKassaApp.classes;
using System.Collections.Generic;
using System;
using MariaPizzaKassaApp.classes;

public class Order
{
    private List<Pizza> Pizzas { get; set; }
    private Dictionary<Pizza, List<Ingredient>> AddedIngredients { get; set; }
    private Dictionary<Pizza, List<Ingredient>> RemovedIngredients { get; set; }
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

    public IReadOnlyDictionary<Pizza, List<Ingredient>> GetAddedIngredients()
    {
        return AddedIngredients;
    }

    public IReadOnlyDictionary<Pizza, List<Ingredient>> GetRemovedIngredients()
    {
        return RemovedIngredients;
    }

    public void AddPizza(Pizza pizza, List<Ingredient> addedIngredients, List<Ingredient> removedIngredients)
    {
        if (pizza == null)
        {
            throw new ArgumentException("Pizza cannot be null", nameof(pizza));
        }
        Pizzas.Add(pizza);
        AddedIngredients[pizza] = addedIngredients;
        RemovedIngredients[pizza] = removedIngredients;
    }

    public void RemovePizza(Pizza pizza)
    {
        if (pizza == null)
        {
            throw new ArgumentException("Pizza cannot be null", nameof(pizza));
        }
        if (!Pizzas.Contains(pizza))
        {
            throw new ArgumentException("Pizza doesn't exist in the order", nameof(pizza));
        }
        Pizzas.Remove(pizza);
        AddedIngredients.Remove(pizza);
        RemovedIngredients.Remove(pizza);
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
}