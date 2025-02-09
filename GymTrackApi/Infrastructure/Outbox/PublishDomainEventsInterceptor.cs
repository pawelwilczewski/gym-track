using Domain.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Outbox;

internal sealed class PublishDomainEventsInterceptor : ISaveChangesInterceptor
{
	private readonly IServiceProvider services;

	public PublishDomainEventsInterceptor(IServiceProvider services) => this.services = services;

	public async ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData,
		InterceptionResult<int> result,
		CancellationToken cancellationToken)
	{
		var publishEndpoint = services.GetRequiredService<IPublishEndpoint>();

		var aggregateRootsWithEvents = eventData.Context!.ChangeTracker
			.Entries<AggregateRoot>()
			.Select(entry => entry.Entity);

		var domainEvents = new List<IDomainEvent>();
		foreach (var aggregateRoot in aggregateRootsWithEvents)
		{
			domainEvents.AddRange(aggregateRoot.DomainEvents);
			aggregateRoot.ClearDomainEvents();
		}

		foreach (var @event in domainEvents)
		{
			await publishEndpoint.Publish(@event.GetType(), @event, cancellationToken).ConfigureAwait(false);
		}

		return result;
	}
}