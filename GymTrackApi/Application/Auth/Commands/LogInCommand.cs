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
	string Email,
	string Password) : IRequest<ResultType>;

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
		var emailOrError = EmailAddress.TryFrom(request.Email);
		if (!emailOrError.IsSuccess) return new Error();

		var passwordOrError = Password.TryFrom(request.Password);
		if (!passwordOrError.IsSuccess) return new Error();

		var user = await usersDataContext.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(user => user.Email == emailOrError.ValueObject.Value, cancellationToken)
			.ConfigureAwait(false);

		if (user is null) return new Error();

		return passwordVerifier.Verify(passwordOrError.ValueObject, user.PasswordHash)
			? new Success<JsonWebToken>(tokenProvider.Create(user))
			: new Error();
	}
}