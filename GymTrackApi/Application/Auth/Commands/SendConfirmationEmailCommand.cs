using Application.Auth.Abstractions;
using Application.Email;
using Application.Persistence;
using Domain.Models.User;
using Dunet;
using Functional.Monads;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands;

using ResultType = Result<Success, SendConfirmationEmailError>;

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
		CancellationToken cancellationToken) =>
		await Result<User?, SendConfirmationEmailError>.Success.FromAsync(usersDataContext.Users
				.Include(user => user.EmailConfirmationCodes)
				.FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken)
				.ConfigureAwait(false))
			.BindAsync<User?, User, SendConfirmationEmailError>(user => user switch
			{
				null                           => new SendConfirmationEmailError.UserNotFound(),
				_ when !user.HasConfirmedEmail => new SendConfirmationEmailError.UserAlreadyConfirmed(),
				_                              => user
			})
			.MapAsync(async user =>
			{
				var confirmation = emailConfirmationCodeGenerator.Generate();
				user.AddEmailConfirmationCode(confirmation);
				await usersDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
				await userEmailSender.SendEmailConfirmationLink(user, confirmation, cancellationToken).ConfigureAwait(false);
				return new Success();
			});
}

[Union]
public partial record class SendConfirmationEmailError
{
	public sealed partial record UserNotFound;

	public sealed partial record UserAlreadyConfirmed;
}