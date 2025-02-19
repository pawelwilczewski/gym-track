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
			.Include(user => user.RefreshTokens.Where(token => token.RefreshToken == request.Token))
			.FirstOrDefaultAsync(
				user => user.RefreshTokens.Count > 0,
				cancellationToken)
			.ConfigureAwait(false);

		if (user is null || !user.RefreshTokens.Single().IsTokenValid(request.Token)) return new Error();

		user.RemoveRefreshToken(request.Token);
		var newRefreshToken = refreshTokenProvider.Create();
		user.AddRefreshTokenAndCleanUp(newRefreshToken);
		await usersDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success<LogInResponse>(new LogInResponse(
			accessTokenProvider.Create(user, request.RequestOrigin).Value,
			newRefreshToken.Token.Value));
	}
}