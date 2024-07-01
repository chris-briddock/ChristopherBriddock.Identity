using Domain.Constants;

namespace Domain.DomainEvents.Events;

public sealed class UserRemovedClientEvent : EventBase
{
    public override string Name { get; set; } = EventConstants.UserRemovedClient;
}
