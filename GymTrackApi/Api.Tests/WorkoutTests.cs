using System.Security.Claims;
using Api.Dtos;
using Api.Routes.App.Workouts;
using Application.Persistence;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace Api.Tests;

internal sealed class WorkoutTests
{
	[Before(Test)]
	public void Setup() { }

	[Test]
	public async Task CreateWorkout_AdminWithValidData_ReturnsOk()
	{
		var mockDataContext = new Mock<IDataContext>();

		mockDataContext.Setup(dc => dc.Workouts.Add(It.IsAny<Workout>()));
		mockDataContext.Setup(dc => dc.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

		var userClaims = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, Role.ADMINISTRATOR)]));
		var mockHttpContext = new DefaultHttpContext
		{
			User = userClaims
		};

		var request = new CreateWorkoutRequest("Test Workout");

		var result = await CreateWorkout.Handler(
			mockHttpContext,
			request,
			mockDataContext.Object,
			CancellationToken.None
		);

		await Assert.That(result.Result).IsTypeOf<Ok>();
	}
}