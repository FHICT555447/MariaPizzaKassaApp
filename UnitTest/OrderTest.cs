using MarioPizzaKassaApp.classes;
using MySql.Data.MySqlClient;

namespace UnitTest;

public class OrderTest
{
    [Fact]
    public void AddPizzaShouldNotThrowWhenCorrect()
    {
        //Arrange
        List<Ingredient> ingredients = new List<Ingredient>
                {
                    new Ingredient(1, "Tomato", 1.0m, true),
                    new Ingredient(2, "Cheese", 1.5m, true),
                    new Ingredient(3, "Pineapple", 0.5m, true)
                };
        Pizza pizza = new Pizza(1, "Margherita", 10.0m, ingredients, PizzaSize.Size.Medium);
        Order order = new Order();
        //Act
        order.AddPizza(pizza, new List<Ingredient>(), new List<Ingredient>());

        //Assert
        Assert.NotEmpty(order.GetPizzas());
    }

    [Fact]
    public void AddPizzaShouldThrowWhenPizzaIsNull()
    {
        //Arrange
        Order order = new Order();
        //Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => order.AddPizza(null, new List<Ingredient>(), new List<Ingredient>()));
        Assert.Equal("Pizza cannot be null (Parameter 'pizza')", exception.Message);
    }

    [Fact]
    public void RemovePizzaShouldNotThrowWhenCorrect()
    {
        //Arrange
        List<Ingredient> ingredients = new List<Ingredient>
                {
                    new Ingredient(1, "Tomato", 1.0m, true),
                    new Ingredient(2, "Cheese", 1.5m, true),
                    new Ingredient(3, "Pineapple", 0.5m, true)
                };
        Pizza pizza = new Pizza(1, "Margherita", 10.0m, ingredients, PizzaSize.Size.Medium);
        Order order = new Order();
        order.AddPizza(pizza, new List<Ingredient>(), new List<Ingredient>());
        //Act
        order.RemovePizza(pizza);
        //Assert
        Assert.Empty(order.GetPizzas());
    }

    [Fact]
    public void RemovePizzaShouldThrowWhenPizzaIsIncorrect()
    {
        //Arrange
        List<Ingredient> ingredients = new List<Ingredient>
                {
                    new Ingredient(1, "Tomato", 1.0m, true),
                    new Ingredient(2, "Cheese", 1.5m, true),
                    new Ingredient(3, "Pineapple", 0.5m, true)
                };
        Pizza pizza = new Pizza(1, "Margherita", 10.0m, ingredients, PizzaSize.Size.Medium);
        Order order = new Order();
        order.AddPizza(pizza, new List<Ingredient>(), new List<Ingredient>());
        //Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => order.RemovePizza(new Pizza(2, "Hawaii", 12.0m, ingredients, PizzaSize.Size.Medium)));
        Assert.Equal("Pizza doesn't exist in the order (Parameter 'pizza')", exception.Message);
    }

    [Fact]
    public void GetTotalPriceShouldReturnTotalPrice()
    {
        //Arrange
        var order = new Order();
        var ingredients = new List<Ingredient>
        {
            new Ingredient(1, "Tomato", 1.0m, true),
            new Ingredient(2, "Cheese", 1.5m, true),
            new Ingredient(3, "Pineapple", 0.5m, true)
        };
        var pizzas = new List<Pizza>
        {
            new Pizza(1, "Margherita", 10.0m, ingredients, PizzaSize.Size.Medium),
            new Pizza(1, "Margherita", 10.0m, ingredients, PizzaSize.Size.ExtraLarge)
        };
        
        foreach (var pizza in pizzas)
        {
            order.AddPizza(pizza, new List<Ingredient>(), new List<Ingredient>());
        }

        //Act
        var totalPrice = order.GetTotalPrice();

        //Assert
        Assert.Equal(24.0m, totalPrice);
    }
}
