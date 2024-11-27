namespace Onboarding.Interfaces
{
	/// <summary>
	/// This interface is for example register form data validation.
	/// </summary>
	public interface IUserRepository
	{
		async Task<bool> UserExistsByLoginAsync(string login);
		async Task<bool> UserExistsByEmailAsync(string email);
	}
}
