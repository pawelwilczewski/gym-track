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

public sealed record class RefreshLoginCommand(
	RefreshToken Token,
	string RequestOrigin) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class RefreshLoginHandler : IRequestHandler<RefreshLoginCommand, ResultType>
{
	private readonly IUsersDataContext usersDataContext;
	private readonly IAccessTokenProvider accessTokenProvider;
	private readonly IRefreshTokenProvider refreshTokenProvider;

	public RefreshLoginHandler(
		IUsersDataContext usersDataContext,
		IAccessTokenProvider accessTokenProvider,
		IRefreshTokenProvider refreshTokenProvider)
	{
		this.usersDataContext = usersDataContext;
		this.accessTokenProvider = accessTokenProvider;
		this.refreshTokenProvider = refreshTokenProvider;
	}

	public async Task<ResultType> Handle(
		RefreshLoginCommand request,
		CancellationToken cancellationToken)
	{
		var user = await usersDataContext.Users
			.Include(user => user.RefreshTokens)
			.FirstOrDefaultAsync(
				user => user.RefreshTokens.Any(token => token.RefreshToken == request.Token),
				cancellationToken)
			.ConfigureAwait(false);

		if (user is null
			|| !user.RefreshTokens.First(token => token.RefreshToken == request.Token).IsTokenValid(request.Token))
		{
			return new Error();
		}

		user.RemoveRefreshToken(request.Token);
		var newRefreshToken = refreshTokenProvider.Create();
		user.AddRefreshTokenAndCleanUp(newRefreshToken);
		await usersDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success<LogInResponse>(new LogInResponse(
			accessTokenProvider.Create(user, request.RequestOrigin).Value,
			newRefreshToken.Token.Value));
	}
}