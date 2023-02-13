using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pocstock.Data;
using pocstock.Models;
using pocstock.Services;

namespace pocstock.Controllers
{
    public class FileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ImportExcelPage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcelFile(IFormFile ExcelFile)
        {
            try
            {
                var workbook = new XLWorkbook(ExcelFile.OpenReadStream());

                // Declaration
                var ProductWorkSheet = workbook.Worksheet("ProductBackUp");
                var StockWorkSheet = workbook.Worksheet("StockBackUp");
                var JobDetailWorkSheet = workbook.Worksheet("JobDetailBackUp");
                var JobProductWorkSheet = workbook.Worksheet("JobProductBackUp");
                var StockHistoryStatusWorkSheet = workbook.Worksheet("StockHistoryStatusBackUp");
                var JobHistoryStatusWorkSheet = workbook.Worksheet("JobHistoryStatusBackUp");
                var StockHistoryWorkSheet = workbook.Worksheet("StockHistoryBackUp");
                var JobHistoryWorkSheet = workbook.Worksheet("JobHistoryBackUp");

                var ProductList = new List<Product>();
                var StockList = new List<Stock>();
                var JobDetailList = new List<JobDetail>();
                var JobProductList = new List<JobProduct>();
                var StockHistoryStatusList = new List<HistoryStatus>();
                var JobHistoryStatusList = new List<JobStatus>();
                var StockHistoryList = new List<StockHistoryLog>();
                var JobHistoryList = new List<JobHistoryLog>();

                // Delete Title Row
                ProductWorkSheet.FirstRow().Delete();
                StockWorkSheet.FirstRow().Delete();
                JobDetailWorkSheet.FirstRow().Delete();
                JobProductWorkSheet.FirstRow().Delete();   
                StockHistoryStatusWorkSheet.FirstRow().Delete();
                JobHistoryStatusWorkSheet.FirstRow().Delete();
                StockHistoryWorkSheet.FirstRow().Delete();
                JobHistoryWorkSheet.FirstRow().Delete();

                var ProductWorkSheetCount = Convert.ToInt32(ProductWorkSheet.Cell(1, 7).Value);
                var StockWorkSheetCount = Convert.ToInt32(StockWorkSheet.Cell(1, 5).Value);
                var JobDetailWorkSheetCount = Convert.ToInt32(JobDetailWorkSheet.Cell(1, 12).Value);
                var JobProductWorkSheetCount = Convert.ToInt32(JobProductWorkSheet.Cell(1, 4).Value);
                var StockHistoryStatusWorkSheetCount = Convert.ToInt32(StockHistoryStatusWorkSheet.Cell(1, 3).Value);
                var JobHistoryStatusWorkSheetCount = Convert.ToInt32(JobHistoryStatusWorkSheet.Cell(1, 3).Value);
                var StockHistoryWorkSheetCount = Convert.ToInt32(StockHistoryWorkSheet.Cell(1, 7).Value);
                var JobHistoryWorkSheetCount = Convert.ToInt32(JobHistoryWorkSheet.Cell(1, 5).Value);

                // Add Data to ListData
                for (var row = 1; row <= ProductWorkSheetCount; row++)
                {
                    ProductList.Add(new Product
                    {
                        //Id = Convert.ToInt32(Row.Cell(1).Value),
                        Name = ProductWorkSheet.Cell(row, 2).Value.ToString()?.Trim() ?? string.Empty,
                        SellingPrice = Convert.ToInt32(ProductWorkSheet.Cell(row, 3).Value),
                        CostPrice = Convert.ToInt32(ProductWorkSheet.Cell(row, 4).Value),
                        UpdateDate = Convert.ToDateTime(ProductWorkSheet.Cell(row, 5).Value),
                        Deleted = Convert.ToBoolean(ProductWorkSheet.Cell(row, 6).Value),
                    });
                }
                for (var row = 1; row <= StockWorkSheetCount; row++)
                {
                    StockList.Add(new Stock
                    {
                        //Id = Convert.ToInt32(StockWorkSheet.Cell(row, 1).Value),
                        Id = row,
                        Number = Convert.ToInt32(StockWorkSheet.Cell(row, 2).Value),
                        StockTime = Convert.ToDateTime(StockWorkSheet.Cell(row, 3).Value),
                        Deleted = Convert.ToBoolean(StockWorkSheet.Cell(row, 4).Value)
                    });
                }
                for (var row = 1; row <= JobDetailWorkSheetCount; row++)
                {
                    JobDetailList.Add(new JobDetail
                    {
                        JobId = JobDetailWorkSheet.Cell(row, 1).Value.ToString()?.Trim() ?? string.Empty,
                        CreateDate = Convert.ToDateTime(JobDetailWorkSheet.Cell(row, 2).Value),
                        JobDescription = JobDetailWorkSheet.Cell(row, 3).Value.ToString()?.Trim(),
                        JobStatusId = Convert.ToInt32(JobDetailWorkSheet.Cell(row, 4).Value),
                        CompanyName = JobDetailWorkSheet.Cell(row, 5).Value.ToString()?.Trim(),
                        CustomerName = JobDetailWorkSheet.Cell(row, 6).Value.ToString()?.Trim(),
                        TaxId = JobDetailWorkSheet.Cell(row, 7).Value.ToString()?.Trim(),
                        CustomerPhoneNumber = JobDetailWorkSheet.Cell(row, 8).Value.ToString()?.Trim(),
                        JobLocation = JobDetailWorkSheet.Cell(row, 9).Value.ToString()?.Trim(),
                        JobWage = Convert.ToInt32(JobDetailWorkSheet.Cell(row, 10).Value),
                        Deleted = Convert.ToBoolean(JobDetailWorkSheet.Cell(row, 11).Value),
                    });
                }
                for (var row = 1; row <= JobProductWorkSheetCount; row++)
                {
                    JobProductList.Add(new JobProduct
                    {
                        JobId = JobProductWorkSheet.Cell(row, 1).Value.ToString()?.Trim() ?? string.Empty,
                        ProductId = Convert.ToInt32(JobProductWorkSheet.Cell(row, 2).Value),
                        ProductCount = Convert.ToInt32(JobProductWorkSheet.Cell(row, 3).Value),
                    });
                }
                for (var row = 1; row <= StockHistoryStatusWorkSheetCount; row++)
                {
                    StockHistoryStatusList.Add(new HistoryStatus
                    {
                        HistoryStatusName = StockHistoryStatusWorkSheet.Cell(row, 2).Value.ToString()?.Trim() ?? string.Empty
                    });
                }
                for (var row = 1; row <= JobHistoryStatusWorkSheetCount; row++)
                {
                    JobHistoryStatusList.Add(new JobStatus
                    {
                        JobStatusName = JobHistoryStatusWorkSheet.Cell(row, 2).Value.ToString()?.Trim() ?? string.Empty
                    });
                }
                for (var row = 1; row <= StockHistoryWorkSheetCount; row++)
                {
                    StockHistoryList.Add(new StockHistoryLog
                    {
                        ProductId = Convert.ToInt32(StockHistoryWorkSheet.Cell(row, 1).Value),
                        ActionId = Convert.ToInt32(StockHistoryWorkSheet.Cell(row, 2).Value),
                        ActionNumber = Convert.ToInt32(StockHistoryWorkSheet.Cell(row, 3).Value),
                        JobNo = StockHistoryWorkSheet.Cell(row, 4).Value.ToString()?.Trim(),
                        DateTime = Convert.ToDateTime(StockHistoryWorkSheet.Cell(row, 5).Value),
                        Remark = StockHistoryWorkSheet.Cell(row, 6).Value.ToString()?.Trim() ?? string.Empty,
                    });
                }
                for (var row = 1; row <= JobHistoryWorkSheetCount; row++)
                {
                    JobHistoryList.Add(new JobHistoryLog
                    {
                        JobNo = JobHistoryWorkSheet.Cell(row, 1).Value.ToString()?.Trim() ?? string.Empty,
                        ActionId = Convert.ToInt32(JobHistoryWorkSheet.Cell(row, 2).Value),
                        DateTime = Convert.ToDateTime(JobHistoryWorkSheet.Cell(row, 3).Value),
                        Remark = JobHistoryWorkSheet.Cell(row, 4).Value.ToString()?.Trim() ?? string.Empty
                    });
                }

                // Delete all Old Data in Database
                //await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Product");
                //await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Stock");
                //await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE JobDetail");
                //await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE JobProduct");
                //await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE StockHistoryLog");
                //await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE JobHistoryLog");
                //await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE HistoryStatus");
                //await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE JobStatus");

                // For SQLite
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Product");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Stock");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM JobDetail");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM JobProduct");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM StockHistoryLog");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM JobHistoryLog");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM HistoryStatus");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM JobStatus");
                // Reset ID Sqlite_sequence ONLY FOR SQLite
                await _context.Database.ExecuteSqlRawAsync("delete from sqlite_sequence where name='Product'");
                await _context.Database.ExecuteSqlRawAsync("delete from sqlite_sequence where name='Stock'");
                await _context.Database.ExecuteSqlRawAsync("delete from sqlite_sequence where name='JobDetail'");
                await _context.Database.ExecuteSqlRawAsync("delete from sqlite_sequence where name='JobProduct'");
                await _context.Database.ExecuteSqlRawAsync("delete from sqlite_sequence where name='StockHistoryLog'");
                await _context.Database.ExecuteSqlRawAsync("delete from sqlite_sequence where name='JobHistoryLog'");
                await _context.Database.ExecuteSqlRawAsync("delete from sqlite_sequence where name='HistoryStatus'");
                await _context.Database.ExecuteSqlRawAsync("delete from sqlite_sequence where name='JobStatus'");

                // Add ListData to Database
                await _context.Product.AddRangeAsync(ProductList);
                await _context.Stock.AddRangeAsync(StockList); 
                await _context.JobDetail.AddRangeAsync(JobDetailList);
                await _context.JobProduct.AddRangeAsync(JobProductList);
                await _context.HistoryStatus.AddRangeAsync(StockHistoryStatusList);
                await _context.JobStatus.AddRangeAsync(JobHistoryStatusList);
                await _context.StockHistoryLog.AddRangeAsync(StockHistoryList);
                await _context.JobHistoryLog.AddRangeAsync(JobHistoryList);

                await _context.SaveChangesAsync();
                return RedirectToAction("StockPreview", "Stock");
            }
            catch
            {
                TempData["ImportFail"] = $"<strong>&nbsp;เกิดข้อผิดพลาด!!</strong> &nbsp ไม่สามารถอัพโหลดข้อมูลได้ โปรดตรวจสอบไฟล์ Excel ใหม่อีกครั้ง " +
                    $"<strong> &nbsp;*มีข้อมูลในโปรแกรมอยู่แล้ว &nbsp;*อาจเกิดจากมีการแก้ไขฟอร์แมตของไฟล์ Excel</strong>";

                return View("../Home/Index");
            }
        }

        public IActionResult GenerateExcelConfirm()
        {
            return View();
        }

        public async Task<IActionResult> GenerateExcelFile()
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var Service = new Service();

            // Get Data
            var Product = await _context.Product.ToListAsync();
            var Stock = await _context.Stock.ToListAsync();
            var JobDetail = await _context.JobDetail.ToListAsync();
            var JobProduct = await _context.JobProduct.ToListAsync();
            var StockStatus = await _context.HistoryStatus.ToListAsync();
            var JobStatus = await _context.JobStatus.ToListAsync();
            var StockHistory = await _context.StockHistoryLog.ToListAsync();
            var JobHistory = await _context.JobHistoryLog.ToListAsync();

            // Excel WorkSheet Generate
            var workbook = new XLWorkbook();
            var ProductWorkSheet = workbook.Worksheets.Add("ProductBackUp");
            var StockWorkSheet = workbook.Worksheets.Add("StockBackUp");
            var StockHistoryWorkSheet = workbook.Worksheets.Add("StockHistoryBackUp");
            var JobDetailWorkSheet = workbook.Worksheets.Add("JobDetailBackUp");
            var JobProductWorkSheet = workbook.Worksheets.Add("JobProductBackUp");
            var JobHistoryWorkSheet = workbook.Worksheets.Add("JobHistoryBackUp");
            var StockHistoryStatusWorkSheet = workbook.Worksheets.Add("StockHistoryStatusBackUp");
            var JobHistoryStatusWorkSheet = workbook.Worksheets.Add("JobHistoryStatusBackUp");

            // Product
            var currentRow = 1;
            ProductWorkSheet.Cell(currentRow, 1).Value = "Id";
            ProductWorkSheet.Cell(currentRow, 2).Value = "Name";
            ProductWorkSheet.Cell(currentRow, 3).Value = "SellingPrice";
            ProductWorkSheet.Cell(currentRow, 4).Value = "CostPrice";
            ProductWorkSheet.Cell(currentRow, 5).Value = "UpdateDate";
            ProductWorkSheet.Cell(currentRow, 6).Value = "Deleted";
            ProductWorkSheet.Cell(currentRow, 7).Value = "จำนวนข้อมูล";
            ProductWorkSheet.Cell(currentRow + 1, 7).Value = Product.Count;

            foreach (var item in Product)
            {
                currentRow++;
                ProductWorkSheet.Cell(currentRow, 1).Value = item.Id;
                ProductWorkSheet.Cell(currentRow, 2).Value = item.Name;
                ProductWorkSheet.Cell(currentRow, 3).Value = item.SellingPrice;
                ProductWorkSheet.Cell(currentRow, 4).Value = item.CostPrice;
                ProductWorkSheet.Cell(currentRow, 5).Value = item.UpdateDate;
                ProductWorkSheet.Cell(currentRow, 6).Value = item.Deleted ? 1 : 0;
            }

            // Stock
            currentRow = 1;
            StockWorkSheet.Cell(currentRow, 1).Value = "Id";
            StockWorkSheet.Cell(currentRow, 2).Value = "Number";
            StockWorkSheet.Cell(currentRow, 3).Value = "StockTime";
            StockWorkSheet.Cell(currentRow, 4).Value = "Deleted";
            StockWorkSheet.Cell(currentRow, 5).Value = "จำนวนข้อมูล";
            StockWorkSheet.Cell(currentRow + 1, 5).Value = Stock.Count;

            foreach (var item in Stock)
            {
                currentRow++;
                StockWorkSheet.Cell(currentRow, 1).Value = item.Id;
                StockWorkSheet.Cell(currentRow, 2).Value = item.Number;
                StockWorkSheet.Cell(currentRow, 3).Value = item.StockTime;
                StockWorkSheet.Cell(currentRow, 4).Value = item.Deleted ? 1 : 0;
            }

            // JobDetail
            currentRow = 1;
            JobDetailWorkSheet.Cell(currentRow, 1).Value = "JobId";
            JobDetailWorkSheet.Cell(currentRow, 2).Value = "CreateDate";
            JobDetailWorkSheet.Cell(currentRow, 3).Value = "JobDescription";
            JobDetailWorkSheet.Cell(currentRow, 4).Value = "JobStatusId";
            JobDetailWorkSheet.Cell(currentRow, 5).Value = "CompanyName";
            JobDetailWorkSheet.Cell(currentRow, 6).Value = "CustomerName";
            JobDetailWorkSheet.Cell(currentRow, 7).Value = "TaxId";
            JobDetailWorkSheet.Cell(currentRow, 8).Value = "CustomerPhoneNumber";
            JobDetailWorkSheet.Cell(currentRow, 9).Value = "JobLocation";
            JobDetailWorkSheet.Cell(currentRow, 10).Value = "JobWage";
            JobDetailWorkSheet.Cell(currentRow, 11).Value = "Deleted";
            JobDetailWorkSheet.Cell(currentRow, 12).Value = "จำนวนข้อมูล";
            JobDetailWorkSheet.Cell(currentRow + 1, 12).Value = JobDetail.Count;

            foreach (var item in JobDetail)
            {
                currentRow++;
                JobDetailWorkSheet.Cell(currentRow, 1).Value = item.JobId;
                JobDetailWorkSheet.Cell(currentRow, 2).Value = item.CreateDate;
                JobDetailWorkSheet.Cell(currentRow, 3).Value = item.JobDescription;
                JobDetailWorkSheet.Cell(currentRow, 4).Value = item.JobStatusId;
                JobDetailWorkSheet.Cell(currentRow, 5).Value = item.CompanyName;
                JobDetailWorkSheet.Cell(currentRow, 6).Value = item.CustomerName;
                JobDetailWorkSheet.Cell(currentRow, 7).Value = item.TaxId;
                JobDetailWorkSheet.Cell(currentRow, 8).Value = item.CustomerPhoneNumber;
                JobDetailWorkSheet.Cell(currentRow, 9).Value = item.JobLocation;
                JobDetailWorkSheet.Cell(currentRow, 10).Value = item.JobWage;
                JobDetailWorkSheet.Cell(currentRow, 11).Value = item.Deleted ? 1 : 0;
            }

            // JobProduct
            currentRow = 1;
            JobProductWorkSheet.Cell(currentRow, 1).Value = "JobId";
            JobProductWorkSheet.Cell(currentRow, 2).Value = "ProductId";
            JobProductWorkSheet.Cell(currentRow, 3).Value = "ProductCount";
            JobProductWorkSheet.Cell(currentRow, 4).Value = "จำนวนข้อมูล";
            JobProductWorkSheet.Cell(currentRow + 1, 4).Value = JobProduct.Count;

            foreach (var item in JobProduct)
            {
                currentRow++;
                JobProductWorkSheet.Cell(currentRow, 1).Value = item.JobId;
                JobProductWorkSheet.Cell(currentRow, 2).Value = item.ProductId;
                JobProductWorkSheet.Cell(currentRow, 3).Value = item.ProductCount;
            }

            // Stock Status
            currentRow = 1;
            StockHistoryStatusWorkSheet.Cell(currentRow, 1).Value = "HistoryStatusId";
            StockHistoryStatusWorkSheet.Cell(currentRow, 2).Value = "HistoryStatusName";
            StockHistoryStatusWorkSheet.Cell(currentRow, 3).Value = "จำนวนข้อมูล";
            StockHistoryStatusWorkSheet.Cell(currentRow + 1, 3).Value = StockStatus.Count;

            foreach (var item in StockStatus)
            {
                currentRow++;
                StockHistoryStatusWorkSheet.Cell(currentRow, 1).Value = item.HistoryStatusId;
                StockHistoryStatusWorkSheet.Cell(currentRow, 2).Value = item.HistoryStatusName;
            }

            // Job Status
            currentRow = 1;
            JobHistoryStatusWorkSheet.Cell(currentRow, 1).Value = "JobStatusId";
            JobHistoryStatusWorkSheet.Cell(currentRow, 2).Value = "JobStatusName";
            JobHistoryStatusWorkSheet.Cell(currentRow, 3).Value = "จำนวนข้อมูล";
            JobHistoryStatusWorkSheet.Cell(currentRow + 1, 3).Value = JobStatus.Count;

            foreach (var item in JobStatus)
            {
                currentRow++;
                JobHistoryStatusWorkSheet.Cell(currentRow, 1).Value = item.JobStatusId;
                JobHistoryStatusWorkSheet.Cell(currentRow, 2).Value = item.JobStatusName;
            }

            // Stock History
            currentRow = 1;
            StockHistoryWorkSheet.Cell(currentRow, 1).Value = "ProductId";
            StockHistoryWorkSheet.Cell(currentRow, 2).Value = "ActionId";
            StockHistoryWorkSheet.Cell(currentRow, 3).Value = "ActionNumber";
            StockHistoryWorkSheet.Cell(currentRow, 4).Value = "JobNo";
            StockHistoryWorkSheet.Cell(currentRow, 5).Value = "DateTime";
            StockHistoryWorkSheet.Cell(currentRow, 6).Value = "Remark";
            StockHistoryWorkSheet.Cell(currentRow, 7).Value = "จำนวนข้อมูล";
            StockHistoryWorkSheet.Cell(currentRow + 1, 7).Value = StockHistory.Count;

            foreach (var item in StockHistory)
            {
                currentRow++;
                StockHistoryWorkSheet.Cell(currentRow, 1).Value = item.ProductId;
                StockHistoryWorkSheet.Cell(currentRow, 2).Value = item.ActionId;
                StockHistoryWorkSheet.Cell(currentRow, 3).Value = item.ActionNumber;
                StockHistoryWorkSheet.Cell(currentRow, 4).Value = item.JobNo;
                StockHistoryWorkSheet.Cell(currentRow, 5).Value = item.DateTime;
                StockHistoryWorkSheet.Cell(currentRow, 6).Value = item.Remark;
            }

            // Job History
            currentRow = 1;
            JobHistoryWorkSheet.Cell(currentRow, 1).Value = "JobNo";
            JobHistoryWorkSheet.Cell(currentRow, 2).Value = "ActionId";
            JobHistoryWorkSheet.Cell(currentRow, 3).Value = "DateTime";
            JobHistoryWorkSheet.Cell(currentRow, 4).Value = "Remark";
            JobHistoryWorkSheet.Cell(currentRow, 5).Value = "จำนวนข้อมูล";
            JobHistoryWorkSheet.Cell(currentRow + 1, 5).Value = JobHistory.Count;

            foreach (var item in JobHistory)
            {
                currentRow++;
                JobHistoryWorkSheet.Cell(currentRow, 1).Value = item.JobNo;
                JobHistoryWorkSheet.Cell(currentRow, 2).Value = item.ActionId;
                JobHistoryWorkSheet.Cell(currentRow, 3).Value = item.DateTime;
                JobHistoryWorkSheet.Cell(currentRow, 4).Value = item.Remark;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TJP_BackUpFile_{Service.GenerateStringDateTimeForFileName(currentTime)}.xlsx");
        }

        public async Task<IActionResult> GenerateExcelInvoice(string JobId)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var Service = new Service();
            string JobName = string.Empty;

            // Get data
            var JobDetail = await _context.JobDetail.FirstOrDefaultAsync(x => x.JobId == JobId);
            var JobProduct = await _context.JobProduct.Where(x => x.JobId == JobId).ToListAsync();
            var Product = await _context.Product.ToListAsync();

            // Excel WorkSheet Generate
            var workbook = new XLWorkbook();
            var InvoiceWorkSheet = workbook.Worksheets.Add("Invoice Bill");

            if (JobDetail != null)
            {
                var JobStatus = await _context.JobStatus.Where(x => x.JobStatusId == JobDetail.JobStatusId).Select(x => x.JobStatusName).FirstOrDefaultAsync();

                // Generate JobDetail Data
                var currentRow = 1;
                InvoiceWorkSheet.Cell(currentRow, 1).Value = "ชื่อบิล";
                InvoiceWorkSheet.Cell(currentRow, 2).Value = JobDetail.JobId;

                currentRow++;
                InvoiceWorkSheet.Cell(currentRow, 1).Value = "วันที่เปิดบิล";
                InvoiceWorkSheet.Cell(currentRow, 2).Value = JobDetail.CreateDate;

                currentRow++;
                InvoiceWorkSheet.Cell(currentRow, 1).Value = "รายละเอียดบิล";
                InvoiceWorkSheet.Cell(currentRow, 2).Value = JobDetail.JobDescription;

                currentRow++;
                InvoiceWorkSheet.Cell(currentRow, 1).Value = "ชื่อบริษัท";
                InvoiceWorkSheet.Cell(currentRow, 2).Value = JobDetail.CompanyName;

                currentRow++;
                InvoiceWorkSheet.Cell(currentRow, 1).Value = "ชื่อลูกค้า";
                InvoiceWorkSheet.Cell(currentRow, 2).Value = JobDetail.CustomerName;

                currentRow++;
                InvoiceWorkSheet.Cell(currentRow, 1).Value = "เลขผู้เสียภาษี";
                InvoiceWorkSheet.Cell(currentRow, 2).Value = JobDetail.CustomerPhoneNumber;

                currentRow++;
                InvoiceWorkSheet.Cell(currentRow, 1).Value = "ที่อยู่";
                InvoiceWorkSheet.Cell(currentRow, 2).Value = JobDetail.JobLocation;

                currentRow++;
                InvoiceWorkSheet.Cell(currentRow, 1).Value = "โทร";
                InvoiceWorkSheet.Cell(currentRow, 2).Value = JobDetail.CustomerPhoneNumber;

                // Can Check Null like this !!!!
                currentRow++;
                InvoiceWorkSheet.Cell(currentRow, 1).Value = "สถานะ";
                InvoiceWorkSheet.Cell(currentRow, 2).Value = JobStatus ?? string.Empty;

                if (JobProduct.Any())
                {
                    currentRow += 2;
                    InvoiceWorkSheet.Cell(currentRow, 1).Value = "รายการสินค้า";
                    currentRow++;

                    currentRow++;
                    InvoiceWorkSheet.Cell(currentRow, 1).Value = "จำนวน (ชิ้น)";
                    InvoiceWorkSheet.Cell(currentRow, 2).Value = "สินค้า";
                    InvoiceWorkSheet.Cell(currentRow, 3).Value = "หน่วยละ (บาท)";
                    InvoiceWorkSheet.Cell(currentRow, 4).Value = "จำนวนเงิน (บาท)";

                    currentRow++;
                    InvoiceWorkSheet.Cell(currentRow, 2).Value = "ค่าบริการรวมทั้งหมด";
                    InvoiceWorkSheet.Cell(currentRow, 4).Value = JobDetail.JobWage;

                    foreach (var item in JobProduct)
                    {
                        var ThisProduct = Product.FirstOrDefault(x => x.Id == item.ProductId);

                        if (ThisProduct != null)
                        {
                            currentRow++;
                            InvoiceWorkSheet.Cell(currentRow, 1).Value = item.ProductCount;
                            InvoiceWorkSheet.Cell(currentRow, 2).Value = ThisProduct.Name;
                            InvoiceWorkSheet.Cell(currentRow, 3).Value = ThisProduct.SellingPrice;
                            InvoiceWorkSheet.Cell(currentRow, 4).Value = ThisProduct.SellingPrice * item.ProductCount;
                        }
                    }
                }
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();


            return File(content, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                $"TJP_Invoice_{JobName}_{Service.GenerateStringDateTimeForFileName(currentTime)}.xlsx");
        }
    }
}
