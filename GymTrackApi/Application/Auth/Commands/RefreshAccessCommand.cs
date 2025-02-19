using Application.Auth.Abstractions;
using Application.Auth.Dtos;
using Application.Persistence;
using Domain.Common.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Auth.Commands;

using ResultType = OneOf<Success<LogInResponse>, Error>;

public sealed record class RefreshAccessCommand(
	RefreshToken Token,
	string RequestOrigin) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class RefreshAccessHandler : IRequestHandler<RefreshAccessCommand, ResultType>
{
	private readonly IUsersDataContext usersDataContext;
	private readonly IAccessTokenProvider accessTokenProvider;
	private readonly IRefreshTokenProvider refreshTokenProvider;

	public RefreshAccessHandler(
		IUsersDataContext usersDataContext,
		IPasswordVerifier passwordVerifier,
		IAccessTokenProvider accessTokenProvider,
		IRefreshTokenProvider refreshTokenProvider)
	{
		this.usersDataContext = usersDataContext;
		this.accessTokenProvider = accessTokenProvider;
		this.refreshTokenProvider = refreshTokenProvider;
	}

	public async Task<ResultType> Handle(
		RefreshAccessCommand request,
		CancellationToken cancellationToken)
	{
		var user = await usersDataContext.Users
			.Include(user => user.RefreshToken)
			.AsNoTracking()
			.FirstOrDefaultAsync(
				user => user.RefreshToken != null && user.RefreshToken.IsTokenValid(request.Token),
				cancellationToken)
			.ConfigureAwait(false);

		if (user is null) return new Error();

		var refreshToken = refreshTokenProvider.Create();
		user.UpdateRefreshToken(refreshToken);
		await usersDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success<LogInResponse>(new LogInResponse(
			accessTokenProvider.Create(user, request.RequestOrigin).Value,
			refreshToken.Token.Value));
	}
}