using Application.Auth.Abstractions;
using Application.Email;
using Application.Persistence;
using Domain.Common.Results;
using Domain.Models.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Auth.Commands;

using ResultType = OneOf<Success, NotFound, UserAlreadyConfirmed>;

public sealed record class SendConfirmationEmailCommand(
	UserId UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class SendConfirmationEmailHandler : IRequestHandler<SendConfirmationEmailCommand, ResultType>
{
	private readonly IUsersDataContext usersDataContext;
	private readonly IEmailConfirmationCodeGenerator emailConfirmationCodeGenerator;
	private readonly IUserEmailSender userEmailSender;

	public SendConfirmationEmailHandler(
		IUsersDataContext usersDataContext,
		IEmailConfirmationCodeGenerator emailConfirmationCodeGenerator,
		IUserEmailSender userEmailSender)
	{
		this.usersDataContext = usersDataContext;
		this.emailConfirmationCodeGenerator = emailConfirmationCodeGenerator;
		this.userEmailSender = userEmailSender;
	}

	public async Task<ResultType> Handle(
		SendConfirmationEmailCommand request,
		CancellationToken cancellationToken)
	{
		var user = await usersDataContext.Users
			.Include(user => user.EmailConfirmationCode)
			.FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken)
			.ConfigureAwait(false);

		if (user is null) return new NotFound();
		if (user.HasConfirmedEmail) return new UserAlreadyConfirmed();

		var confirmation = emailConfirmationCodeGenerator.Generate();
		if (!user.TryUpdateEmailConfirmationCode(confirmation, out var error))
		{
			throw new Exception($"Error when updating confirmation code: {error.Value.ErrorMessage}");
		}

		await userEmailSender
			.SendEmailConfirmationLink(user, user.EmailConfirmationCode!.Data, cancellationToken)
			.ConfigureAwait(false);

		return new Success();
	}
}