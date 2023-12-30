using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using aspnet3.Models;
using aspnet3.Data;
using Microsoft.AspNetCore.Authorization;

namespace aspnet3.Controllers
{
    [Authorize(Roles = "Admin, Worker")]
    public class CarController : Controller
    {
        private readonly CarServiceContext dbContext;

        public CarController(CarServiceContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View(new CarModel(dbContext.Cars.ToList()));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Car car)
        {
            if (car.VINcode?.Length > 18)
                ModelState.AddModelError("VINcode", "VINcode's length need to be less than 19 symbols.");

            ModelState.Remove("Mileage");
            if (!ModelState.IsValid)
                return View(car);

            dbContext.Cars.Add(car);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Car");
        }

        public async Task<IActionResult> Edit(string? vincode)
        {
            if (!String.IsNullOrEmpty(vincode))
            {
                Car? car = await dbContext.Cars.FirstOrDefaultAsync(p => p.VINcode == vincode);
                if (car != null)
                    return View(car);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Car car)
        {
            ModelState.Remove("Mileage");
            if (!ModelState.IsValid)
                return View(car);

            dbContext.Cars.Update(car);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Car");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string? vincode)
        {
            if (!String.IsNullOrEmpty(vincode))
            {
                Car? car = await dbContext.Cars.FirstOrDefaultAsync(p => p.VINcode == vincode);
                if (car != null)
                {
                    dbContext.Cars.Remove(car);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Index", "Car");
                }
            }
            return NotFound();
        }
    }
}
