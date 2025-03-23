using ETicaretData.Helpers;
using ETicaretData.Identity;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using System.Linq;

namespace ETicaretSitesiUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var adminIds = admins.Select(a => a.Id).ToList();
            var users = new List<AppUser>();
            if (adminIds is null)
            {
                users = _userManager.Users.ToList();
            }
            else
            {
                users = _userManager.Users.Where(i => !adminIds.Contains(i.Id)).ToList();
            }
            //foreach (var item in admins)
            //{
            //    users = _userManager.Users.Where(i => !adminIds.Contains(i.Id)).ToList();
            //}
            return View(users);
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> RoleAssign(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var roles = _roleManager.Roles.Where(i => i.Name != "Admin").ToList();
            var userRoles = await _userManager.GetRolesAsync(user);
            var RoleAssigns = new List<RoleAssignModels>();
            roles.ForEach(role => RoleAssigns.Add(new RoleAssignModels
            {
                HasAssign = userRoles.Contains(role.Name),
                Id = role.Id,
                Name = role.Name
            }));
            ViewBag.name = user.Name;
            return View(RoleAssigns);
        }
        [HttpPost]
        public async Task<IActionResult> RoleAssign(List<RoleAssignModels> models, int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            foreach (var role in models)
            {
                if (role.HasAssign)
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }

            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var sonuc=await _userManager.DeleteAsync(user);
            if (sonuc.Succeeded)
            {
                return RedirectToAction("Index");
               
            }
            return NotFound("Silme işlemi başarısız.");
        }
    }
}
