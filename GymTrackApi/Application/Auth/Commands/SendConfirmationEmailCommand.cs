using Application.Auth.Abstractions;
using Application.Email;
using Application.Persistence;
using Domain.Common.Results;
using Domain.Models.User;
using FuncNet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands;

using ResultType = Result<User, NotFound, UserAlreadyConfirmed, EmailSendingError, DatabaseError>;

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
			.Include(user => user.EmailConfirmationCodes)
			.FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken)
			.ConfigureAwait(false);

		if (user is null) return new NotFound();
		if (user.HasConfirmedEmail) return new UserAlreadyConfirmed();

		var confirmation = emailConfirmationCodeGenerator.Generate();
		user.AddEmailConfirmationCode(confirmation);
		await usersDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		await userEmailSender
			.SendEmailConfirmationLink(user, confirmation, cancellationToken)
			.ConfigureAwait(false);

		return user;
	}
}