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

        public PizzaController(IWebHostEnvironment _environment)
        {
            Environment = _environment;
        }
        public IActionResult Index()
        {
            using (PizzaContext db = new PizzaContext())
            {
                List<Pizza> pizze = db.Pizzas.ToList<Pizza>();

                return View(pizze);
            }
        }

        public IActionResult Details(int Id)
        {
            using (PizzaContext db = new PizzaContext())
            {
                Pizza pizza;
                try
                {
                    pizza = db.Pizzas.First(p => p.PizzaId == Id);

                    return View(pizza);  

                }catch (Exception)
                { 
                    return View("NotFound", Id);    
                }
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pizza data, IFormFile img)
        {
            data.ImgPath = Path.Combine(Environment.WebRootPath, "img", img.FileName);

            ModelState.ClearValidationState("ImgPath");

            TryValidateModel(data);

            if (!ModelState.IsValid)
            {
                return View("Create", data);
            }

            using (var stream = System.IO.File.Create(data.ImgPath))
            {
                await img.CopyToAsync(stream);
            }

            using (PizzaContext db = new PizzaContext())
            {
                Pizza pizzaToCreate = new Pizza();
                pizzaToCreate.Name = data.Name;
                pizzaToCreate.Ingredients = data.Ingredients;
                pizzaToCreate.ImgPath = "~/" + Path.GetRelativePath(Environment.WebRootPath, data.ImgPath);
                pizzaToCreate.Price = data.Price;
                db.Pizzas.Add(pizzaToCreate);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            using (PizzaContext db = new PizzaContext())
            {
                Pizza? pizza = db.Pizzas.FirstOrDefault(p => p.PizzaId == Id);
                if (pizza == null)
                    return View("NotFound", Id);
                else
                {
                    return View(pizza);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Pizza data, IFormFile img)
        {
            if (img != null)
            {
                var imgToDelete = Path.GetFullPath(Path.Combine(Environment.WebRootPath, data.ImgPath.Substring(2)));
                System.IO.File.Delete(imgToDelete);

                data.ImgPath = Path.Combine(Environment.WebRootPath, "img", img.FileName);

                ModelState.ClearValidationState("ImgPath");

                TryValidateModel(data);
            } else
            {
                ModelState["img"].ValidationState = ModelValidationState.Valid;
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            if (img != null)
            {
                using (var stream = System.IO.File.Create(data.ImgPath))
                {
                    await img.CopyToAsync(stream);
                }
            }

            using (PizzaContext db = new PizzaContext())
            {
                Pizza pizzaToEdit = db.Pizzas.First(p => p.PizzaId == data.PizzaId);
                pizzaToEdit.Name = data.Name;
                pizzaToEdit.Ingredients = data.Ingredients;
                pizzaToEdit.ImgPath = img == null ? data.ImgPath : "~/" + Path.GetRelativePath(Environment.WebRootPath, data.ImgPath);
                pizzaToEdit.Price = data.Price;
     
                db.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using (PizzaContext db = new PizzaContext())
            {
                Pizza? pizzaToDelete = db.Pizzas.Where(pizza => pizza.PizzaId == id).FirstOrDefault();

                if (pizzaToDelete != null)
                {
                    var imgToDelete = Path.GetFullPath(Path.Combine(Environment.WebRootPath, pizzaToDelete.ImgPath.Substring(2)));
                    System.IO.File.Delete(imgToDelete);

                    db.Pizzas.Remove(pizzaToDelete);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound();
                }
            }
        }
    }
}
