using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pocstock.Models
{
    public class StockHistoryLog
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [NotMapped]
        public string ProductName { get; set; } = string.Empty;

        public int ActionId { get; set; }

        [NotMapped]
        public string ActionName { get; set; } = string.Empty;

        public int ActionNumber { get; set; }

        [MaxLength(255)]
        public string? JobNo { get; set; }

        [NotMapped]
        public string? JobName { get; set; }

        public DateTime DateTime { get; set; }

        [MaxLength(255)]
        public string Remark { get; set; } = string.Empty;

        [NotMapped]
        public string StringDisplayDate { get; set; } = string.Empty;
    }
}
