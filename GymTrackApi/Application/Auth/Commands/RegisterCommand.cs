using Application.Auth.Abstractions;
using Application.Persistence;
using Domain.Common.Results;
using Domain.Common.ValueObjects;
using Domain.Models.User;
using FuncNet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands;

using ResultType = Result<Success, Error>;

public sealed record class RegisterCommand(
	EmailAddress Email,
	Password Password) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class RegisterHandler : IRequestHandler<RegisterCommand, ResultType>
{
	private readonly IUsersDataContext usersDataContext;
	private readonly IPasswordHasher passwordHasher;

	public RegisterHandler(IUsersDataContext usersDataContext, IPasswordHasher passwordHasher)
	{
		this.usersDataContext = usersDataContext;
		this.passwordHasher = passwordHasher;
	}

	public async Task<ResultType> Handle(
		RegisterCommand request,
		CancellationToken cancellationToken)
	{
		var existingUser = await usersDataContext.Users
			.FirstOrDefaultAsync(user => user.Email == request.Email, cancellationToken)
			.ConfigureAwait(false);

		if (existingUser is not null) return new Error();

		var newUser = User.Create(request.Email, passwordHasher.Hash(request.Password));
		await usersDataContext.Users.AddAsync(newUser, cancellationToken).ConfigureAwait(false);

		await usersDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}