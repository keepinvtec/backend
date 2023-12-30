using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using aspnet3.Models;
using aspnet3.Data;

namespace aspnet3.Controllers
{
    [ApiController]
    [Route("api/car")]
    public class CarApiController : Controller
    {
        private readonly CarServiceContext dbContext;

        public CarApiController(CarServiceContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<List<Car>> GetCars()
        {
            return await dbContext.Cars.ToListAsync();
        }

        [HttpGet("{vincode}")]
        public async Task<IActionResult> Get(string vincode)
        {
            Car? car = await dbContext.Cars.FirstOrDefaultAsync(p => p.VINcode == vincode);
            if (car != null)
            {
                return Ok(car);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Car car)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            dbContext.Cars.Add(car);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{vincode},{year}")]
        public async Task<IActionResult> Put(string vincode, int year, string model)
        {
            if (year <= 2000 || year > 2022)
                return BadRequest();

            Car? temp = await dbContext.Cars.FirstOrDefaultAsync(p => p.VINcode == vincode);
            if (temp != null)
            {
                temp.YearOfProd = year;
                temp.Model = model;
                dbContext.Cars.Update(temp);
                await dbContext.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }

        [HttpDelete("{vincode}")]
        public async Task<IActionResult> Delete(string vincode)
        {
            Car? car = await dbContext.Cars.FirstOrDefaultAsync(p => p.VINcode == vincode);
            if (car != null)
            {
                dbContext.Cars.Remove(car);
                await dbContext.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }
    }
}
