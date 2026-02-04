using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoRentalApp.Models
{
    [Table("carinspections")]
    public class CarInspection
    {
        [Key]
        public int InspectionID { get; set; }

        [Required]
        public int ContractID { get; set; }

        [Required]
        [StringLength(20)]
        public string InspectionType { get; set; }

        [Required]
        public DateTime InspectionDate { get; set; }

        public string Notes { get; set; }

        public decimal? DamageCost { get; set; }

        public virtual RentalContract Contract { get; set; }
    }
}