using System.ComponentModel.DataAnnotations;
using la_mia_pizzeria_static.Validations;

namespace la_mia_pizzeria_static.Models
{
    public class Pizza
    {
        public int PizzaId { get; set; }

        [Required(ErrorMessage = "Il campo è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il nome non può avere più di 50 caratteri")]
        public string Name { get; set; }
      
        [Required(ErrorMessage = "Il campo è obbligatorio")]
        [ImgValidation]
        public string ImgPath { get; set; }

        [Required(ErrorMessage = "Il campo è obbligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "Il prezzo dev'essere positivo")]
        public decimal Price { get; set; }

        public List<Ingredient>? Ingredients { get; set; }

        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        public static void Seed()
        {
            using (PizzaContext db = new PizzaContext())
            {

                var pizza = new Pizza()
                {
                    Name = "Marinara",
                    ImgPath = "~/img/marinara.jpg",
                    Price = 4.00m,
                    CategoryId = 1,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient()
                        {
                            Name = "Pomodoro"
                        },
                        new Ingredient()
                        {
                            Name = "Origano"
                        }
                    }
                };

                db.Add(pizza);
                db.SaveChanges();

                pizza = new Pizza()
                {
                    Name = "Margherita",
                    ImgPath = "~/img/margherita.jpg",
                    Price = 4.00m,
                    CategoryId = 1,
                    Ingredients = new List<Ingredient>
                    {
                        db.Ingredients.Where(c => c.Name == "Pomodoro").FirstOrDefault(),
                        new Ingredient()
                        {
                            Name = "Mozzarella"
                        }
                    }
                };

                db.Add(pizza);
                db.SaveChanges();

                pizza = new Pizza()
                {
                    Name = "Capricciosa",
                    ImgPath = "~/img/capricciosa.jpg",
                    Price = 4.00m,
                    CategoryId = 1,
                    Ingredients = new List<Ingredient>
                    {
                        db.Ingredients.Where(c => c.Name == "Pomodoro").FirstOrDefault(),
                        db.Ingredients.Where(c => c.Name == "Mozzarella").FirstOrDefault(),
                        new Ingredient()
                        {
                            Name = "Prosciutto Cotto"
                        },
                        new Ingredient()
                        {
                            Name = "Funghi"
                        },
                        new Ingredient()
                        {
                            Name = "Olive"
                        }
                    }
                };

                db.Add(pizza);
                db.SaveChanges();

            }
        }
    }
}
