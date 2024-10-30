using System.ComponentModel.Design;
using System.Text;
using Application.Persistence;
using Domain.Models.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Api.Tests.Mocks;

internal sealed class MockDataContextBuilder
{
	private const string ADMIN_PASSWORD = "Admin!123";
	private const string USER_PASSWORD = "User!123";

	public static async Task<MockDataContextBuilder> CreateEmpty()
	{
		var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase("GymTrack-Test")
			.Options);

		return new MockDataContextBuilder
		{
			Context = context,
			RoleManager = await CreateRoleManager(context).ConfigureAwait(false),
			UserManager = CreateUserManager(context)
		};

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

		async Task<RoleManager<Role>> CreateRoleManager(AppDbContext dbContext)
		{
			var roleStore = new RoleStore<Role, AppDbContext, Guid>(dbContext);
			List<IRoleValidator<Role>> validators = [new RoleValidator<Role>()];
			var roleManager = new RoleManager<Role>(roleStore, validators,
				MockLookupNormalizer(),
				new IdentityErrorDescriber(),
				new Mock<ILogger<RoleManager<Role>>>().Object);

			// add default roles
			await roleManager.CreateAsync(new Role(Role.ADMINISTRATOR)).ConfigureAwait(false);

			return roleManager;

			static ILookupNormalizer MockLookupNormalizer()
			{
				var normalizerFunc = new Func<string?, string?>(i => i == null ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(i)).ToUpperInvariant());
				var lookupNormalizer = new Mock<ILookupNormalizer>();
				lookupNormalizer.Setup(i => i.NormalizeName(It.IsAny<string?>())).Returns(normalizerFunc);
				lookupNormalizer.Setup(i => i.NormalizeEmail(It.IsAny<string?>())).Returns(normalizerFunc);
				return lookupNormalizer.Object;
			}
		}
	}

	private IDataContext Context { get; init; } = default!;
	private UserManager<User> UserManager { get; init; } = default!;
	private RoleManager<Role> RoleManager { get; init; } = default!;
	private readonly List<Func<Task>> tasks = [];

	public MockDataContextBuilder WithAdminUser(string email)
	{
		var user = new User
		{
			Email = email,
			EmailConfirmed = true
		};

		tasks.Add(async () =>
		{
			var result = await UserManager.CreateAsync(user, ADMIN_PASSWORD).ConfigureAwait(false);
			await Assert.That(result.Succeeded).IsTrue();

			result = await UserManager.AddToRoleAsync(user, Role.ADMINISTRATOR).ConfigureAwait(false);
			await Assert.That(result.Succeeded).IsTrue();
		});

		return this;
	}

	public MockDataContextBuilder WithUser(string email)
	{
		var user = new User
		{
			Email = email,
			EmailConfirmed = true
		};

		tasks.Add(async () =>
		{
			var result = await UserManager.CreateAsync(user, USER_PASSWORD).ConfigureAwait(false);
			await Assert.That(result.Succeeded).IsTrue();
		});

		return this;
	}

	public async Task<IDataContext> Build()
	{
		await Task.WhenAll(tasks.Select(task => task())).ConfigureAwait(false);
		return Context;
	}
}