using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Demo1.Models;

namespace Demo1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Net1041Bai3Context _context;

        public ProductsController(Net1041Bai3Context context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string? searchString, decimal? priceFrom, decimal? priceTo, string? sortBy, string? sortOrder)
        {
            // Khởi tạo truy vấn với các sản phẩm và bao gồm danh mục
            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            // Áp dụng bộ lọc tìm kiếm nếu có
            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p => p.ProductName.Contains(searchString));
            }

            // Áp dụng bộ lọc phạm vi giá nếu được cung cấp
            if (priceFrom.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= priceFrom.Value);
            }
            if (priceTo.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= priceTo.Value);
            }

            // Sắp xếp kết quả dựa trên thuộc tính được chọn và thứ tự
            if (!string.IsNullOrEmpty(sortBy))
            {
                productsQuery = sortOrder == "desc"
                    ? sortBy switch
                    {
                        "ProductName" => productsQuery.OrderByDescending(p => p.ProductName),
                        "Price" => productsQuery.OrderByDescending(p => p.Price),
                        "Quantity" => productsQuery.OrderByDescending(p => p.Quantity),
                        "CreatedDate" => productsQuery.OrderByDescending(p => p.CreatedDate),
                        _ => productsQuery
                    }
                    : sortBy switch
                    {
                        "ProductName" => productsQuery.OrderBy(p => p.ProductName),
                        "Price" => productsQuery.OrderBy(p => p.Price),
                        "Quantity" => productsQuery.OrderBy(p => p.Quantity),
                        "CreatedDate" => productsQuery.OrderBy(p => p.CreatedDate),
                        _ => productsQuery
                    };
            }

            // Lưu trữ các cài đặt bộ lọc và sắp xếp hiện tại vào ViewData
            ViewData["SearchString"] = searchString;
            ViewData["PriceFrom"] = priceFrom;
            ViewData["PriceTo"] = priceTo;
            ViewData["SortBy"] = sortBy;
            ViewData["SortOrder"] = sortOrder;

            // Thực hiện truy vấn và lấy danh sách sản phẩm
            var products = await productsQuery.ToListAsync();
            return View(products);
        }



        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Price,Quantity,CategoryId,Brand,Model,ManufactureDate,ExpirationDate,Rating,IsAvailable,CreatedDate,UpdatedDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,Price,Quantity,CategoryId,Brand,Model,ManufactureDate,ExpirationDate,Rating,IsAvailable,CreatedDate,UpdatedDate")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
