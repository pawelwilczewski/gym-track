using Application.Persistence;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Mocks;

internal static class DataContextMocks
{
	public static IDataContext CreateEmpty() => new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
		.UseInMemoryDatabase("GymTrack-Test")
		.Options);
}