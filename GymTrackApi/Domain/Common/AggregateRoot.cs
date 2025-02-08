namespace Domain.Common;

public abstract class AggregateRoot
{
	public IEnumerable<IDomainEvent> DomainEvents => domainEvents;
	private readonly List<IDomainEvent> domainEvents = [];

	public void ClearDomainEvents() => domainEvents.Clear();

	protected void Raise(IDomainEvent domainEvent) => domainEvents.Add(domainEvent);
}