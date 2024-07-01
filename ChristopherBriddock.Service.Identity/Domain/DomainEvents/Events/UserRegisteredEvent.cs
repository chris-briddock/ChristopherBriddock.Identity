using Domain.Constants;

namespace Domain.DomainEvents.Events;

public class UserRegisteredEvent : EventBase
{
    public override string Name { get; set; } = EventConstants.UserRegistered;
}
