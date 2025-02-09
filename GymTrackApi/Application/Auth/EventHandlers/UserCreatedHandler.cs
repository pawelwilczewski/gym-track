using Domain.Models.User;
using MediatR;

namespace Application.Auth.EventHandlers;

// ReSharper disable once UnusedType.Global
internal sealed class UserCreatedHandler : INotificationHandler<UserCreatedEvent>
{
	public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
	{
		Console.Out.WriteLine("User created: {0}", notification.UserId);
		return Task.CompletedTask;
	}
}