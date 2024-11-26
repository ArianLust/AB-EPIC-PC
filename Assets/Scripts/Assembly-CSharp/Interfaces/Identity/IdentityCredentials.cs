namespace Interfaces.Identity
{
	public class IdentityCredentials
	{
		public string UserName { get; set; }

		public string Password { get; set; }

		public string Email { get; set; }

		public string AvatarAsset { get; set; }

		public bool IsEmpty()
		{
			return string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password);
		}
	}
}
