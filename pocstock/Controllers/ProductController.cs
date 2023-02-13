using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pocstock.Data;
using pocstock.Models;
using pocstock.Services;

namespace pocstock.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            //for (var i = 1; i <= 125; i++)
            //{
            //    var product = new Product
            //    {
            //        Name = "Test" + i,
            //        CostPrice = 1000,
            //        SellingPrice = 2000,
            //        ProductCount = 10,
            //    };

            //    if (i >= 100)
            //    {
            //        product.Deleted = true;
            //    }

            //    await CreateProduct(product);

            //    if (i >= 100)
            //    {
            //        var stock = await _context.Stock.LastAsync();
            //        stock.Deleted = true;
            //        await _context.SaveChangesAsync();
            //    }
            //}

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var CheckProduct = await _context.Product.Where(x => !x.Deleted).FirstOrDefaultAsync(x => x.Name == product.Name);

            if (ModelState.IsValid && CheckProduct == null)
            {
                product.UpdateDate = currentTime;
                var TrashProduct = await _context.Product.FirstOrDefaultAsync(x => x.Deleted && x.Name == product.Name);

                if (TrashProduct == null)
                {
                    await _context.Product.AddAsync(product);
                    await _context.SaveChangesAsync();

                    var ProductID = await _context.Product.FirstAsync(x => !x.Deleted && x.Name == product.Name);

                    await _context.Stock.AddAsync(new Stock
                    {
                        Id = ProductID.Id,
                        Name = ProductID.Name,
                        Number = product.ProductCount > 0 ? product.ProductCount : 0,
                        StockTime = currentTime
                    });

                    await _context.StockHistoryLog.AddAsync(new StockHistoryLog
                    {
                        ProductId = ProductID.Id,
                        ActionId = 1,
                        DateTime = currentTime,
                    });

                    if (product.ProductCount > 0)
                    {
                        await _context.StockHistoryLog.AddAsync(new StockHistoryLog
                        {
                            ProductId = ProductID.Id,
                            ActionId = 2,
                            ActionNumber = product.ProductCount,
                            DateTime = currentTime,
                        });
                    }
                }

                // For Create duplicate Name of Product
                else
                {
                    var NewProduct = new Product
                    {
                        Id = TrashProduct.Id,
                        Name = product.Name,
                        CostPrice = product.CostPrice,
                        SellingPrice = product.SellingPrice
                    };

                    var Stock = await _context.Stock.FindAsync(TrashProduct.Id);
                    if(Stock != null)
                    {
                        Stock.Deleted = false;
                        Stock.Number = product.ProductCount > 0 ? product.ProductCount : 0;
                    }

                    await _context.StockHistoryLog.AddAsync(new StockHistoryLog
                    {
                        ProductId = TrashProduct.Id,
                        ActionId = 1,
                        DateTime = currentTime,
                    });

                    if (product.ProductCount > 0)
                    {
                        await _context.StockHistoryLog.AddAsync(new StockHistoryLog
                        {
                            ProductId = TrashProduct.Id,
                            ActionId = 2,
                            ActionNumber = product.ProductCount,
                            DateTime = currentTime,
                        });
                    }

                    _context.Product.Remove(TrashProduct);
                    await _context.Product.AddAsync(NewProduct);

                    await _context.SaveChangesAsync();

                }

                TempData["ProductSuccess"] = $"<strong>&nbsp;สำเร็จ!!</strong> &nbsp; บันทึกข้อมูลสินค้า <strong>{product.Name}</strong> เรียบร้อยแล้ว";
            }
            else
            {
                if (string.IsNullOrEmpty(product.Name))
                {
                    TempData["ProductDanger"] = $"<strong>&nbsp;เกิดข้อผิดพลาด!!</strong> &nbsp; ไม่สามารถบันทึกข้อมูลได้ โปรดตรวจสอบข้อมูลสินค้าที่กรอกใหม่อีกครั้ง <strong>&nbsp;*กรอกข้อมูลที่สำคัญไม่ครบ &nbsp;*สินค้านี้อาจมีในระบบแล้ว</strong>";
                }
                else
                {
                    TempData["ProductDanger"] = $"<strong>&nbsp;เกิดข้อผิดพลาด!!</strong> &nbsp; ไม่สามารถบันทึกข้อมูล <strong>{product.Name}</strong> ได้ โปรดตรวจสอบข้อมูลสินค้าที่กรอกใหม่อีกครั้ง <strong>&nbsp;*กรอกข้อมูลที่สำคัญไม่ครบ &nbsp;*สินค้านี้อาจมีในระบบแล้ว</strong>";
                }       
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("StockPreview", "Stock");
        }


        // GET
        [HttpGet]
        public async Task<IActionResult> EditProduct(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var service = new Service();
            var DataFromDB = await _context.Product.FindAsync(id);

            if (DataFromDB != null)
            {
                DataFromDB.StringDisplayDate = service.GenerateStringDateTime(DataFromDB.UpdateDate);

                return View(DataFromDB);
            }

            return NotFound();
        }

        //Post
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            // Id != product.id for change price or detail with the same Name
            var CheckName = await _context.Product.Where(x => x.Name == product.Name && x.Id != product.Id && !x.Deleted).Select(x => x.Name).FirstOrDefaultAsync();

            if (ModelState.IsValid && string.IsNullOrEmpty(CheckName))
            {
                var DataFormDb = await _context.Product.FindAsync(product.Id);

                if (DataFormDb == null)
                {
                    return NotFound("Product not found");
                }

                DataFormDb.Name = product.Name;
                DataFormDb.CostPrice = product.CostPrice;
                DataFormDb.SellingPrice = product.SellingPrice;
                DataFormDb.UpdateDate = currentTime;

                TempData["ProductSuccess"] = $"<strong>&nbsp;สำเร็จ!!</strong> &nbsp; แก้ไขข้อมูลสินค้า <strong>{product.Name}</strong> เรียบร้อยแล้ว";
            }
            else
            {
                if (string.IsNullOrEmpty(product.Name))
                {
                    TempData["ProductDanger"] = $"&nbsp;<strong>เกิดข้อผิดพลาด!!</strong> &nbsp; ไม่สามารถแก้ไขข้อมูลได้ โปรดตรวจสอบข้อมูลสินค้าที่กรอกใหม่อีกครั้ง <strong>&nbsp;*กรอกข้อมูลที่สำคัญไม่ครบ &nbsp;*สินค้านี้อาจมีในระบบแล้ว</strong>";
                }
                else
                {
                    TempData["ProductDanger"] = $"&nbsp;<strong>เกิดข้อผิดพลาด!!</strong> &nbsp; ไม่สามารถแก้ไขข้อมูล <strong>{product.Name}</strong> ได้ โปรดตรวจสอบข้อมูลสินค้าที่กรอกใหม่อีกครั้ง <strong>&nbsp;*กรอกข้อมูลที่สำคัญไม่ครบ &nbsp;*สินค้านี้อาจมีในระบบแล้ว</strong>";
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("StockPreview", "Stock");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var DataFromDB = await _context.Product.FindAsync(id);

            if (DataFromDB == null)
            {
                return NotFound();
            }

            return View(DataFromDB);
        }

        [HttpGet]        
        public async Task<IActionResult> Delete(int id)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var ProductFromDB = await _context.Product.FindAsync(id);
            var StockFromDb = await _context.Stock.FindAsync(id);

            if (ProductFromDB != null && StockFromDb != null)
            {
                ProductFromDB.Deleted = true;
                ProductFromDB.UpdateDate = currentTime;

                StockFromDb.Deleted = true;

                await _context.StockHistoryLog.AddAsync(new StockHistoryLog
                {
                    ProductId = ProductFromDB.Id,
                    ActionId = 4,
                    DateTime = currentTime,
                });

                TempData["ProductDelete"] = $"<strong>&nbsp;สำเร็จ!!</strong> &nbsp; ลบข้อมูลสินค้า <strong>{ProductFromDB.Name}</strong> เรียบร้อยแล้ว";
                await _context.SaveChangesAsync();
                return RedirectToAction("StockPreview", "Stock");
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> TrashProduct()
        {
            return View(await _context.Product.Where(x => x.Deleted).OrderByDescending(x => x.UpdateDate).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> RestoreProduct(int id)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var TrashProduct = await _context.Product.FindAsync(id);
            var TrashStock = await _context.Stock.FindAsync(id);

            if (TrashProduct != null && TrashStock != null)
            {
                TrashProduct.Deleted = false;
                TrashProduct.UpdateDate = currentTime;

                TrashStock.Deleted = false;

                await _context.StockHistoryLog.AddAsync(new StockHistoryLog
                {
                    ProductId = id,
                    ActionId = 5,
                    DateTime = currentTime
                });

                TempData["ProductRestore"] = $"<strong>&nbsp;สำเร็จ!!</strong> &nbsp; กู้คืนข้อมูลสินค้า <strong>{TrashProduct.Name}</strong> เรียบร้อยแล้ว";
                await _context.SaveChangesAsync();
                return RedirectToAction("StockPreview", "Stock");
            }

            return NotFound("Restore Product : 404 NotFound");
        }
    }
}