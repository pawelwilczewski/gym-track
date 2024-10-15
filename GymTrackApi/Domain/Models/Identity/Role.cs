using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Identity;

public class Role(string name) : IdentityRole<Guid>(name)
{
	public const string ADMINISTRATOR = "Administrator";
}