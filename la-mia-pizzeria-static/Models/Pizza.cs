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
        [StringLength(250, ErrorMessage = "Il campo non può avere più di 250 caratteri")]
        public string Ingredients { get; set; }
        [Required(ErrorMessage = "Il campo è obbligatorio")]
        [ImgValidation]
        public string ImgPath { get; set; }
        [Required(ErrorMessage = "Il campo è obbligatorio")]
        [Range(18, int.MaxValue, ErrorMessage = "Il prezzo dev'essere positivo")]
        public decimal Price { get; set; }

        public static void Seed()
        {
            using (PizzaContext db = new PizzaContext())
            {
                var seed = new Pizza[]
                {
                    new Pizza
                    {
                        Name = "Marinara",
                        Ingredients = "Pomodoro, origano, aglio, olio",
                        ImgPath = "~/img/marinara.jpg",
                        Price = 4.00m
                    },
                    new Pizza
                    {
                        Name = "Margherita",
                        Ingredients = "Pomodoro, mozzarella",
                        ImgPath = "~/img/margherita.jpg",
                        Price = 4.00m
                    },
                    new Pizza
                    {
                        Name = "Capricciosa",
                        Ingredients = "Pomodoro, mozzarella, funghi, olive, capperi",
                        ImgPath = "~/img/capricciosa.jpg",
                        Price = 4.00m
                    }
                };

                db.AddRange(seed);
                db.SaveChanges();
            }
        }
    }
}
