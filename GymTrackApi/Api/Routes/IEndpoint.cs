namespace Api.Routes;

public interface IEndpoint
{
	IEndpointRouteBuilder Map(IEndpointRouteBuilder builder);
}

public static class EndpointRouteBuilderExtensions
{
	public static IEndpointRouteBuilder Map(this IEndpointRouteBuilder builder, IEndpoint endpoint) =>
		endpoint.Map(builder);
}