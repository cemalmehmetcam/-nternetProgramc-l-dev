using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }
            return View(userList);
        }

        // ADMİN YETKİSİ VER
        [HttpPost]
        public async Task<IActionResult> AssignAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Admin rolü yoksa oluştur
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            await _userManager.AddToRoleAsync(user, "Admin");
            return RedirectToAction("Index");
        }

        // ADMİN YETKİSİNİ AL
        [HttpPost]
        public async Task<IActionResult> RevokeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Kendi yetkini alamazsın
            if (User.Identity.Name == user.UserName)
            {
                TempData["Error"] = "Kendi yetkinizi alamazsınız!";
                return RedirectToAction("Index");
            }

            await _userManager.RemoveFromRoleAsync(user, "Admin");
            return RedirectToAction("Index");
        }

        // KULLANICI SİL
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // KENDİNİ SİLEMEZ KONTROLÜ
                if (User.Identity.Name == user.UserName)
                {
                    TempData["Error"] = "Kendinizi silemezsiniz!";
                    return RedirectToAction("Index");
                }

                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
    }
}