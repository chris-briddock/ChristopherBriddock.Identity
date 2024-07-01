using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ValueObjects;

/// <summary>
/// Represents an address value object.
/// </summary>
[ComplexType]
public class Address : IEquatable<Address>
{
    /// <summary>
    /// Gets the street address.
    /// </summary>
    public string Street { get; }

    /// <summary>
    /// Gets the city.
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Gets the state or province.
    /// </summary>
    public string State { get; }

    /// <summary>
    /// Gets the postal code.
    /// </summary>
    public string PostalCode { get; }

    /// <summary>
    /// Gets the country.
    /// </summary>
    public string Country { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Address"/> class.
    /// </summary>
    /// <param name="street">The street address.</param>
    /// <param name="city">The city.</param>
    /// <param name="state">The state or province.</param>
    /// <param name="postalCode">The postal code.</param>
    /// <param name="country">The country.</param>
    public Address(string street,
                   string city,
                   string state,
                   string postalCode,
                   string country)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state ?? throw new ArgumentNullException(nameof(state));
        PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
        Country = country ?? throw new ArgumentNullException(nameof(country));
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj);
    }

    /// <inheritdoc/>
    public bool Equals(Address? other)
    {
        return other! != null! &&
               Street == other.Street &&
               City == other.City &&
               State == other.State &&
               PostalCode == other.PostalCode &&
               Country == other.Country;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Street, City, State, PostalCode, Country);
    }

    /// <summary>
    /// Determines whether two specified instances of <see cref="Address"/> are equal.
    /// </summary>
    /// <param name="left">The first address to compare.</param>
    /// <param name="right">The second address to compare.</param>
    /// <returns>true if the two addresses are equal; otherwise, false.</returns>
    public static bool operator ==(Address left, Address right)
    {
        return EqualityComparer<Address>.Default.Equals(left, right);
    }

    /// <summary>
    /// Determines whether two specified instances of <see cref="Address"/> are not equal.
    /// </summary>
    /// <param name="left">The first address to compare.</param>
    /// <param name="right">The second address to compare.</param>
    /// <returns>true if the two addresses are not equal; otherwise, false.</returns>
    public static bool operator !=(Address left, Address right)
    {
        return !(left == right);
    }
}
