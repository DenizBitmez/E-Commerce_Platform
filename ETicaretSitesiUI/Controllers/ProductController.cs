using ETicaretDal.Abstract;
using ETicaretData.Context;
using ETicaretData.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ETicaretSitesiUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        
        private readonly ETicaretContext _contex;
        private readonly IProductDal _productDal;

        public ProductController(ETicaretContext contex, IProductDal productDal)
        {
            _contex = contex;
            _productDal = productDal;
        }
        public async Task<IActionResult> Index()
        {
            var pr = _contex.Products.Include(p => p.Category).ToList();
            return View(pr);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _contex.Products == null)
            {
                return NotFound();
            }
            var product = _productDal.Get(Convert.ToInt32(id));
            if (product == null)
            {
                return NotFound();

            }
            return View(product);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _contex.Products == null)
            {
                return NotFound();
            }
            // await: id gelene kadar beklesin diye kullanıldı
            var product = await _contex.Products.FindAsync(id);
            if (product == null || _contex.Products == null)
            {
                return RedirectToAction("Error", "Home");

            }
            ViewData["CategoryId"] = new SelectList(_contex.Categories, "Id", "Name", product.CategoryId);
            return View(product);

        }
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(_contex.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int id,[Bind("Id", "Name", "Description", "Image", "Stock", "Price", "IsHome", "IsApproved", "CategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return RedirectToAction("Error", "Home");
            }

            if (ModelState.IsValid)
            {
                _productDal.Update(product);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_contex.Categories, "Id", "Name", product.CategoryId);
            return View(product);

        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _contex.Products == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var product = await _contex.Products.Include(p => p.Category).FirstOrDefaultAsync(x => x.Id == id);
            //var product = await _contex.Products.FindAsync(id);
            if (product == null )
            {
                return RedirectToAction("Error", "Home");

            }
            //ViewData["CategoryId"] = new SelectList(_contex.Categories, "Id", "Name", product.CategoryId);
            return View(product);

        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_contex.Products==null)
            {
                return RedirectToAction("Error", "Home");
                //return Problem("Böyle bir ürün yok");
            }
            var product=await _contex.Products.FindAsync( id);
            if(product != null )
            {
                _contex.Products.Remove(product);
               
            }
            await _contex.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

