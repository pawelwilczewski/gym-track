using Application.Persistence;
using Domain.Common.Results;
using Domain.Common.ValueObjects;
using Domain.Models.User;
using FuncNet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands;

using ResultType = Result<Success, Error>;

public sealed record class ConfirmEmailCommand(
	EmailConfirmationCode Code,
	UserId UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, ResultType>
{
	private readonly IUsersDataContext usersDataContext;

	public ConfirmEmailHandler(IUsersDataContext usersDataContext) => this.usersDataContext = usersDataContext;

	public async Task<ResultType> Handle(
		ConfirmEmailCommand request,
		CancellationToken cancellationToken)
	{
		var user = await usersDataContext.Users
			.Include(user => user.EmailConfirmationCodes)
			.FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken)
			.ConfigureAwait(false);

		if (user is null) return new Error();

		if (user.TryConfirmEmail(request.Code))
		{
			await usersDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
			return new Success();
		}

		return new Error();
	}
}