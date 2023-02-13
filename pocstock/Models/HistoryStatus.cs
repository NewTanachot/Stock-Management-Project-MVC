using System.ComponentModel.DataAnnotations;

namespace pocstock.Models
{
    public class HistoryStatus
    {
        public int HistoryStatusId { get; set; }

        [MaxLength(255)]
        public string HistoryStatusName { get; set; } = string.Empty;
    }
}
