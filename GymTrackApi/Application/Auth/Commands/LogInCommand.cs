using Application.Auth.Abstractions;
using Application.Persistence;
using Domain.Common.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Auth.Commands;

using ResultType = OneOf<Success<JsonWebToken>, Error>;

public sealed record class LogInCommand(
	EmailAddress Email,
	Password Password,
	string RequestOrigin) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class LogInHandler : IRequestHandler<LogInCommand, ResultType>
{
	private readonly IUsersDataContext usersDataContext;
	private readonly IPasswordVerifier passwordVerifier;
	private readonly ITokenProvider tokenProvider;

	public LogInHandler(
		IUsersDataContext usersDataContext,
		IPasswordVerifier passwordVerifier,
		ITokenProvider tokenProvider)
	{
		this.usersDataContext = usersDataContext;
		this.passwordVerifier = passwordVerifier;
		this.tokenProvider = tokenProvider;
	}

	public async Task<ResultType> Handle(
		LogInCommand request,
		CancellationToken cancellationToken)
	{
		var user = await usersDataContext.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(user => user.Email == request.Email, cancellationToken)
			.ConfigureAwait(false);

		if (user is null) return new Error();

		return passwordVerifier.Verify(request.Password, user.PasswordHash)
			? new Success<JsonWebToken>(tokenProvider.Create(user, request.RequestOrigin))
			: new Error();
	}
}