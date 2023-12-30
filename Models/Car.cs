using System.ComponentModel.DataAnnotations;

namespace aspnet3.Models
{
    public class Car
    {
        public string VINcode { get; set; } = null!;

        [Required]
        public string? Brand { get; set; }

        public string? Model { get; set; }

        [Required]
        [Range(2001, 2022)]
        public int? YearOfProd { get; set; }

        public int Mileage { get; set; }

        public virtual List<Invoice> Invoices { get; } = new List<Invoice>();
    }
}
