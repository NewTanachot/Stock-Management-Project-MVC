using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using pocstock.Data;
using pocstock.Models;
using pocstock.Services;

namespace pocstock.Controllers
{
    public class JobDetailController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobDetailController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> JobPreview()
        {
            //var Service = new Service();
            //var JobStatus = await _context.JobStatus.ToListAsync();
            //var JobDetails = new List<JobDetail>();

            //if (Filter == 0)
            //{
            //    JobDetails = await _context.JobDetail.Where(x => !x.Deleted).ToListAsync();
            //}
            //else
            //{
            //    JobDetails = await _context.JobDetail.Where(x => !x.Deleted && x.JobStatusId == Filter).ToListAsync();
            //}

            //foreach (var item in JobDetails)
            //{
            //    item.StringDisplayDate = Service.GenerateStringDateTime(item.CreateData);
            //    item.JobStatusName = JobStatus.First(x => x.JobStatusId == item.JobStatusId).JobStatusName;
            //}

            var JobStatus = await _context.JobStatus.ToListAsync();
            TempData["JobStatus"] = JobStatus;

            TempData["JobDetailActive"] = "active";
            return View();
        }

        [HttpGet("/JobDetail/GetJob/{Filter}")]
        public async Task<IActionResult> GetJob(int Filter)
        {
            var Service = new Service();
            var JobStatus = await _context.JobStatus.ToListAsync();
            var JobDetails = new List<JobDetail>();

            if (Filter == 0)
            {
                JobDetails = await _context.JobDetail.Where(x => !x.Deleted).ToListAsync();
            }
            else
            {
                JobDetails = await _context.JobDetail.Where(x => !x.Deleted && x.JobStatusId == Filter).ToListAsync();
            }

            foreach (var item in JobDetails)
            {
                if (string.IsNullOrEmpty(item.CompanyName))
                {
                    item.CompanyName = "-";
                }

                if (string.IsNullOrEmpty(item.CustomerName))
                {
                    item.CustomerName = "-";
                }

                item.StringDisplayDate = Service.GenerateStringDateTime(item.CreateDate);
                item.JobStatusName = JobStatus.First(x => x.JobStatusId == item.JobStatusId).JobStatusName;
            }

            return Ok(JobDetails);
        }

        public IActionResult CreateJob()
        {
            var JobIdForNewCreateJob = new CreateJobDetail
            {
                JobId = Guid.NewGuid().ToString().ToUpper(),
            };

            TempData["JobDetailActive"] = "active";
            return View(JobIdForNewCreateJob);
        }

        [HttpGet]
        public async Task<IActionResult> JobDetailPreview(string JobId)
        {
            var Service = new Service();
            var JobDetail = await _context.JobDetail.FirstOrDefaultAsync(x => x.JobId == JobId);

            if(JobDetail != null)
            {
                var JobProducts = await _context.JobProduct.Where(x => x.JobId == JobId).ToListAsync();

                if (JobProducts != null)
                {
                    var ProductFromDB = await _context.Product.ToListAsync();

                    foreach (var product in JobProducts)
                    {
                        var ProductMaster = ProductFromDB.FirstOrDefault(x => x.Id == product.ProductId);

                        if (ProductMaster != null)
                        {
                            product.ProductName = ProductMaster.Name;
                            product.ProductPrice = ProductMaster.SellingPrice;
                        }
                    }

                    JobDetail.JobProducts.AddRange(JobProducts);
                }

                // Thai DateTime Converter
                JobDetail.DisplayDateForPrinting = Service.ConvertToThaiDateTime(JobDetail.CreateDate);
                JobDetail.StringDisplayDate = Service.ConvertToThaiDateTime(JobDetail.CreateDate, true);

                // PhoneNumber Converter
                if (!string.IsNullOrEmpty(JobDetail.CustomerPhoneNumber))
                {
                    JobDetail.CustomerPhoneNumber = Service.ConvertToPhoneNumberFormat(JobDetail.CustomerPhoneNumber);
                }

                // Get all JobStatus
                JobDetail.JobDisplayStatus.AddRange(await _context.JobStatus.ToListAsync());

                TempData["JobDetailActive"] = "active";
                return View(JobDetail);
            }

            return NotFound("JobId not Found");
        }

