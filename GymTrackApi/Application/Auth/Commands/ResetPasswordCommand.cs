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
			.Include(user => user.PasswordResetCodes)
			.Include(user => user.RefreshTokens) // for tokens invalidation
			.FirstOrDefaultAsync(
				user => user.PasswordResetCodes.Any(code => code.PasswordResetCode == request.Code),
				cancellationToken)
			.ConfigureAwait(false);

		if (user is null
			|| !user.PasswordResetCodes.First(code => code.PasswordResetCode == request.Code).IsCodeValid(request.Code))
		{
			return new Error();
		}

		user.UpdatePasswordHash(passwordHasher.Hash(request.NewPassword));
		await usersDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}