namespace Domain.ValueObjects;

/// <summary>
/// Value object representing a phone number
/// Ensures phone number validation and equality
/// </summary>
public sealed class PhoneNumber : IEquatable<PhoneNumber>
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty", nameof(value));

        // Remove all non-numeric characters except +
        var cleaned = new string(value.Where(c => char.IsDigit(c) || c == '+').ToArray());

        if (cleaned.Length < 10)
            throw new ArgumentException("Phone number must contain at least 10 digits", nameof(value));

        Value = cleaned;
    }

    public bool Equals(PhoneNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is PhoneNumber other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
}
