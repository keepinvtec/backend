using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using aspnet3.Models;
using aspnet3.Data;
using Microsoft.AspNetCore.Authorization;

namespace aspnet3.Controllers
{
    [Authorize(Roles = "Admin, Worker")]
    public class InvoiceController : Controller
    {
        private readonly CarServiceContext dbContext;

        public InvoiceController(CarServiceContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Create()
        {
            return View(new InvoiceModel(dbContext.Cars.ToList(), new Invoice()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Invoice invoice)
        {
            var temp = false;
            foreach (Car car in dbContext.Cars.ToList())
            {
                if (car.VINcode == invoice.CarVINcode)
                    temp = true;
            }
            if (!temp)
                invoice.Car.VINcode = invoice.CarVINcode;
            else
            {
                var temp1 = invoice.InvoiceId;
                var temp2 = invoice.CarVINcode;
                invoice = new Invoice { InvoiceId = temp1, CarVINcode = temp2 };
            }

            if (invoice.CarVINcode.Contains("Select"))
                ModelState.AddModelError("", "The VINcode field is required.");
            if (invoice.CarVINcode.Length > 18)
                ModelState.AddModelError("", "VINcode's length need to be less than 19 symbols.");

            ModelState.Remove("invoice.Car.VINcode");
            ModelState.Remove("invoice.Car.Brand");
            ModelState.Remove("invoice.Car.YearOfProd");
            if (!ModelState.IsValid)
                return View(new InvoiceModel(dbContext.Cars.ToList(), new Invoice()));

            dbContext.Invoices.Add(invoice);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Invoice? invoice = await dbContext.Invoices.FirstOrDefaultAsync(p => p.InvoiceId == id);
                if (invoice != null)
                    return View(new InvoiceModel(dbContext.Cars.ToList(), invoice));
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Invoice invoice)
        {
            dbContext.Invoices.Update(invoice);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Invoice? invoice = await dbContext.Invoices.FirstOrDefaultAsync(p => p.InvoiceId == id);
                if (invoice != null)
                {
                    dbContext.Invoices.Remove(invoice);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
            }
            return NotFound();
        }
    }
}
