using Application.Auth.Abstractions;
using Application.Email;
using Application.Persistence;
using Domain.Common.Results;
using Domain.Models.User;
using Functional.Monads;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Application.Auth.Commands;

using ResultType = Result<Success, OneOf<UserNotFound, UserAlreadyConfirmed>>;

public readonly record struct UserNotFound;

public readonly record struct UserAlreadyConfirmed;

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
		await Success<User?>.FromAsync<OneOf<UserNotFound, UserAlreadyConfirmed>>(usersDataContext.Users
				.Include(user => user.EmailConfirmationCodes)
				.FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken)
				.ConfigureAwait(false))
			.BindAsync<User?, User, OneOf<UserNotFound, UserAlreadyConfirmed>>(user => user switch
			{
				null                           => OneOf<UserNotFound, UserAlreadyConfirmed>.FromT0(new UserNotFound()),
				_ when !user.HasConfirmedEmail => OneOf<UserNotFound, UserAlreadyConfirmed>.FromT1(new UserAlreadyConfirmed()),
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