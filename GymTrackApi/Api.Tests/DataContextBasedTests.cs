using Api.Tests.Mocks;
using Application.Persistence;

namespace Api.Tests;

internal abstract class DataContextBasedTests
{
	protected IDataContext DataContext { get; private set; } = default!;

	[Before(Test)]
	public async Task SetUpEach()
	{
		DataContext = DataContextMocks.CreateEmpty();
		await SetUpDataContext(DataContext);
	}

	protected virtual Task SetUpDataContext(IDataContext dataContext) => Task.CompletedTask;

	[After(Test)]
	public void CleanUpEach()
	{
		DataContext.Dispose();
	}
}