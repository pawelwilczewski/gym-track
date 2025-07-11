using Application.Auth.Abstractions;
using Application.Auth.Dtos;
using Application.Persistence;
using Domain.Common.Results;
using Domain.Common.ValueObjects;
using FuncNet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands;

using ResultType = Result<Success<LogInResponse>, Error>;

public sealed record class LogInCommand(
	EmailAddress Email,
	Password Password,
	string RequestOrigin) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class LogInHandler : IRequestHandler<LogInCommand, ResultType>
{
	private readonly IUsersDataContext usersDataContext;
	private readonly IPasswordVerifier passwordVerifier;
	private readonly IAccessTokenProvider accessTokenProvider;
	private readonly IRefreshTokenProvider refreshTokenProvider;

	public LogInHandler(
		IUsersDataContext usersDataContext,
		IPasswordVerifier passwordVerifier,
		IAccessTokenProvider accessTokenProvider,
		IRefreshTokenProvider refreshTokenProvider)
	{
		this.usersDataContext = usersDataContext;
		this.passwordVerifier = passwordVerifier;
		this.accessTokenProvider = accessTokenProvider;
		this.refreshTokenProvider = refreshTokenProvider;
	}

	public async Task<ResultType> Handle(
		LogInCommand request,
		CancellationToken cancellationToken)
	{
		var user = await usersDataContext.Users
			.FirstOrDefaultAsync(user => user.Email == request.Email, cancellationToken)
			.ConfigureAwait(false);

		if (user is null) return new Error();

		if (!passwordVerifier.Verify(request.Password, user.PasswordHash)) return new Error();

		var newRefreshToken = refreshTokenProvider.Create();
		user.AddRefreshTokenAndCleanUp(newRefreshToken);
		await usersDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success<LogInResponse>(new LogInResponse(
			accessTokenProvider.Create(user, request.RequestOrigin).Value,
			newRefreshToken.Token.Value));
	}
}