using Domain.Constants;

namespace Domain.DomainEvents.Events;

public sealed class UserRegisteredClientEvent : EventBase
{
    public override string Name { get; set; } = EventConstants.UserRegisteredClient;
}
