using Application.Auth.Abstractions;
using Application.Persistence;
using Domain.Models.User;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Tests.Unit.Mocks;

internal sealed class MockDataContextBuilder
{
	private AppDbContext Context { get; init; } = null!;
	private readonly List<Func<Task>> tasks = [];
	private readonly IPasswordHasher passwordHasher = new PasswordHasher();

	public static MockDataContextBuilder CreateEmpty()
	{
		var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options);

		var builder = new MockDataContextBuilder
		{
			Context = context
		};

		return builder;
	}

	public MockDataContextBuilder WithUser(IUserInfo userInfo)
	{
		var user = User.Create(userInfo.Email, passwordHasher.Hash(userInfo.Password));

		tasks.Add(async () =>
		{
			Context.Users.Add(user);
			await Context.SaveChangesAsync().ConfigureAwait(false);
		});

		return this;
	}

	public MockDataContextBuilder WithEntity(object entity)
	{
		tasks.Add(async () =>
		{
			await Context.AddAsync(entity);
			await Context.SaveChangesAsync();
		});

		return this;
	}

	public async Task<AppDbContext> Build()
	{
		await Task.WhenAll(tasks.Select(task => task())).ConfigureAwait(false);
		return Context;
	}

	public async Task<IUserDataContext> Build(IUserInfo forUser)
	{
		await Task.WhenAll(tasks.Select(task => task())).ConfigureAwait(false);
		var factory = new UserDataContextFactory(Context);

		return factory.ForUser(forUser.Id);
	}
}