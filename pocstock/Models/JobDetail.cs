using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pocstock.Models
{
    public class JobDetail
    {
        [Key]
        [MaxLength(255)]
        public string JobId { get; set; } = string.Empty;

        //public string JobId { get; set; } = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("==", "");

        [MaxLength(255)]
        public string? JobDescription { get; set; } = string.Empty;

        public int JobWage { get; set; } = 0;

        public int JobStatusId { get; set; }

        [NotMapped]
        public string JobStatusName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? CompanyName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? CustomerName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? JobLocation { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? TaxId { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? CustomerPhoneNumber { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; }

        public bool Deleted { get; set; } = false;

        [NotMapped]
        public string StringDisplayDate { get; set; } = string.Empty;

        [NotMapped]
        public string DisplayDateForPrinting { get; set; } = string.Empty;

        [NotMapped]
        public List<JobProduct> JobProducts { get; set; } = new();

        [NotMapped]
        public List<JobStatus> JobDisplayStatus { get; set; } = new();
    }

    public class CreateJobDetail : JobDetail
    {
        public string StringProducts { get; set; } = string.Empty;

    }
}
