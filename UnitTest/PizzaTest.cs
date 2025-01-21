namespace UnitTest;
using MarioPizzaKassaApp.classes;

public class PizzaTest
{
    [Fact]
    public void ConstructorWithoutSizeShouldInitializePropertiesCorrectly()
    {
        //Arrange
        List<Ingredient> ingredients = new List<Ingredient>
        {
            new Ingredient(1, "Tomato", 1.0m, true),
            new Ingredient(2, "Cheese", 1.5m, true),
            new Ingredient(3, "Pineapple", 0.5m, true)
        };
        //Act
        Pizza pizza = new Pizza(1, "Margherita", 10.0m, ingredients);
        //Assert
        Assert.Equal(1, pizza.ID);
        Assert.Equal("Margherita", pizza.Name);
        Assert.Equal(10.0m, pizza.Price);
    }

    [Fact]
    public void ConstructorWithSizeShouldInitializePropertiesCorrectly()
    {
        //Arrange
        List<Ingredient> ingredients = new List<Ingredient>
        {
            new Ingredient(1, "Tomato", 1.0m, true),
            new Ingredient(2, "Cheese", 1.5m, true),
            new Ingredient(3, "Pineapple", 0.5m, true)
        };

        //Act
        Pizza pizza = new Pizza(1, "Margherita", 10.0m, ingredients, PizzaSize.Size.Medium);

        //Assert
        Assert.Equal(1, pizza.ID);
        Assert.Equal("Margherita", pizza.Name);
        Assert.Equal(10.0m, pizza.Price);
        Assert.Equal(PizzaSize.Size.Medium, pizza.Size);
    }

    [Fact]
    public void ConstructorsShouldThrowWhenIDIsZero()
    {
        //Arrange
        List<Ingredient> ingredients = new List<Ingredient>
        {
            new Ingredient(1, "Tomato", 1.0m, true),
            new Ingredient(2, "Cheese", 1.5m, true),
            new Ingredient(3, "Pineapple", 0.5m, true)
        };
        //Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Pizza(0, "Margherita", 10.0m, ingredients));
        Assert.Equal("ID cannot be zero. (Parameter 'id')", exception.Message);

        exception = Assert.Throws<ArgumentException>(() => new Pizza(0, "Margherita", 10.0m, ingredients, PizzaSize.Size.Medium));
        Assert.Equal("ID cannot be zero. (Parameter 'id')", exception.Message);
    }

    [Fact]
    public void ConstructorsShouldThrowWhenPriceIsZeroOrLess()
    {
        //Arrange
        List<Ingredient> ingredients = new List<Ingredient>
        {
            new Ingredient(1, "Tomato", 1.0m, true),
            new Ingredient(2, "Cheese", 1.5m, true),
            new Ingredient(3, "Pineapple", 0.5m, true)
        };
        //Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Pizza(1, "Margherita", 0.0m, ingredients));
        Assert.Equal("Price must be greater than zero. (Parameter 'price')", exception.Message);
        exception = Assert.Throws<ArgumentException>(() => new Pizza(1, "Margherita", -1.0m, ingredients, PizzaSize.Size.Medium));
        Assert.Equal("Price must be greater than zero. (Parameter 'price')", exception.Message);
    }

    [Fact]
    public void AddIngreadientShouldAddIngredientWhenCorrect()
    {
        //Arrange
        List<Ingredient> ingredients = new List<Ingredient>
        {
            new Ingredient(1, "Tomato", 1.0m, true),
            new Ingredient(2, "Cheese", 1.5m, true),
            new Ingredient(3, "Pineapple", 0.5m, true)
        };
        Pizza pizza = new Pizza(1, "Margherita", 10.0m, ingredients);

        //Act
        pizza.AddIngredient(new Ingredient(4, "Pepperoni", 1.5m, true));

        //Assert
        Assert.Equal(4, pizza.GetIngredients().Count);
    }

    [Fact]
    public void RemoveIngredientShouldRemoveIngredientWhenCorrect()
    {
        //Arrange
        List<Ingredient> ingredients = new List<Ingredient>
        {
            new Ingredient(1, "Tomato", 1.0m, true),
            new Ingredient(2, "Cheese", 1.5m, true),
            new Ingredient(3, "Pineapple", 0.5m, true)
        };
        Pizza pizza = new Pizza(1, "Margherita", 10.0m, ingredients);

        //Act
        pizza.RemoveIngredient(new Ingredient(1, "Tomato", 1.0m, true));

        //Assert
        Assert.Equal(2, pizza.GetIngredients().Count);
    }
}
