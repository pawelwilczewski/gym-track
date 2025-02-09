using Domain.Common;

namespace Domain.Models.User;

public sealed record class UserCreatedEvent : IDomainEvent
{
	public required UserId UserId { get; init; }
}