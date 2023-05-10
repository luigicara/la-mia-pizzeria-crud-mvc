using la_mia_pizzeria_static.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Web;

namespace la_mia_pizzeria_static.Controllers
{
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
                pizza = DB.Pizzas.First(p => p.PizzaId == Id);

                return View(pizza);  

            }catch (Exception)
            { 
                return View("NotFound", Id);    
            }
            
        }

        public IActionResult Create()
        {
            List<Category> categories = DB.Categories.ToList();

            PizzaFormModel model = new PizzaFormModel();
            model.Pizza = new Pizza();
            model.Categories = categories;
            return View(model);
        }

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
                data.Pizza = new Pizza();
                return View("Create", data);
            }

            using (var stream = System.IO.File.Create(data.Pizza.ImgPath))
            {
                await img.CopyToAsync(stream);
            }

            Pizza pizzaToCreate = new Pizza();
            pizzaToCreate.Name = data.Pizza.Name;
            pizzaToCreate.Ingredients = data.Pizza.Ingredients;
            pizzaToCreate.ImgPath = "~/" + Path.GetRelativePath(Environment.WebRootPath, data.Pizza.ImgPath);
            pizzaToCreate.Price = data.Pizza.Price;
            pizzaToCreate.CategoryId = data.Pizza.CategoryId;
            DB.Pizzas.Add(pizzaToCreate);
            DB.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {

            Pizza? pizza = DB.Pizzas.FirstOrDefault(p => p.PizzaId == Id);
            List<Category> categories = DB.Categories.ToList();

            PizzaFormModel model = new PizzaFormModel();
            model.Pizza = pizza;
            model.Categories = categories;

            if (pizza == null)
                return View("NotFound", Id);
            else
            {
                return View(model);
            }
            
        }

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
                return View("Edit", data);
            }

            if (img != null)
            {
                using (var stream = System.IO.File.Create(data.Pizza.ImgPath))
                {
                    await img.CopyToAsync(stream);
                }
            }

            Pizza pizzaToEdit = DB.Pizzas.First(p => p.PizzaId == data.Pizza.PizzaId);
            pizzaToEdit.Name = data.Pizza.Name;
            pizzaToEdit.Ingredients = data.Pizza.Ingredients;
            pizzaToEdit.ImgPath = img == null ? data.Pizza.ImgPath : "~/" + Path.GetRelativePath(Environment.WebRootPath, data.Pizza.ImgPath);
            pizzaToEdit.Price = data.Pizza.Price;
            pizzaToEdit.CategoryId = data.Pizza.CategoryId;

            DB.SaveChanges();

            return RedirectToAction("Index");
            
        }

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