        [HttpPost]
        public async Task<IActionResult> StoreJobDetail(CreateJobDetail createJobDetail)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            List<JobProduct> JobProducts = new();
            var CuttingJobNameLastNum = new List<int>();
            var ProductHistory = new List<StockHistoryLog>();
            var Service = new Service();
            string jobName = string.Empty;

            // JobDetail Create
            var JobDetail = new JobDetail
            {
                JobId = createJobDetail.JobId,
                JobDescription = createJobDetail.JobDescription,
                JobStatusId = 1,
                CompanyName = createJobDetail.CompanyName,
                CustomerName = createJobDetail.CustomerName,
                CustomerPhoneNumber = createJobDetail.CustomerPhoneNumber,
                TaxId = createJobDetail.TaxId,
                JobLocation = createJobDetail.JobLocation,
                JobWage = createJobDetail.JobWage,
                CreateDate = currentTime
            };
            
            // JobHistory Create
            var JobHistoryLog = new JobHistoryLog
            {
                JobNo = createJobDetail.JobId,
                ActionId = 1,
                DateTime = currentTime
            };

            // JobProduct Create
            if (!string.IsNullOrEmpty(createJobDetail.StringProducts))
            {
                var allProductId = createJobDetail.StringProducts.Split('_').ToList();

                foreach (var ProductId in allProductId)
                {
                    var Product = await _context.Product.FindAsync(int.Parse(ProductId));

                    if (Product != null)
                    {
                        JobProducts.Add(new JobProduct
                        {
                            JobId = createJobDetail.JobId,
                            ProductId = Product.Id,
                            ProductCount = 1
                        });
                    }
                }

                var DistinctJobProducts = JobProducts.DistinctBy(x => x.ProductId).ToList();

                foreach (var item in DistinctJobProducts)
                {
                    var ProductNumber = JobProducts.Where(x => x.ProductId == item.ProductId).ToList();
                    item.ProductCount = ProductNumber.Count;

                    // Stock DisCount
                    var Stock = await _context.Stock.FirstOrDefaultAsync(x => x.Id == item.ProductId);

                    if (Stock != null)
                    {
                        Stock.Number -= ProductNumber.Count;

                        ProductHistory.Add(new StockHistoryLog
                        {
                            ProductId = Stock.Id,
                            ActionId = 6,
                            ActionNumber = ProductNumber.Count,
                            JobNo = createJobDetail.JobId,
                            DateTime = currentTime,
                        });
                    }
                }

                await _context.StockHistoryLog.AddRangeAsync(ProductHistory);
                await _context.JobProduct.AddRangeAsync(DistinctJobProducts);
            }

            TempData["CreateJobSuccess"] = $"<strong>&nbsp;สำเร็จ!!</strong> &nbsp; บันทึกข้อมูลรายการบิล <strong>{createJobDetail.JobId}</strong> เรียบร้อยแล้ว";

            await _context.JobDetail.AddAsync(JobDetail);
            await _context.JobHistoryLog.AddAsync(JobHistoryLog);
            await _context.SaveChangesAsync();

