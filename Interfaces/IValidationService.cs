namespace Onboarding.Interfaces
{
	/// <summary>
	/// This interface is for example register form data validation.
	/// </summary>
	public interface IValidationService
	{
		bool ValidateLoginFormat(string login);
		bool ValidatePasswordFormat(string password);
		bool ValidateEmailFormat(string email);
		async Task<bool> IsLoginUniqueAsync(string login);
		async Task<bool> IsEmailUniqueAsync(string email);
	}
}
