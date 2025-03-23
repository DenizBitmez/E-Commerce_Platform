using ETicaretDal.Concreate;
using ETicaretData.Context;
using ETicaretData.Entities;
using ETicaretData.Identity;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ETicaretSitesiUI.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ETicaretContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, ETicaretContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult Login()
        {
            if (User.Identity == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {

            //var user = await _userManager.FindByNameAsync(login.UserName);
            //if (user != null)
            //{
            //    var sonuc = await _signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, true);
            //    if (sonuc.Succeeded)
            //    {
            //        return RedirectToAction("Index", "Card");
            //    }
            //}

            ////else if (await _userManager.IsInRoleAsync(user, "Employee"))
            //// {
            ////     // Eğer kullanıcı "Employee" rolündeyse, çalışan paneline yönlendir
            ////     return RedirectToAction("Index", "EmployeeDashboard");
            //// }
            //else if (await _userManager.IsInRoleAsync(user, "Admin"))
            //{
            //    // Eğer kullanıcı "Admin" rolündeyse, admin paneline yönlendir
            //    return RedirectToAction("Index", "Home");
            //}
            //else
            //{
            //    // Diğer kullanıcıları varsayılan sayfaya yönlendir
            //    return RedirectToAction("Index", "Home");
            //}
            //return View(login);


            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user != null)
            {
                var sonuc = await _signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, true);
                if (sonuc.Succeeded)
                {
                    // Kullanıcı başarıyla giriş yaptı, şimdi rolüne göre yönlendirme yap
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        // Eğer kullanıcı "Admin" rolündeyse, admin paneline yönlendir
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // Diğer kullanıcıları varsayılan sayfaya yönlendir
                        return RedirectToAction("Index", "Card");
                    }
                }
            }

            // Giriş başarısız olduysa, hata mesajı göster
            ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
            return View(login);

        }
    
        public async Task<IActionResult> Register()
        {
            if (User.Identity.Name != null)
            {
                return RedirectToAction("Index", "Card");
            }
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel _model)
        {

            var user = new AppUser
            {
                Email = _model.Mail,
                UserName = _model.UserName,
                Name = _model.Name,
                Surname = _model.Surname,
                PasswordHash = _model.Password
            };
            var sonuc = await _userManager.CreateAsync(user, _model.Password);
            await _userManager.AddToRoleAsync(user, "Users");

            if (!sonuc.Succeeded)
            {
                return RedirectToAction("Login");
                //HttpContext.Session.Clear();
            }
            return View(_model);


        }
            public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            
            return RedirectToAction("Login", "Account");
        }
      
        [Authorize]
        public async Task<IActionResult> UserProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Geçersiz kullanıcı ID'si.");
            }

            var user = await _context.Users
                .Where(u => u.Id == userId) 
                .Select(u => new ProfileViewModel
                {
                    Name = u.Name,
                    Email = u.Email,
                    Phone=u.PhoneNumber,
                    Address=u.Address,
                    Orders = _context.Orders
                        .Where(o => o.Id == userId) 
                        .Select(o => new OrderViewModel
                        {
                            OrderId = o.Id,
                            OrderDate = o.OrderDate,
                            orderState = o.orderState,
                            Total = o.Total,
                            ShippingInfo= o.ShippingInfo

                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdString, out int userId))
            {
                var user = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new ProfileViewModel
                    {
                        Name = u.Name,
                        Email = u.Email,
                        Phone = u.PhoneNumber,
                        Address=u.Address
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound("Kullanıcı bulunamadı.");
                }

                return View(user);
            }
            else
            {
                return BadRequest("Geçersiz kullanıcı ID'si.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdString, out int userId))
                {
                    var user = await _context.Users.FindAsync(userId);
                    if (user == null)
                    {
                        return NotFound("Kullanıcı bulunamadı.");
                    }

                    user.Name = model.Name;
                    user.Email = model.Email;
                    user.PhoneNumber = model.Phone;
                    user.Address = model.Address;

                    await _context.SaveChangesAsync();

                    return RedirectToAction("UserProfile");
                }
                else
                {
                    return BadRequest("Geçersiz kullanıcı ID'si.");
                }
            }

            return View(model);
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
