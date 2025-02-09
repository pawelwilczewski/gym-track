using System.Reflection;
using System.Text.Json;
using Domain.Common;
using Infrastructure.Serialization;
using MassTransit;
using MediatR;

namespace Infrastructure.Outbox;

internal sealed class DomainEventMessageConsumer : IConsumer<IDomainEvent>
{
	private static readonly Dictionary<string, Type?> eventTypes = [];
	private static readonly Assembly domainAssembly = Assembly.GetAssembly(typeof(IDomainEvent))!;

	private readonly IMediator mediator;

	public DomainEventMessageConsumer(IMediator mediator) => this.mediator = mediator;

	public async Task Consume(ConsumeContext<IDomainEvent> context)
	{
		var type = context.SupportedMessageTypes
			.Select(DecodeType)
			.SingleOrDefault(type => type is { IsAbstract: false, IsInterface: false });

		if (type is null) throw new NotImplementedException($"Either {type} has inheritance hierarchy or class name was not found in supported message types!");

		var value = await JsonSerializer.DeserializeAsync(
				context.ReceiveContext.Body.GetStream(),
				type,
				JsonSettings.Options,
				context.CancellationToken)
			.ConfigureAwait(false);

		if (value is null) throw new Exception($"Cannot deserialize to domain event type: {type}.");

		await mediator.Publish(value, context.CancellationToken);
	}

	private static Type? DecodeType(string messageType)
	{
		if (eventTypes.TryGetValue(messageType, out var type)) return type;

		const string prefix = "urn:message:";

		var typeName = messageType[prefix.Length..].Replace(':', '.');
		type = domainAssembly.GetType(typeName);
		eventTypes[messageType] = type;

		return type;
	}
}