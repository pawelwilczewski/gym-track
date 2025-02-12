using Application.Auth.Abstractions;
using Application.Persistence;
using Domain.Common.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Auth.Commands;

using ResultType = OneOf<Success, Error>;

public sealed record class ResetPasswordCommand(
	PasswordResetCode Code,
	Password NewPassword) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, ResultType>
{
	private readonly IUsersDataContext usersDataContext;
	private readonly IPasswordHasher passwordHasher;

	public ResetPasswordHandler(IUsersDataContext usersDataContext, IPasswordHasher passwordHasher)
	{
		this.usersDataContext = usersDataContext;
		this.passwordHasher = passwordHasher;
	}

	public async Task<ResultType> Handle(
		ResetPasswordCommand request,
		CancellationToken cancellationToken)
	{
		var user = await usersDataContext.Users
			.Include(user => user.PasswordResetCode)
			.FirstOrDefaultAsync(
				user => user.PasswordResetCode != null && user.PasswordResetCode.IsCodeValid(request.Code),
				cancellationToken)
			.ConfigureAwait(false);

		if (user is null) return new Error();

		user.UpdatePasswordHash(passwordHasher.Hash(request.NewPassword));
		user.DeletePasswordResetCode();
		await usersDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}