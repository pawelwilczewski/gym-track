using Domain.Common;

namespace Domain.Models.User;

public sealed record class UserCreatedEvent(UserId UserId) : IDomainEvent;