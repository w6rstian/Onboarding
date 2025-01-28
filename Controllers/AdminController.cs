using Microsoft.AspNetCore.Identity;
using Onboarding.Models;
using Onboarding.ViewModels;
using Onboarding.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Onboarding.Controllers
{
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<User> _userManager;

        public AdminController(RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> ManageRoles()
        {
            // Pobieramy wszystkich użytkowników
            var users = await _userManager.Users.ToListAsync();  // Musimy dodać await, aby wykonywać operację asynchroniczną

            // Pobieramy dostępne role
            var availableRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            // Dla każdego użytkownika pobieramy przypisane role
            var model = new List<ManageRolesViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                model.Add(new ManageRolesViewModel
                {
                    User = user,
                    AvailableRoles = availableRoles,
                    UserRoles = userRoles
                });
            }

            return View(model);
        }

        public async Task<IActionResult> UpdateRoles(int userId, List<string> selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = selectedRoles.Except(currentRoles).ToList();
            var rolesToRemove = currentRoles.Except(selectedRoles).ToList();

            // Dodanie nowych ról
            if (rolesToAdd.Any())
            {
                await _userManager.AddToRolesAsync(user, rolesToAdd);
            }

            // Usunięcie niepotrzebnych ról
            if (rolesToRemove.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            }

            return RedirectToAction("ManageRoles");
        }






        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AdminPanel()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }

    }
}
