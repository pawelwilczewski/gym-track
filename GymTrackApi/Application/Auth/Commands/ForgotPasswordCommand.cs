using Application.Auth.Abstractions;
using Application.Email;
using Application.Persistence;
using Domain.Common.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands;

public sealed record class ForgotPasswordCommand(
	EmailAddress Email) : IRequest;

// ReSharper disable once UnusedType.Global
internal sealed class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand>
{
	private readonly IUsersDataContext usersDataContext;
	private readonly IUserEmailSender userEmailSender;
	private readonly IPasswordResetCodeGenerator passwordResetCodeGenerator;

	public ForgotPasswordHandler(
		IUsersDataContext usersDataContext,
		IUserEmailSender userEmailSender,
		IPasswordResetCodeGenerator passwordResetCodeGenerator)
	{
		this.usersDataContext = usersDataContext;
		this.userEmailSender = userEmailSender;
		this.passwordResetCodeGenerator = passwordResetCodeGenerator;
	}

	public async Task Handle(
		ForgotPasswordCommand request,
		CancellationToken cancellationToken)
	{
		var user = await usersDataContext.Users
			.Include(user => user.PasswordResetCodes)
			.FirstOrDefaultAsync(user => user.Email == request.Email, cancellationToken)
			.ConfigureAwait(false);

		if (user == null) return;

		var data = passwordResetCodeGenerator.Generate();
		user.AddPasswordResetCode(data);
		await usersDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		await userEmailSender.SendPasswordResetLink(user, data, cancellationToken).ConfigureAwait(false);
	}
}