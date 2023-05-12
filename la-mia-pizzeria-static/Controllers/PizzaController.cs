using la_mia_pizzeria_static.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Web;

namespace la_mia_pizzeria_static.Controllers
{
    [Authorize]
    public class PizzaController : Controller
    {
        private IWebHostEnvironment Environment;
        private PizzaContext DB;

        public PizzaController(IWebHostEnvironment _environment, PizzaContext db)
        {
            Environment = _environment;
            DB = db;
        }
        public IActionResult Index()
        {
            List<Pizza> pizze = DB.Pizzas.ToList<Pizza>();

            return View(pizze);
        }

        public IActionResult Details(int Id)
        {
            Pizza pizza;
            try
            {
                pizza = DB.Pizzas.Include(p => p.Ingredients).First(p => p.PizzaId == Id);

                return View(pizza);  

            }catch (Exception)
            { 
                return View("NotFound", Id);    
            }
            
        }

        [Authorize(Roles = "ADMIN")]
        public IActionResult Create()
        {
            List<Category> categories = DB.Categories.ToList();

            PizzaFormModel model = new PizzaFormModel();
            model.Pizza = new Pizza();
            model.Categories = categories;

            List<Ingredient> ingredients = DB.Ingredients.ToList();
            List<SelectListItem> listIngredients = new List<SelectListItem>();

            foreach (Ingredient ingredient in ingredients)
            {
                listIngredients.Add(
                        new SelectListItem()
                        {
                            Text = ingredient.Name,
                            Value = ingredient.Id.ToString()
                        }
                );
            }

            model.Ingredients = listIngredients;

            return View(model);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PizzaFormModel data, IFormFile img)
        {
            data.Pizza.ImgPath = Path.Combine(Environment.WebRootPath, "img", img.FileName);

            ModelState.ClearValidationState("Pizza.ImgPath");

            TryValidateModel(data);

            if (!ModelState.IsValid)
            {
                List<Category> category = DB.Categories.ToList();
                data.Categories = category;

                List<Ingredient> ingredients = DB.Ingredients.ToList();
                List<SelectListItem> listIngredients = new List<SelectListItem>();

                foreach (Ingredient ingredient in ingredients)
                {
                    listIngredients.Add
                    (
                        new SelectListItem()
                        {
                            Text = ingredient.Name,
                            Value = ingredient.Id.ToString()
                        }
                    );
                }
                data.Ingredients = listIngredients;
                return View("Create", data);
            }

            using (var stream = System.IO.File.Create(data.Pizza.ImgPath))
            {
                await img.CopyToAsync(stream);
            }

            Pizza pizzaToCreate = new Pizza();
            pizzaToCreate.Name = data.Pizza.Name;
            pizzaToCreate.Ingredients = new List<Ingredient>();
            pizzaToCreate.ImgPath = "~/" + Path.GetRelativePath(Environment.WebRootPath, data.Pizza.ImgPath);
            pizzaToCreate.Price = data.Pizza.Price;
            pizzaToCreate.CategoryId = data.Pizza.CategoryId;

            if(data.SelectedIngredients != null)
            {
                foreach (string selectedIngredientId in data.SelectedIngredients)
                {
                    int selectIntIngredientId = int.Parse(selectedIngredientId);
                    Ingredient ingredient = DB.Ingredients.FirstOrDefault(m => m.Id == selectIntIngredientId);
                    pizzaToCreate.Ingredients.Add(ingredient);
                }
            }


            DB.Pizzas.Add(pizzaToCreate);
            DB.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            Pizza? pizza = DB.Pizzas.Include(p => p.Ingredients).FirstOrDefault(p => p.PizzaId == Id);

            if (pizza == null)
                return View("NotFound", Id);
            else
            {
                List<Category> categories = DB.Categories.ToList();

                PizzaFormModel model = new PizzaFormModel();
                model.Pizza = pizza;
                model.Categories = categories;

                List<Ingredient> ingredients = DB.Ingredients.ToList();
                List<SelectListItem> listIngredients = new List<SelectListItem>();
                foreach (Ingredient ingredient in ingredients)
                {
                    listIngredients.Add(
                        new SelectListItem()
                        {
                            Text = ingredient.Name,
                            Value = ingredient.Id.ToString(),
                            Selected = pizza.Ingredients.Any(m => m.Id == ingredient.Id)
                        }
                    );
                }
                model.Ingredients = listIngredients;
                return View(model);
            }
            
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PizzaFormModel data, IFormFile img)
        {
            if (img != null)
            {
                var imgToDelete = Path.GetFullPath(Path.Combine(Environment.WebRootPath, data.Pizza.ImgPath.Substring(2)));
                System.IO.File.Delete(imgToDelete);

                data.Pizza.ImgPath = Path.Combine(Environment.WebRootPath, "img", img.FileName);

                ModelState.ClearValidationState("Pizza.ImgPath");

                TryValidateModel(data);
            } else
            {
                ModelState["img"].ValidationState = ModelValidationState.Valid;
            }

            if (!ModelState.IsValid)
            {
                data.Categories = DB.Categories.ToList();

                List<Ingredient> ingredients = DB.Ingredients.ToList();
                List<SelectListItem> listIngredients = new List<SelectListItem>();
                foreach (Ingredient ingredient in ingredients)
                {
                    listIngredients.Add(
                        new SelectListItem()
                        {
                            Text = ingredient.Name,
                            Value = ingredient.Id.ToString(),
                        }
                    );
                }

                data.Ingredients = listIngredients;
                return View("Edit", data);
            }

            if (img != null)
            {
                using (var stream = System.IO.File.Create(data.Pizza.ImgPath))
                {
                    await img.CopyToAsync(stream);
                }
            }

            Pizza pizzaToEdit = DB.Pizzas.Include(p => p.Ingredients).FirstOrDefault(p => p.PizzaId == data.Pizza.PizzaId);
            pizzaToEdit.Ingredients.Clear();

            pizzaToEdit.Name = data.Pizza.Name;
            pizzaToEdit.ImgPath = img == null ? data.Pizza.ImgPath : "~/" + Path.GetRelativePath(Environment.WebRootPath, data.Pizza.ImgPath);
            pizzaToEdit.Price = data.Pizza.Price;
            pizzaToEdit.CategoryId = data.Pizza.CategoryId;

            if (data.SelectedIngredients != null)
            {
                foreach (string selectedIngredientId in data.SelectedIngredients)
                {
                    int selectIntIngredientId = int.Parse(selectedIngredientId);
                    Ingredient ingredient = DB.Ingredients
                    .Where(m => m.Id == selectIntIngredientId)
                    .FirstOrDefault();
                    pizzaToEdit.Ingredients.Add(ingredient);
                }
            }

            DB.SaveChanges();

            return RedirectToAction("Index");
            
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
     
            Pizza? pizzaToDelete = DB.Pizzas.Where(pizza => pizza.PizzaId == id).FirstOrDefault();

            if (pizzaToDelete != null)
            {
                var imgToDelete = Path.GetFullPath(Path.Combine(Environment.WebRootPath, pizzaToDelete.ImgPath.Substring(2)));
                System.IO.File.Delete(imgToDelete);

                DB.Pizzas.Remove(pizzaToDelete);

                DB.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
            
        }
    }
}
