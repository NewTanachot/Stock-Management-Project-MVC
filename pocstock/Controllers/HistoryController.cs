using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pocstock.Data;
using pocstock.Models;
using pocstock.Services;

namespace pocstock.Controllers
{
    public class HistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[HttpGet("{controller}/{action}/{filter}/{Increase}/{Decrease}/{Add}")]
        [HttpGet]
        public async Task<IActionResult>  HistoryPreview()
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var History = await _context.StockHistoryLog.ToListAsync();

            // Lazy MiddleWare for Clear Data if Data is too much
            if (History.Count > 50000)
            {
                var DeleteData = History.Where(x => x.DateTime.Date <= currentTime.Date.AddYears(-1)).ToList();

                _context.StockHistoryLog.RemoveRange(DeleteData);
                await _context.SaveChangesAsync();
            }
            // End MiddleWare

            TempData["StockHistoryActive"] = "active";
            return View();
        }

        [HttpGet("{controller}/{action}/{MonthFilter}/{CheckboxFilter?}")]
        public async Task<IActionResult> GetStockHistory(string MonthFilter, string? CheckboxFilter)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var StockHistory = new List<StockHistoryLog>();
            var Service = new Service();

            var HistoryStatus = await _context.HistoryStatus.ToListAsync();
            var AllProductFromDB = await _context.Product.ToListAsync();
            var History = await _context.StockHistoryLog.ToListAsync();

            var FilterStockHistory = History.Where(x => x.DateTime.Date >= currentTime.Date.AddMonths(-Int32.Parse(MonthFilter))).ToList();

            if (!string.IsNullOrEmpty(CheckboxFilter))
            {
                var ListofCheckbox = CheckboxFilter.Split(',').ToList();

                foreach (var item in ListofCheckbox)
                {
                    if (item == "Increase")
                    {
                        StockHistory.AddRange(FilterStockHistory.Where(x => x.ActionId == 2 || x.ActionId == 7).ToList());
                    }

                    if (item == "Decrease")
                    {
                        StockHistory.AddRange(FilterStockHistory.Where(x => x.ActionId == 3 || x.ActionId == 6).ToList());
                    }

                    // Create / Delete / Restore
                    if (item == "Add")
                    {
                        StockHistory.AddRange(FilterStockHistory.Where(x => x.ActionId == 1 || x.ActionId == 4 || x.ActionId == 5).ToList());
                    }
                }

                foreach (var history in StockHistory)
                {
                    var Product = AllProductFromDB.FirstOrDefault(x => x.Id == history.ProductId);

                    if (Product != null)
                    {
                        history.ProductName = Product.Name;
                    }

                    history.StringDisplayDate = Service.GenerateStringDateTime(history.DateTime, true);
                    history.ActionName = HistoryStatus.First(x => x.HistoryStatusId == history.ActionId).HistoryStatusName;

                    // Remark for Seconds in DateTime
                    history.Remark = history.DateTime.Second.ToString();
                }
            }

            return Ok(StockHistory);
        }

        [HttpGet]
        public async Task<IActionResult> JobHistoryPreview()
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var JobHistory = await _context.JobHistoryLog.ToListAsync();

            // Lazy MiddleWare for Clear Data if Data is too much
            if (JobHistory.Count > 50000)
            {
                var DeleteData = JobHistory.Where(x => x.DateTime.Date <= currentTime.Date.AddYears(-2)).ToList();

                _context.JobHistoryLog.RemoveRange(DeleteData);
                await _context.SaveChangesAsync();
            }
            // End MiddleWare

            var JobStatus = await _context.JobStatus.ToListAsync();

            TempData["StatusForJobHistory"] = JobStatus;
            TempData["JobHistoryActive"] = "active";
            return View();
        }

        [HttpGet("{controller}/{action}/{StatusFilter}/{MonthFilter}")]
        public async Task<IActionResult> GetJobHistory(string StatusFilter, string MonthFilter)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var Service = new Service();
            var JobStatus = await _context.JobStatus.ToListAsync();
            var JobHistory = await _context.JobHistoryLog.ToListAsync();
            var JobProductFromDB = await _context.JobProduct.ToListAsync();

            // Month Filter
            var FilterJobHistory = JobHistory.Where(x => x.DateTime.Date >= currentTime.Date.AddMonths(-int.Parse(MonthFilter))).ToList();

            // Status Filter
            if (int.Parse(StatusFilter) != 0)
            {
                FilterJobHistory = FilterJobHistory.Where(x => x.ActionId == int.Parse(StatusFilter)).ToList();
            }

            foreach (var item in FilterJobHistory)
            {
                int ProductCount = 0;
                var StatusName = JobStatus.FirstOrDefault(x => x.JobStatusId == item.ActionId);
                var JobProduct = JobProductFromDB.Where(x => x.JobId == item.JobNo).ToList();
                
                foreach (var product in JobProduct)
                {
                    ProductCount += product.ProductCount;
                }

                if (StatusName != null)
                {
                    item.ProductCount = ProductCount;
                    item.ActionName = StatusName.JobStatusName;
                    item.StringDisplayDate = Service.GenerateStringDateTime(item.DateTime);

                    // Remark for Seconds in DateTime
                    item.Remark = item.DateTime.Second.ToString();
                }
            }

            return Ok(FilterJobHistory);
        }
    }
}
