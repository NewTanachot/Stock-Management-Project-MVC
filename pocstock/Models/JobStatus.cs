using System.ComponentModel.DataAnnotations;

namespace pocstock.Models
{
    public class JobStatus
    {
        public int JobStatusId { get; set; }

        [MaxLength(255)]
        public string JobStatusName { get; set; } = string.Empty;
    }
}
