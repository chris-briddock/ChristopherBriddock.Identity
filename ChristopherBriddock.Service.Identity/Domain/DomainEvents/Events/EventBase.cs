using Domain.Contracts;

namespace Domain.DomainEvents.Events;

public abstract class EventBase : IEvent<Guid>
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;

    public virtual string Name { get; set; } = default!;

    public Guid UserId { get; set; }
}