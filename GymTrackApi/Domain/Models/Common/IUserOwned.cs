namespace Domain.Models.Common;

public interface IUserOwned
{
	Guid UserId { get; }
}