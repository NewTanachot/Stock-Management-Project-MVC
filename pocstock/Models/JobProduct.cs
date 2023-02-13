using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pocstock.Models
{
    public class JobProduct
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(255)]
        public string JobId { get; set; } = string.Empty;

        public int ProductId { get; set; } = 0;

        public int ProductCount { get; set; } = 0;

        [NotMapped]
        public string ProductName { get; set; } = string.Empty;

        [NotMapped]
        public int ProductPrice { get; set; }
    }
}
