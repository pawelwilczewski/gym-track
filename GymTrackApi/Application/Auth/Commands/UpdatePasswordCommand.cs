using Application.Auth.Abstractions;
using Application.Persistence;
using Domain.Common.ValueObjects;
using Domain.Models.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Auth.Commands;

using ResultType = OneOf<Success, Error>;

public sealed record class UpdatePasswordCommand(
	Password OldPassword,
	Password NewPassword,
	UserId UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class UpdatePasswordHandler : IRequestHandler<UpdatePasswordCommand, ResultType>
{
	private readonly IUsersDataContext usersDataContext;
	private readonly IPasswordHasher passwordHasher;
	private readonly IPasswordVerifier passwordVerifier;

	public UpdatePasswordHandler(
		IUsersDataContext usersDataContext,
		IPasswordHasher passwordHasher,
		IPasswordVerifier passwordVerifier)
	{
		this.usersDataContext = usersDataContext;
		this.passwordHasher = passwordHasher;
		this.passwordVerifier = passwordVerifier;
	}

	public async Task<ResultType> Handle(
		UpdatePasswordCommand request,
		CancellationToken cancellationToken)
	{
		var user = await usersDataContext.Users
			.FirstOrDefaultAsync(
				user => user.Id == request.UserId,
				cancellationToken)
			.ConfigureAwait(false);

		if (user is null || !passwordVerifier.Verify(request.OldPassword, user.PasswordHash)) return new Error();

		user.UpdatePasswordHash(passwordHasher.Hash(request.NewPassword));

		// TODO Pawel: consider raising a domain event when password is updated and invalidating refresh tokens then!
		await usersDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}