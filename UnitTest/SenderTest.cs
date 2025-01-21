namespace UnitTest;
using MarioPizzaKassaApp.classes;

public class SenderTest
{
    [Fact]
    public void OrderToDatabaseShouldReturnTrueWhenOrderIsSaved()
    {
        //Arrange
        Order order = new Order();
        order.AddPizza(new Pizza(1, "Margherita", 10.0m, new List<Ingredient>(), PizzaSize.Size.Medium), new List<Ingredient>(), new List<Ingredient>());
        Sender sender = new Sender();
        //Act
        bool result = sender.OrderToDatabase(order);
        //Assert
        Assert.True(result);
    }
}
