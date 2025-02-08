using Application.Persistence;
using Domain.Common.Results;
using Domain.Models.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Auth.Commands;

using ResultType = OneOf<Success, Unauthorized>;

public sealed record class ConfirmEmailCommand(
	string Code,
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
		throw new NotImplementedException();

		var user = await usersDataContext.Users
			.FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken)
			.ConfigureAwait(false);

		if (user is null) return new Unauthorized();

		if (user.HasConfirmedEmail) return new Success();

		// string? code;
		// try
		// {
		// 	code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
		// }
		// catch (FormatException)
		// {
		// 	return new Unauthorized();
		// }

		user.ConfirmEmail();

		// if (!result.Succeeded)
		// {
		// return new Unauthorized();
		// }

		return new Success();
	}
}