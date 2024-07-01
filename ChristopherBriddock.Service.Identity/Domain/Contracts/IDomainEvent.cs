using MediatR;

namespace Domain.Contracts;

// <summary>
/// Represents the interface for an event that is raised within the domain.
/// </summary>
public interface IDomainEvent : INotification
{
}