            return RedirectToAction("JobPreview");
        }

        [HttpGet("/JobDetail/EditJobDetail/{JobId}")]
        public async Task<IActionResult> EditJobDetail(string JobId)
        {
            var Job = await _context.JobDetail.FirstOrDefaultAsync(x => x.JobId == JobId);
            var Service = new Service();

            if (Job != null)
            {
                string ProductString = string.Empty;
                var allJobProduct = await _context.JobProduct.Where(x => x.JobId == Job.JobId).ToListAsync();
                
                if (allJobProduct.Any())
                {
                    foreach (var jobProduct in allJobProduct)
                    {
                        for (var i = 0; i < jobProduct.ProductCount; i++)
                        {
                            if (string.IsNullOrEmpty(ProductString))
                            {
                                ProductString = jobProduct.ProductId.ToString();
                            }
                            else
                            {
                                ProductString = ProductString + "_" + jobProduct.ProductId.ToString();
                            }
                        }
                    }
                }

                var UpdateJobDetail = new CreateJobDetail
                {
                    JobId = Job.JobId,
                    JobDescription = Job.JobDescription,
                    JobDisplayStatus = Job.JobDisplayStatus,
                    JobLocation = Job.JobLocation,
                    JobWage = Job.JobWage,
                    CustomerName = Job.CustomerName,
                    CustomerPhoneNumber = Job.CustomerPhoneNumber,
                    TaxId = Job.TaxId,
                    CompanyName = Job.CompanyName,
                    StringProducts = ProductString,
                };

                return View(UpdateJobDetail);
            }

            return NotFound("JobId not found");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateJobDetail(CreateJobDetail createJobDetail)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var Job = await _context.JobDetail.FirstOrDefaultAsync(x => x.JobId == createJobDetail.JobId);
            var ProductHistory = new List<StockHistoryLog>();
            var FinalJobProductForAdd = new List<JobProduct>();
            var Service = new Service();

            if (Job != null)
            {
                Job.JobDescription = createJobDetail.JobDescription;
                Job.CustomerName = createJobDetail.CustomerName;
                Job.CustomerPhoneNumber = createJobDetail.CustomerPhoneNumber;
                Job.TaxId = createJobDetail.TaxId;
                Job.CompanyName = createJobDetail.CompanyName;
                Job.JobLocation = createJobDetail.JobLocation;
                Job.JobWage = createJobDetail.JobWage;
                
                if (!string.IsNullOrEmpty(createJobDetail.StringProducts))
                {
                    var JobProductFormDb = await _context.JobProduct.Where(x => x.JobId.Equals(createJobDetail.JobId)).ToListAsync();
                    List<JobProduct> JobProducts = new();

                    var allProductId = createJobDetail.StringProducts.Split('_').ToList();

                    if (JobProductFormDb.Any())
                    {
                        var DistinctProduct = allProductId.Distinct().Select(int.Parse).ToList();
                        var SameProduct = DistinctProduct.Intersect(JobProductFormDb.Select(x => x.ProductId)).ToList();
                        var RemoveProduct = JobProductFormDb.Select(x => x.ProductId).Except(SameProduct).ToList();
                        var AddProduct = DistinctProduct.Except(SameProduct).ToList();

                        if (SameProduct.Any())
                        {
                            foreach (var item in SameProduct)
                            {
                                var Stock = await _context.Stock.FirstOrDefaultAsync(x => x.Id == item);
                                var UpdateJobProductFormDb = JobProductFormDb.First(x => x.ProductId.Equals(item));
                                var OldFormDBItemCount = UpdateJobProductFormDb.ProductCount;
                                var NewUpdateItemCount = allProductId.Where(x => x.Equals(item.ToString())).Count();

                                if (Stock != null)
                                {
                                    if (OldFormDBItemCount > NewUpdateItemCount)
                                    {
                                        var sum = OldFormDBItemCount - NewUpdateItemCount;

                                        // JobProduct Update
                                        UpdateJobProductFormDb.ProductCount = NewUpdateItemCount;

                                        // History Update
                                        ProductHistory.Add(new StockHistoryLog
                                        {
                                            ProductId = item,
                                            ActionId = 7,
                                            ActionNumber = sum,
                                            JobNo = createJobDetail.JobId,
                                            DateTime = currentTime,
                                        });

                                        // Stock Update
                                        Stock.Number += sum;
                                    }
                                    else if (OldFormDBItemCount < NewUpdateItemCount)
                                    {
                                        var sum = NewUpdateItemCount - OldFormDBItemCount;

                                        // JobProduct Update
                                        UpdateJobProductFormDb.ProductCount = NewUpdateItemCount;

                                        // History Update
                                        ProductHistory.Add(new StockHistoryLog
                                        {
                                            ProductId = item,
                                            ActionId = 6,
                                            ActionNumber = sum,
                                            JobNo = createJobDetail.JobId,
                                            DateTime = currentTime,
                                        });

                                        // Stock Update
                                        Stock.Number -= sum;
                                    }
                                }
                            }
                        }

                        if (AddProduct.Any())
                        {
                            foreach (var item in AddProduct)
                            {
                                var Stock = await _context.Stock.FirstOrDefaultAsync(x => x.Id == item);
                                var Number = allProductId.Where(x => x == item.ToString()).Count();

                                if (Stock != null)
                                {
                                    // JobProduct Update
                                    JobProducts.Add(new JobProduct
                                    {
                                        JobId = createJobDetail.JobId,
                                        ProductId = item,
                                        ProductCount = Number
                                    });

                                    // History Update
                                    ProductHistory.Add(new StockHistoryLog
                                    {
                                        ProductId = item,
                                        ActionId = 6,
                                        ActionNumber = Number,
                                        JobNo = createJobDetail.JobId,
                                        DateTime = currentTime,
                                    });

                                    // Stock Update
                                    Stock.Number -= Number;
                                }
                            }
                        }

                        if (RemoveProduct.Any())
                        {
                            foreach (var item in RemoveProduct)
                            {
                                var Stock = await _context.Stock.FirstOrDefaultAsync(x => x.Id == item);
                                var Number = JobProductFormDb.FirstOrDefault(x => x.ProductId == item);

                                if(Stock != null && Number != null)
                                {
                                    // Stock Update
                                    Stock.Number += Number.ProductCount;

                                    // History Update
                                    ProductHistory.Add(new StockHistoryLog
                                    {
                                        ProductId = item,
                                        ActionId = 7,
                                        ActionNumber = Number.ProductCount,
                                        JobNo = createJobDetail.JobId,
                                        DateTime = currentTime,
                                    });
                                }
                            }

                            // Delete JobProduct
                            var DeleteProduct = await _context.JobProduct.Where(x => RemoveProduct.Contains(x.ProductId)).ToListAsync();
                            _context.JobProduct.RemoveRange(DeleteProduct);
                        }

                        // Prepare for JobProduct Add to Database
                        FinalJobProductForAdd = JobProducts;
                    }
                    else
                    {
                        foreach (var ProductId in allProductId)
                        {
                            JobProducts.Add(new JobProduct
                            {
                                JobId = createJobDetail.JobId,
                                ProductId = int.Parse(ProductId),
                                ProductCount = 0
                            });
                        }

                        var DistinctJobProducts = JobProducts.DistinctBy(x => x.ProductId).ToList();

                        foreach (var item in DistinctJobProducts)
                        {
                            var ProductNumber = JobProducts.Where(x => x.ProductId == item.ProductId).ToList();
                            item.ProductCount = ProductNumber.Count;

                            // Stock DisCount
                            var Stock = await _context.Stock.FirstOrDefaultAsync(x => x.Id == item.ProductId);

                            if (Stock != null)
                            {
                                Stock.Number -= ProductNumber.Count;

                                ProductHistory.Add(new StockHistoryLog
                                {
                                    ProductId = Stock.Id,
                                    ActionId = 6,
                                    ActionNumber = ProductNumber.Count,
                                    JobNo = createJobDetail.JobId,
                                    DateTime = currentTime,
                                });
                            }
                        }

                        // Prepare for JobProduct Add to Database
                        FinalJobProductForAdd = DistinctJobProducts;
                    }
                }
                else
                {
                    var DeleteJobProduct = await _context.JobProduct.Where(x => x.JobId == createJobDetail.JobId).ToListAsync();
                    _context.JobProduct.RemoveRange(DeleteJobProduct);

                    foreach (var item in DeleteJobProduct)
                    {
                        // Stock Update
                        var Stock = await _context.Stock.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                        
                        if (Stock != null)
                        {
                            Stock.Number += item.ProductCount;
                        }

                        // History Update
                        ProductHistory.Add(new StockHistoryLog
                        {
                            ProductId = item.ProductId,
                            ActionId = 7,
                            ActionNumber = item.ProductCount,
                            JobNo = createJobDetail.JobId,
                            DateTime = currentTime,
                        });
                    }
                }

                await _context.JobProduct.AddRangeAsync(FinalJobProductForAdd);
                await _context.StockHistoryLog.AddRangeAsync(ProductHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction("JobDetailPreview", new { createJobDetail.JobId });
            }

            return NotFound("UpdateJobDetail : JobId not found");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteJobDetail(string JobId)
        {
            var Job = await _context.JobDetail.FirstOrDefaultAsync(x => x.JobId == JobId);

            if (Job != null)
            {
                return View(Job);
            }

            return NotFound("JobId not found");
        }

        [HttpGet("/JobDetail/UpdateJobStatus/{JobId}/{StatusInput}")]
        public async Task<IActionResult> UpdateJobStatus(string JobId, int StatusInput)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var Jobdetail = await _context.JobDetail.FirstOrDefaultAsync(x => x.JobId == JobId);

            if (Jobdetail != null)
            {
                Jobdetail.JobStatusId = StatusInput;

                var JobHistoryLog = new JobHistoryLog
                {
                    JobNo = JobId,
                    ActionId = StatusInput,
                    DateTime = currentTime
                };

                await _context.JobHistoryLog.AddAsync(JobHistoryLog);
                await _context.SaveChangesAsync();
            }

            return Ok();
            //return RedirectToAction("JobDetailPreview", new { JobId });
        }

        [HttpPost]
        public async Task<IActionResult> SetDeleteJob(string JobId)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var Job = await _context.JobDetail.FirstOrDefaultAsync(x => x.JobId == JobId);

            if (Job != null)
            {
                // JobProduct return to Stock
                var JobProduct = await _context.JobProduct.Where(x => x.JobId == JobId).ToListAsync();
                var StockHistory = new List<StockHistoryLog>();

                foreach (var product in JobProduct)
                {
                    var Stock = await _context.Stock.FirstOrDefaultAsync(x => x.Id == product.ProductId);

                    if (Stock != null)
                    {
                        Stock.Number += product.ProductCount;

                        // Stock History Update
                        StockHistory.Add(new StockHistoryLog
                        {
                            ProductId = product.ProductId,
                            ActionId = 7,
                            ActionNumber = product.ProductCount,
                            JobNo = product.JobId,
                            DateTime = currentTime,
                        });
                    }
                }

                // Set remove and Update History
                Job.Deleted = true;
                await _context.StockHistoryLog.AddRangeAsync(StockHistory);
            }

            TempData["DeleteJobSuccess"] = $"<strong>&nbsp;สำเร็จ!!</strong> &nbsp; ลบข้อมูลรายการบิล <strong>{JobId}</strong> เรียบร้อยแล้ว";

            await _context.SaveChangesAsync();
            return RedirectToAction("JobPreview");
        }

        [HttpGet]
        public async Task<IActionResult> TrashPreview()
        {
            var Service = new Service();
            var DeletedJob = await _context.JobDetail.Where(x => x.Deleted).ToListAsync();
            
            foreach (var item in DeletedJob)
            {
                item.StringDisplayDate = Service.GenerateStringDateTime(item.CreateDate); 
            }

            return View(DeletedJob);
        }

        [HttpGet]
        public async Task<IActionResult> RestoreJob(string JobId)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var RestoreJob = await _context.JobDetail.FirstOrDefaultAsync(x => x.JobId == JobId);

            if (RestoreJob != null)
            {
                // Add Product it to Job again
                var JobProduct = await _context.JobProduct.Where(x => x.JobId == JobId).ToListAsync();
                var StockHistory = new List<StockHistoryLog>();

                foreach (var product in JobProduct)
                {
                    var Stock = await _context.Stock.FirstOrDefaultAsync(x => x.Id == product.ProductId);

                    if (Stock != null)
                    {
                        Stock.Number -= product.ProductCount;

                        // Check Product is not enough for JobRestore
                        if (Stock.Number < 0)
                        {
                            var ProductFormDb = await _context.Product.FirstOrDefaultAsync(x => x.Id == Stock.Id);
                            TempData["RestoreJobFail"] = $"<strong>&nbsp;เกิดข้อผิดพลาด!!</strong> &nbsp; ไม่สามารถกู้คืนข้อมูลบิลได้ เนื่องจากสินค้า {ProductFormDb?.Name} ในสต๊อกไม่เพียงพอ";

                            return RedirectToAction("JobPreview");
                        }

                        // Stock History Update
                        StockHistory.Add(new StockHistoryLog
                        {
                            ProductId = product.ProductId,
                            ActionId = 6,
                            ActionNumber = product.ProductCount,
                            JobNo = product.JobId,
                            DateTime = currentTime,
                        });
                    }
                }

                // Restore Job and Update History
                RestoreJob.Deleted = false;
                await _context.StockHistoryLog.AddRangeAsync(StockHistory);
            }

            TempData["RestoreJobSuccess"] = $"<strong>&nbsp;สำเร็จ!!</strong> &nbsp; กู้คืนข้อมูลบิล <strong>{JobId}</strong> เรียบร้อยแล้ว";

            await _context.SaveChangesAsync();
            return RedirectToAction("JobPreview");
        }

        [HttpGet("/JobDetail/StockModal/{StringId}")]
        public async Task<IActionResult> StockModal(string StringId)
        {
            var Stocks = await _context.Stock.Where(x => !x.Deleted).ToListAsync();

            if (StringId != "Empty")
            {
                var ListOfId = StringId.Split('_').ToList();

                foreach (var Id in ListOfId)
                {
                    Stocks.First(x => x.Id == int.Parse(Id)).Number -= 1;
                }
            }

            Stocks = Stocks.Where(x => x.Number > 0).ToList();
            var ProductFromDb = await _context.Product.ToListAsync();

            foreach (var stock in Stocks)
            {
                var Product = ProductFromDb.FirstOrDefault(x => x.Id == stock.Id);
                
                if (Product != null)
                {
                    stock.Name = Product.Name;
                }
            }

            return View(Stocks);
        }

        [HttpGet("/JobDetail/UpdateStockModal/{StringId}/{JobId}")]
        public async Task<IActionResult> UpdateStockModal(string StringId, string JobId)
        {
            var Stocks = await _context.Stock.Where(x => !x.Deleted).ToListAsync();
            var JobProduct = await _context.JobProduct.Where(x => x.JobId == JobId).ToListAsync();

            if (StringId != "Empty")
            {
                var ListOfId = StringId.Split('_').ToList();

                // Create List of ProductId
                List<int> ListProductId = new();

                foreach (var jobProduct in JobProduct)
                {
                    for (var i = 0; i < jobProduct.ProductCount; i++)
                    {
                        ListProductId.Add(jobProduct.ProductId);
                    }
                }

                // Add Product Job
                foreach (var Id in ListOfId)
                {
                    var IndexOfProduct = ListProductId.IndexOf(int.Parse(Id));

                    if (IndexOfProduct != -1)
                    {
                        ListProductId.RemoveAt(IndexOfProduct);
                    }
                    else
                    {
                        Stocks.First(x => x.Id == int.Parse(Id)).Number -= 1;
                    }
                }

                // Minus Product Job
                foreach (var Id in ListOfId.Distinct())
                {
                    var NewProductCount = ListOfId.Where(x => x == Id).Count();
                    var OldProductCount = JobProduct.Where(x => x.ProductId == int.Parse(Id)).Select(x => x.ProductCount).FirstOrDefault(0); 

                    if (OldProductCount != 0 && NewProductCount < OldProductCount)
                    {
                        var sum = OldProductCount - NewProductCount;
                        Stocks.First(x => x.Id == int.Parse(Id)).Number += sum;
                    }
                }

                // Delete Product Job
                foreach (var item in JobProduct)
                {
                    if (!ListOfId.Contains(item.ProductId.ToString()))
                    {
                        Stocks.First(x => x.Id == item.ProductId).Number += item.ProductCount;
                    }
                }
            }
            else
            {
                // Delete all Product Job
                foreach (var item in JobProduct)
                {
                    Stocks.First(x => x.Id == item.ProductId).Number += item.ProductCount;
                }
            }


            Stocks = Stocks.Where(x => x.Number > 0).ToList();
            var ProductFromDb = await _context.Product.ToListAsync();

            foreach (var stock in Stocks)
            {
                var Product = ProductFromDb.FirstOrDefault(x => x.Id == stock.Id);

                if (Product != null)
                {
                    stock.Name = Product.Name;
                }
            }

            return View(Stocks);
        }

        [HttpGet("/JobDetail/GetProductDetail/{StringId}")]
        public async Task<IActionResult> GetProductDetail(string StringId)
        {
            List<Product> Results = new();
            List<string> ListOfId = StringId.Split('_').ToList();

            foreach (var eachId in ListOfId)
            {
                var ProductDetail = await _context.Product.FindAsync(int.Parse(eachId));

                if (ProductDetail != null)
                {
                    Results.Add(new Product
                    {
                        Id = ProductDetail.Id,
                        Name = ProductDetail.Name,
                        CostPrice = ProductDetail.CostPrice,
                        SellingPrice = ProductDetail.SellingPrice,
                        ProductCount = 1
                    });
                }
            }

            var FinalResult = Results.DistinctBy(x => x.Id).ToList();

            foreach (var item in FinalResult)
            {
                var ProductNumber = Results.Where(x => x.Id == item.Id).ToList();
                item.ProductCount = ProductNumber.Count;
                item.SellingPrice = item.ProductCount * await _context.Product.Where(x => x.Id == item.Id).Select(x => x.SellingPrice).FirstAsync();
            }

            FinalResult.Reverse();
            return Ok(FinalResult);
        }

        [HttpGet("/JobDetail/GetProductStock/{ProductId}")]
        public async Task<IActionResult> GetProductStock(int ProductId)
        {
            var Stock = await _context.Stock.FirstOrDefaultAsync(x => x.Id == ProductId);

            if (Stock != null)
            {
                return Ok(new { ProductCount = Stock.Number });
            }

            return NotFound("Product not found");
        }

        [HttpGet("/JobDetail/UpdateGetProductStock/{ProductId}/{JobId}")]
        public async Task<IActionResult> UpdateGetProductStock(int ProductId, string JobId)
        {
            var Stock = await _context.Stock.FirstOrDefaultAsync(x => x.Id == ProductId);
            var JobProduct = await _context.JobProduct.FirstOrDefaultAsync(x => x.JobId == JobId && x.ProductId == ProductId);

            if (Stock != null)
            {
                if (JobProduct != null)
                {
                    return Ok(new { ProductCount = Stock.Number + JobProduct.ProductCount });
                }
                else
                {
                    return Ok(new { ProductCount = Stock.Number });
                }
            }

            return NotFound("UpdateGetProductStock : Product not found");
        }

        [HttpGet("/JobDetail/FindCustomerInfo/{CustomerName}")]
        public async Task<IActionResult> FindCustomerInfo(string CustomerName)
        {
            return Ok(await _context.JobDetail.Where(x => x.CustomerName == CustomerName).OrderByDescending(x => x.CreateDate).FirstOrDefaultAsync());
        }

        [HttpGet("/JobDetail/FindCompanyInfo/{CompanyName}")]
        public async Task<IActionResult> FindCompanyInfo(string CompanyName)
        {
            return Ok(await _context.JobDetail.Where(x => x.CompanyName == CompanyName).OrderByDescending(x => x.CreateDate).FirstOrDefaultAsync());
        }
    }
}

