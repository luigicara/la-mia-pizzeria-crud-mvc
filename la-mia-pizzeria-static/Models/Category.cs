using System.ComponentModel.DataAnnotations;

namespace la_mia_pizzeria_static.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public string Nome { get; set; }

        public List<Pizza> Pizze { get; set; }

        public static void Seed()
        {
            using (PizzaContext db = new PizzaContext())
            {

                var seed = new Category[]
                {
                    new Category
                    {
                        Nome = "PizzaBuona"
                    }
                };

                db.AddRange(seed);
                db.SaveChanges();
            }

        }
    }
}

