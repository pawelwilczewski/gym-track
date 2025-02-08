using Application.Persistence;
using Domain.Common.ValueObjects;
using MediatR;

namespace Application.Auth.Commands;

public sealed record class ForgotPasswordCommand(
	EmailAddress Email) : IRequest;

// ReSharper disable once UnusedType.Global
internal sealed class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand>
{
	private readonly IUsersDataContext usersDataContext;

	public ForgotPasswordHandler(IUsersDataContext usersDataContext) => this.usersDataContext = usersDataContext;

	public async Task Handle(
		ForgotPasswordCommand request,
		CancellationToken cancellationToken) =>
		throw new NotImplementedException();
}