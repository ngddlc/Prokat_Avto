using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoRentalApp.Models
{
    [Table("cars")]
    public class Car
    {
        [Key]
        public int CarID { get; set; }

        [Required]
        [StringLength(20)]
        [RegularExpression(@"^[А-Я]{1}\d{3}[А-Я]{2}\s\d{2,3}$")]
        public string PlateNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Brand { get; set; }

        [Required]
        [StringLength(50)]
        public string Model { get; set; }

        [Required]
        [Range(1990, 2030)]
        public int Year { get; set; }

        [Required]
        [StringLength(30)]
        public string Color { get; set; }

        [Required]
        [StringLength(30)]
        public string BodyType { get; set; }

        [Required]
        [StringLength(20)]
        public string EngineType { get; set; }

        [Required]
        [Range(0.5, 10.0)]
        public decimal EngineVolume { get; set; }

        [Required]
        [StringLength(20)]
        public string TransmissionType { get; set; }

        [Required]
        [StringLength(20)]
        public string CarClass { get; set; }

        [Required]
        [Range(100, 50000)]
        public decimal DailyPrice { get; set; }

        [Required]
        [Range(0, 1000000)]
        public int Mileage { get; set; }

        public DateTime? LastServiceDate { get; set; }

        [Required]
        [StringLength(17, MinimumLength = 17)]
        public string VIN { get; set; }

        [Required]
        public int CarStatusID { get; set; }

        public virtual CarStatus CarStatus { get; set; }

        public string DisplayName => $"{Brand} {Model} ({PlateNumber})";
    }
}