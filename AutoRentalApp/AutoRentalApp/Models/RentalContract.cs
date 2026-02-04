using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoRentalApp.Models
{
    [Table("rentalcontracts")]
    public class RentalContract
    {
        [Key]
        public int ContractID { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime PlannedEndDateTime { get; set; }

        public DateTime? ActualEndDateTime { get; set; }

        [Required]
        [Range(0, 1000000)]
        public decimal TotalAmount { get; set; }

        [Required]
        [Range(0, 500000)]
        public decimal DepositAmount { get; set; }

        [Required]
        public int ClientID { get; set; }

        [Required]
        public int CarID { get; set; }

        [Required]
        public int ManagerID { get; set; }

        [Required]
        public int ContractStatusID { get; set; }

        public virtual Client Client { get; set; }
        public virtual Car Car { get; set; }
        public virtual Employee Manager { get; set; }
        public virtual ContractStatus ContractStatus { get; set; }

        public string ContractNumber => $"ДОГ-{ContractID:D6}";
    }
}