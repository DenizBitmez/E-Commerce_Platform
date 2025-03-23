using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ETicaretSitesiUI.Models;
using ETicaretData.ViewModels;
using ETicaretDal.Abstract;
using ETicaretData.Entities;
using ETicaretDal.Concreate;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ETicaretData.Context;

namespace ETicaretSitesiUI.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICategoryDal _categoryDal;
    private readonly IProductDal _productDal;
    private readonly IReviewDal _reviewDal;
    private readonly ETicaretContext _context;

    public HomeController(ILogger<HomeController> logger, ICategoryDal categoryDal, IProductDal productDal, IReviewDal reviewDal, ETicaretContext context)
    {
        _logger = logger;
        _categoryDal = categoryDal;
        _productDal = productDal;
        _reviewDal = reviewDal;
        _context = context;
    }

    public IActionResult Index()
    {
        var product = _productDal.GetAll(p => p.IsHome && p.IsApproved);
        return View(product);
    }
    [AllowAnonymous]
    public IActionResult List(int? id, string? search, decimal? minPrice, decimal? maxPrice)
    {
        ViewBag.ıd = id;
        var product = _productDal.GetAll(x => x.IsApproved);
        if (id != null)
        {
            product = product.Where(x => x.CategoryId == id).ToList();
        }
        if (!string.IsNullOrEmpty(search))
        {
            product = product.Where(x => x.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        if (minPrice.HasValue)
        {
            product = product.Where(x => x.Price >= minPrice.Value).ToList();
        }

        if (maxPrice.HasValue)
        {
            product = product.Where(x => x.Price <= maxPrice.Value).ToList();
        }

        var models = new ListViewModel()
        {
            Categories = _categoryDal.GetAll(),
            Products = product

        };
        return View(models);
    }
    [AllowAnonymous]
    public IActionResult Details(int id)
    {
        var product = _productDal.Get(id);
        var reviews = _reviewDal.GetReviewsForProduct(id);

        if (product == null)
        {
            return NotFound();
        }

        var productViewModel = new ProductViewModel
        {
            Product = product,
            Reviews = reviews,
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };

        return View(productViewModel);
    }
 // Sadece giriş yapmış kullanıcılar erişebilir
    [HttpPost]
    public async Task<IActionResult> AddReview(int productId, int rating, string comment)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Giriş yapmış kullanıcının ID'si
        var userName = User.Identity.Name; // Kullanıcı adı

        var review = new ReviewViewModel
        {
            ProductId = productId,
            UserId = userId,
            UserName = userName,
            Rating = rating,
            Comment = comment,
            Date = DateTime.Now
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = productId });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


