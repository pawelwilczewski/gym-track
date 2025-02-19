using Application.Persistence;
using Domain.Models.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Auth.Commands;

using ResultType = OneOf<Success, NotFound>;

public sealed record class InvalidateRefreshTokensCommand(
	UserId UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class InvalidateRefreshTokensHandler : IRequestHandler<InvalidateRefreshTokensCommand, ResultType>
{
	private readonly IUsersDataContext usersDataContext;

	public InvalidateRefreshTokensHandler(IUsersDataContext usersDataContext) =>
		this.usersDataContext = usersDataContext;

	public async Task<ResultType> Handle(
		InvalidateRefreshTokensCommand request,
		CancellationToken cancellationToken)
	{
		var user = await usersDataContext.Users
			.Include(user => user.RefreshTokens)
			.FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken)
			.ConfigureAwait(false);

		if (user is null) return new NotFound();

		user.InvalidateRefreshTokens();
		await usersDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}