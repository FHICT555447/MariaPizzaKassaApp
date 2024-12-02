using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioPizzaKassaApp.classes
{
    public class Ingredient
    {
        //fields & properties
        public int ID { get; private set; }
        public string Name {  get; private set; }
        public decimal PurchasePrice { get; private set; }
        public bool IsFinishingIngredient { get; private set; }

        //constructor
        public Ingredient(int id, string name, decimal purchasePrice, bool ifFinishingIngredient)
        {
            ID = id;
            Name = name;
            PurchasePrice = purchasePrice;
            IsFinishingIngredient = ifFinishingIngredient;
        }

        //methods
        public static List<Ingredient> GetAllIngredients()
        {
            List<Ingredient> ingredients = new List<Ingredient>();

            string connectionString = "server=192.168.156.8;user=root;database=mario_db;port=3306;password=Y4GFV8cnLr5JMx2s";
            string query = "SELECT id, name, purchase_price, finishing_ingredient FROM ingredients";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    string name = reader.GetString("name");
                    decimal purchasePrice = reader.GetDecimal("purchase_price");
                    bool finishingIngredient = reader.GetBoolean("finishing_ingredient");

                    ingredients.Add(new Ingredient(id, name, purchasePrice, finishingIngredient));
                }
            }
            return ingredients;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Ingredient))
                return false;

            Ingredient other = (Ingredient)obj;
            return this.ID == other.ID; // You can also check other properties if needed
        }

        // Override GetHashCode to ensure consistency with Equals
        public override int GetHashCode()
        {
            return ID.GetHashCode(); // Ensures that two ingredients with the same ID have the same hash code
        }
    }
}
