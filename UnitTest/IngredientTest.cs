using MarioPizzaKassaApp.classes;
using MySql.Data.MySqlClient;
using System;

namespace UnitTest
{
    public class IngredientTest
    {
        [Fact]
        public void ConstructorShouldInitializePropertiesCorrectly()
        {
            //Arrange
            string name = "Tomato";

            //Act
            Ingredient ingredient = new Ingredient(1, name, 1.0m, true);

            //Assert
            Assert.Equal(1, ingredient.ID);
            Assert.Equal(name, ingredient.Name);
            Assert.Equal(1.0m, ingredient.PurchasePrice);
        }

        [Fact]
        public void ConstructorShouldThrowArgumentExceptionWhenIDIsZero()
        {
            //Arrange
            string name = "Tomato";

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Ingredient(0, name, 1.0m, true));
            Assert.Equal("ID must be greater than zero. (Parameter 'id')", exception.Message);
        }

        [Fact]
        public void ConstructorShouldThrowArgumentExceptionWhenNameIsNull()
        {
            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Ingredient(1, null, 1.0m, true));
            Assert.Equal("Name cannot be null or empty. (Parameter 'name')", exception.Message);
        }

        [Fact]
        public void ConstructorShouldThrowArgumentExceptionWhenNameIsEmpty()
        {
            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Ingredient(1, "", 1.0m, true));
            Assert.Equal("Name cannot be null or empty. (Parameter 'name')", exception.Message);
        }

        [Fact]
        public void GetAllIngredientsShouldReturnListOfIngredients()
        {
            //Arrange
            string connectionString = "server=192.168.126.9;user=root;database=mario_db;port=3306;password=Y4GFV8cnLr5JMx2s";
            //Act
            List<Ingredient> ingredients = Ingredient.GetAllIngredients(connectionString);
            //Assert
            Assert.NotEmpty(ingredients);
        }

        [Fact]
        public void GetAllIngredientsShouldThrowExceptionWhenNoConnection()
        {
            // Arrange
            string invalidConnectionString = "server=192.168.126.9;user=root;database=mario_db;port=3306;password=InvalidPassword";

            // Act & Assert
            var exception = Assert.Throws<MySqlException>(() => Ingredient.GetAllIngredients(invalidConnectionString));

            // Validate that the exception message contains a relevant sub-string
            Assert.Contains("Authentication to host", exception.Message);
        }
    }
}
