using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pocstock.Models
{
    public class Stock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [NotMapped]
        public string Name { get; set; } = string.Empty;

        public int Number { get; set; } = 0;

        public DateTime StockTime { get; set; } = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

        public bool Deleted { get; set; } = false;

        [NotMapped]
        public string StringDisplayDate { get; set; } = string.Empty;
    }

    public class EditStock
    {
        public int Id { get; set; } = 0;

        public int Count { get; set; } = 0;
    }
}
