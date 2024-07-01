namespace Domain.Contracts;

/// <summary>
/// Represents a contract for a domain event implementation.
/// </summary>
public interface IEvent<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets the unique identifier for the event.
    /// </summary>
    TKey Id { get; }

    /// <summary>
    /// Gets the timestamp when the event occurred.
    /// </summary>
    DateTime OccurredOn { get; }

    TKey UserId { get; } 
}
