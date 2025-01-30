using System.ComponentModel.Design;
using Application.Persistence;
using Domain.Models.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Application.Tests.Unit.Mocks;

internal sealed class MockDataContextBuilder
{
	private AppDbContext Context { get; init; } = default!;
	private UserManager<User> UserManager { get; init; } = default!;
	private RoleManager<Role> RoleManager { get; init; } = default!;
	private readonly List<Func<Task>> tasks = [];

	public static MockDataContextBuilder CreateEmpty()
	{
		var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options);

		var builder = new MockDataContextBuilder
		{
			Context = context,
			RoleManager = CreateRoleManager(context),
			UserManager = CreateUserManager(context)
		};

		builder.tasks.Add(async () => await AddDefaultRoles(builder));

		return builder;

		UserManager<User> CreateUserManager(AppDbContext dbContext)
		{
			var userStore = new UserStore<User, Role, AppDbContext, Guid>(dbContext);
			var options = new Mock<IOptions<IdentityOptions>>();
			var idOptions = new IdentityOptions();
			idOptions.Lockout.AllowedForNewUsers = false;
			options.Setup(o => o.Value).Returns(idOptions);
			var userValidators = new List<IUserValidator<User>>();
			var validator = new Mock<IUserValidator<User>>();
			userValidators.Add(validator.Object);
			List<PasswordValidator<User>> passwordValidators = [new()];
			var userManager = new UserManager<User>(userStore, options.Object, new PasswordHasher<User>(),
				userValidators, passwordValidators, new UpperInvariantLookupNormalizer(),
				new IdentityErrorDescriber(), new ServiceContainer(),
				new Mock<ILogger<UserManager<User>>>().Object);
			validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<User>()))
				.Returns(Task.FromResult(IdentityResult.Success))
				.Verifiable();
			return userManager;
		}

		RoleManager<Role> CreateRoleManager(AppDbContext dbContext)
		{
			var roleStore = new RoleStore<Role, AppDbContext, Guid>(dbContext);
			List<IRoleValidator<Role>> validators = [new RoleValidator<Role>()];
			var roleManager = new RoleManager<Role>(roleStore, validators,
				new UpperInvariantLookupNormalizer(),
				new IdentityErrorDescriber(),
				new Mock<ILogger<RoleManager<Role>>>().Object);

			return roleManager;
		}

		static async Task AddDefaultRoles(MockDataContextBuilder builder)
		{
			if (!await builder.RoleManager.RoleExistsAsync(Role.ADMINISTRATOR).ConfigureAwait(false))
			{
				await builder.RoleManager.CreateAsync(new Role(Role.ADMINISTRATOR)).ConfigureAwait(false);
			}
		}
	}

	public MockDataContextBuilder WithUser(IUserInfo userInfo)
	{
		var user = new User
		{
			Id = userInfo.Id,
			Email = userInfo.Email,
			EmailConfirmed = true
		};

		tasks.Add(async () =>
		{
			if (await UserManager.FindByIdAsync(userInfo.Id.ToString()).ConfigureAwait(false) is not null) return;

			var result = await UserManager.CreateAsync(user, userInfo.Password).ConfigureAwait(false);
			await Assert.That(result.Succeeded).IsTrue();

			if (userInfo is AdminInfo)
			{
				result = await UserManager.AddToRoleAsync(user, Role.ADMINISTRATOR).ConfigureAwait(false);
				await Assert.That(result.Succeeded).IsTrue();
			}
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