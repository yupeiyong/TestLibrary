using System.Collections.Generic;


namespace TestProject.Linq
{

    public sealed class Product
    {

        public int Id { get; set; }

        public string Category { get; set; }

        public double Value { get; set; }


        public override string ToString()
        {
            return $"[{Id}: {Category} - {Value}]";
        }


        public static List<Product> GetList()
        {
            var products = new List<Product>
            {
                new Product {Id = 1, Category = "Electronics", Value = 15.0},
                new Product {Id = 2, Category = "Groceries", Value = 40.0},
                new Product {Id = 3, Category = "Garden", Value = 210.3},
                new Product {Id = 4, Category = "Pets", Value = 2.1},
                new Product {Id = 5, Category = "Electronics", Value = 19.95},
                new Product {Id = 6, Category = "Pets", Value = 21.25},
                new Product {Id = 7, Category = "Pets", Value = 5.50},
                new Product {Id = 8, Category = "Garden", Value = 13.0},
                new Product {Id = 9, Category = "Automotive", Value = 10.0},
                new Product {Id = 10, Category = "Electronics", Value = 250.0}
            };
            return products;
        }

    }

}