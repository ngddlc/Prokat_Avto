using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoRentalApp.Models
{
    [Table("carstatuses")]
    public class CarStatus
    {
        [Key]
        public int CarStatusID { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusName { get; set; }
    }
}