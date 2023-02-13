using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pocstock.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "โปรดใส่ชื่อ")]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "โปรดใส่ราคาขาย")]
        public int SellingPrice { get; set; }

        [Required(ErrorMessage = "โปรดใส่ราคาทุน")]
        public int CostPrice { get; set; }

        public DateTime UpdateDate { get; set; } = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

        public bool Deleted { get; set; } = false;

        [NotMapped]
        public string StringDisplayDate { get; set; } = string.Empty;

        [NotMapped]
        public int ProductCount { get; set; } = 0;
    }
}
