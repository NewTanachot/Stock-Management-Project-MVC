using Microsoft.EntityFrameworkCore;
using pocstock.Models;

namespace pocstock.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Product { get; set; }

        public DbSet<Stock> Stock { get; set; }

        public DbSet<StockHistoryLog> StockHistoryLog { get; set; }

        public DbSet<JobDetail> JobDetail { get; set; }

        public DbSet<JobProduct> JobProduct { get; set; }
        
        public DbSet<JobStatus> JobStatus { get; set; }

        public DbSet<HistoryStatus> HistoryStatus { get; set; }

        public DbSet<JobHistoryLog> JobHistoryLog { get; set; }
    }
}
