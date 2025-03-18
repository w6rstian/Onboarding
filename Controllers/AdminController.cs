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

        public async Task<IActionResult> ManageUsers(string searchTerm)
        {
            var users = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                users = users.Where(u => u.Email.ToLower().Contains(searchTerm) ||
                                         u.Name.ToLower().Contains(searchTerm) ||
                                         u.Surname.ToLower().Contains(searchTerm));
            }

            var userList = await users.ToListAsync();
            ViewData["SearchTerm"] = searchTerm;

            return View(userList);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToAction("ManageUsers");
        }

        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var isNowy = userRoles.Contains("Nowy");

            var buddies = await _userManager.GetUsersInRoleAsync("Buddy");

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                BuddyId = user.BuddyId,
                AvailableBuddies = isNowy ? buddies.ToList() : new List<User>() // Pokazujemy Buddich tylko dla roli "Nowy"
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (user == null) return NotFound();

            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Email = model.Email;
            user.BuddyId = model.BuddyId;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("ManageUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

    }
}
