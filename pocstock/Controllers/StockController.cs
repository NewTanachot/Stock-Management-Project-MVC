using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using pocstock.Data;
using pocstock.Models;
using pocstock.Services;

namespace pocstock.Controllers
{
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> StockPreview(string Toggle = "Disable")
        {
            var dbData = await _context.Stock.Where(x => !x.Deleted).ToListAsync();
            var ProductFromDb = await _context.Product.ToListAsync();
            var Service = new Service();

            foreach (var data in dbData)
            {
                var Product = ProductFromDb.FirstOrDefault(x => x.Id == data.Id);

                if (Product != null)
                {
                    data.Name = Product.Name;
                }

                data.StringDisplayDate = Service.GenerateStringDateTime(data.StockTime);
            }

            TempData["StockActive"] = "active";

            if (Toggle == "Enable")
            {
                return View("StockPreviewEdit", dbData);
            }

            return View(dbData);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStock(string ProductObject)
        {
            if (!string.IsNullOrEmpty(ProductObject))
            {
                DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                List<string> ProductsName = new();

                // Cast JSON to List
                var EditStock = JsonConvert.DeserializeObject<List<EditStock>>(ProductObject);
                EditStock.Reverse();
                var DistinctEditStock = EditStock.DistinctBy(x => x.Id).ToList();

                foreach (var item in DistinctEditStock)
                {
                    var StockProduct = await _context.Stock.FindAsync(item.Id);

                    if (StockProduct != null)
                    {
                        if (item.Count > StockProduct.Number)
                        {
                            await _context.StockHistoryLog.AddAsync(new StockHistoryLog
                            {
                                ProductId = StockProduct.Id,
                                ActionId = 2,
                                ActionNumber = item.Count - StockProduct.Number,
                                DateTime = currentTime,
                            });

                            // Update Stock
                            StockProduct.Number = item.Count;
                        }
                        else if (item.Count < StockProduct.Number)
                        {
                            await _context.StockHistoryLog.AddAsync(new StockHistoryLog
                            {
                                ProductId = StockProduct.Id,
                                ActionId = 3,
                                ActionNumber = item.Count >= 0 ? StockProduct.Number - item.Count : StockProduct.Number,
                                DateTime = currentTime,
                            });

                            // Update Stock
                            if (item.Count > 0)
                            {
                                StockProduct.Number = item.Count;
                            }
                            else
                            {
                                StockProduct.Number = 0;
                            }
                        }

                        StockProduct.StockTime = currentTime;
                    }

                    // Add ProductName
                    ProductsName.Add(await _context.Product.Where(x => x.Id == item.Id).Select(x => x.Name).FirstAsync());
                }

                string StringProductName = $"<strong>&nbsp;สำเร็จ!!</strong> &nbsp; แก้ไขข้อมูลจำสินค้า <strong>";
                ProductsName.ForEach(x => StringProductName += (x + ", "));
                TempData["StockSuccess"] = StringProductName + "</strong> ในสต็อกเรียบร้อยแล้ว";

                await _context.SaveChangesAsync();
            }
           
            return RedirectToAction("StockPreview", new { Toggle = "Disable" });
        }

        [HttpGet]
        public async Task<IActionResult> AddStock(int id)
        {
            var Stock = await _context.Stock.FindAsync(id);

            if (Stock == null)
            {
                return NotFound();
            }

            return View(Stock);
        }

        [HttpPost]
        public async Task<IActionResult> AddStock(int id, int ResultNumber)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var StockProduct = await _context.Stock.FindAsync(id);

            if (StockProduct != null)
            {
                // Update Stock History
                if (StockProduct.Number < ResultNumber)
                {
                    await _context.StockHistoryLog.AddAsync(new StockHistoryLog
                    {
                        ProductId = StockProduct.Id,
                        ActionId = 2,
                        ActionNumber = ResultNumber - StockProduct.Number,
                        DateTime = currentTime,
                    });
                }
                else if (StockProduct.Number > ResultNumber)
                {
                    await _context.StockHistoryLog.AddAsync(new StockHistoryLog
                    {
                        ProductId = StockProduct.Id,
                        ActionId = 3,
                        ActionNumber = StockProduct.Number - ResultNumber,
                        DateTime = currentTime,
                    });
                }

                // Update Stock number and Time
                StockProduct.Number = ResultNumber;
                StockProduct.StockTime = currentTime;
            }

            var ProductName = await _context.Product.Where(x => x.Id == id).Select(x => x.Name).FirstAsync();
            TempData["StockSuccess"] = $"<strong>&nbsp;สำเร็จ!!</strong> &nbsp; แก้ไขข้อมูลจำสินค้า <strong>{ProductName}</strong> ในสต็อกเรียบร้อยแล้ว";

            await _context.SaveChangesAsync();
            return RedirectToAction("StockPreview", new { Toggle = "Disable" });
        }

        [HttpGet("/Stock/FindProductCount/{Id}")]
        public async Task<IActionResult> FindProductCount(int Id)
        {
            var Stock = await _context.Stock.FindAsync(Id);

            if (Stock == null)
            {
                return Ok();
            }

            return Ok(Stock.Number);
        }

        [HttpGet]
        public async Task<IActionResult> TrashProductPreview()
        {
            return View(await _context.Product.Where(x => x.Deleted).OrderByDescending(x => x.UpdateDate).ToListAsync());
        }
    }
}
