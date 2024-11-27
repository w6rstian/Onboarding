using Onboarding.Interfaces;
using System.Text.RegularExpressions;

namespace Onboarding.Services
{
	/// <summary>
	/// This service is for example register form data validation.
	/// It validates login, password, and email using regex and checks for duplicate usernames and emails.
	/// </summary>
	public class ValidationService : IValidationService
	{
		private IUserRepository _userRepository;

		public ValidationService(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		// Login requirements: (alphanumeric, 5-50 characters)
		bool IValidationService.ValidateLoginFormat(string login)
		{
			var loginRegex = new Regex(@"^[a-zA-Z0-9]{5,50}$");
			return loginRegex.IsMatch(login);
		}

		// Password requirements: ( >= 8 characters, letters, digits, special characters)
		bool IValidationService.ValidatePasswordFormat(string password)
		{
			var passwordRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
			return passwordRegex.IsMatch(password);
		}

		bool IValidationService.ValidateEmailFormat(string email)
		{
			var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
			return emailRegex.IsMatch(email);
		}

		async Task<bool> IValidationService.IsLoginUniqueAsync(string login)
		{
			return !await _userRepository.UserExistsByLoginAsync(login);
		}

		async Task<bool> IValidationService.IsEmailUniqueAsync(string email)
		{
			return !await _userRepository.UserExistsByEmailAsync(email);
		}
	}
}
