using ETicaretDal.Abstract;
using ETicaretDal.Concreate;
using ETicaretData.Context;
using ETicaretData.Entities;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ETicaretSitesiUI.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ETicaretContext _context;
        private readonly ICategoryDal _dal;

        public CategoryController(ETicaretContext context, ICategoryDal dal)
        {
            _context = context;
            _dal = dal;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();

           
            var categoryItems = categories.Select(c => new CategoryItem
            {
                Id = c.Id,
                Name = c.Name,
                Description= c.Description
            }).ToList();

            return View(categoryItems);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int id, [Bind("Id", "Name", "Description", "Image")] Category category)
        {
            if (id != category.Id)
            {
                return RedirectToAction("Error", "Home");
            }

            if (ModelState.IsValid)
            {
                _dal.Add(category);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", category.Id);
            return View(category);

        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0 || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Description")] Category category)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    var databaseCategory = await _context.Categories
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == category.Id);

                    if (databaseCategory == null)
                    {
                        return NotFound();
                    }

                    ModelState.AddModelError(string.Empty, "The record has been modified by another user.");
                    return View(category);
                }
            }
            return View(category);

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var category = await _context.Categories.Include(p => p.Products).FirstOrDefaultAsync(x => x.Id == id);
            //var product = await _contex.Products.FindAsync(id);
            if (category == null)
            {
                return RedirectToAction("Error", "Home");

            }
            //ViewData["CategoryId"] = new SelectList(_contex.Categories, "Id", "Name", product.CategoryId);
            return View(category);

        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return RedirectToAction("Error", "Home");
                //return Problem("Böyle bir ürün yok");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);

            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
