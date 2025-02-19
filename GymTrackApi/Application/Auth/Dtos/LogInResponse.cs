namespace Application.Auth.Dtos;

public sealed record class LogInResponse(
	string AccessToken,
	string RefreshToken);