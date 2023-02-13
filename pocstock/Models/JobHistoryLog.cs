using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pocstock.Models
{
    public class JobHistoryLog
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(255)] 
        public string JobNo { get; set; } = string.Empty;

        [NotMapped]
        public string JobName { get; set; } = string.Empty;

        public int ActionId { get; set; }

        [NotMapped]
        public int ProductCount { get; set; }

        [NotMapped]
        public string ActionName { get; set; } = string.Empty;

        public DateTime DateTime { get; set; }

        [MaxLength(255)]
        public string Remark { get; set; } = string.Empty;

        [NotMapped]
        public string StringDisplayDate { get; set; } = string.Empty;
    }
}
