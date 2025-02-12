using Application.Auth.Commands;
using Domain.Models.User;
using MediatR;

namespace Application.Auth.EventHandlers;

// ReSharper disable once UnusedType.Global
internal sealed class UserCreatedSendEmailHandler : INotificationHandler<UserCreatedEvent>
{
	private readonly ISender sender;

	public UserCreatedSendEmailHandler(ISender sender) => this.sender = sender;

	public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
	{
		await sender
			.Send(new SendConfirmationEmailCommand(notification.UserId), cancellationToken)
			.ConfigureAwait(false);
	}
}