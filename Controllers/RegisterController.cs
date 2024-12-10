using Microsoft.AspNetCore.Mvc;
using Onboarding.Interfaces;
using Onboarding.Services;
using Onboarding.ViewModels;

namespace Onboarding.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IValidationService _validationService;

        public RegisterController(IValidationService validationService)
        {
            _validationService = validationService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!_validationService.ValidateLoginFormat(model.Login))
            {
                ModelState.AddModelError("Login", "Login must be alphanumeric and have 5-50 characters.");
                return View(model);
            }

            if (!_validationService.ValidatePasswordFormat(model.Password))
            {
                ModelState.AddModelError("Password", "Password must be at least 8 characters long, including a letter, digit, and a special character.");
                return View(model);
            }

            if (!_validationService.ValidateEmailFormat(model.Email))
            {
                ModelState.AddModelError("Email", "Incorrect email format.");
                return View(model);
            }

            var isLoginUnique = await _validationService.IsLoginUniqueAsync(model.Login);

            if (!isLoginUnique)
            {
                ModelState.AddModelError("Login", "Login is already taken.");
                return View(model);
            }

            var isEmailUnique = await _validationService.IsEmailUniqueAsync(model.Email);
            if (!isEmailUnique)
            {
                ModelState.AddModelError("Email", "Email is already taken.");
                return View(model);
            }

            return RedirectToAction("Success");
        }
    }
}
