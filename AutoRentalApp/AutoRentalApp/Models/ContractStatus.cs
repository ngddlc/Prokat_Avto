using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoRentalApp.Models
{
    [Table("contractstatuses")]
    public class ContractStatus
    {
        [Key]
        public int ContractStatusID { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusName { get; set; }
    }
}