using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using aspnet3.Models;
using aspnet3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace aspnet3.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly CarServiceContext dbContext;
        private readonly RoleManager<IdentityRole> roleManager;

        public HomeController(CarServiceContext dbContext, RoleManager<IdentityRole> roleManager)
        {
            this.dbContext = dbContext;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string brand, int invoice = 0, int page = 1,
            SortState sortOrder = SortState.InvoiceAsc)
        {
            int pageSize = 3;

            IQueryable<Invoice> invoices = dbContext.Invoices.Include(x => x.Car);

            if (invoice != 0)
            {
                invoices = invoices.Where(p => p.InvoiceId == invoice);
            }
            if (!string.IsNullOrEmpty(brand))
            {
                if (brand != "All")
                    invoices = invoices.Where(p => p.Car.Brand!.Contains(brand));
            }

            switch (sortOrder)
            {
                case SortState.InvoiceDesc:
                    invoices = invoices.OrderByDescending(s => s.InvoiceId);
                    break;
                case SortState.BrandAsc:
                    invoices = invoices.OrderBy(s => s.Car!.Brand);
                    break;
                case SortState.BrandDesc:
                    invoices = invoices.OrderByDescending(s => s.Car!.Brand);
                    break;
                default:
                    invoices = invoices.OrderBy(s => s.InvoiceId);
                    break;
            }

            var count = await invoices.CountAsync();
            var items = await invoices.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            IndexViewModel viewModel = new IndexViewModel(
                items,
                new PageViewModel(count, page, pageSize),
                new FilterViewModel(dbContext.Cars.ToList(), brand, invoice),
                new SortViewModel(sortOrder)
            );
            return View("~/Views/Invoice/Index.cshtml", viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegRoles()
        {
            var roles = new[] {"Admin", "Worker", "Generic"};
            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
            
            return RedirectToAction("Index");
        }
    }
}
