using Domain.ValueObjects;
using Xunit;

namespace UnitTests.Domain.ValueObjects;

public class PhoneNumberTests
{
    [Fact]
    public void Constructor_ValidPhoneNumber_CreatesInstance()
    {
        // Arrange
        var phoneNumberString = "+15551234567";

        // Act
        var phoneNumber = new PhoneNumber(phoneNumberString);

        // Assert
        Assert.Equal(phoneNumberString, phoneNumber.Value);
    }

    [Theory]
    [InlineData("+1 (555) 123-4567")]
    [InlineData("555-123-4567")]
    [InlineData("(555) 123-4567")]
    public void Constructor_PhoneNumberWithFormatting_RemovesNonNumericCharacters(string input)
    {
        // Act
        var phoneNumber = new PhoneNumber(input);

        // Assert
        Assert.DoesNotContain("-", phoneNumber.Value);
        Assert.DoesNotContain("(", phoneNumber.Value);
        Assert.DoesNotContain(")", phoneNumber.Value);
        Assert.DoesNotContain(" ", phoneNumber.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_EmptyPhoneNumber_ThrowsArgumentException(string? input)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PhoneNumber(input!));
    }

    [Fact]
    public void Constructor_ShortPhoneNumber_ThrowsArgumentException()
    {
        // Arrange
        var shortNumber = "123456";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PhoneNumber(shortNumber));
    }

    [Fact]
    public void Equals_SamePhoneNumber_ReturnsTrue()
    {
        // Arrange
        var phone1 = new PhoneNumber("+15551234567");
        var phone2 = new PhoneNumber("+15551234567");

        // Act & Assert
        Assert.True(phone1.Equals(phone2));
        Assert.Equal(phone1.GetHashCode(), phone2.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentPhoneNumber_ReturnsFalse()
    {
        // Arrange
        var phone1 = new PhoneNumber("+15551234567");
        var phone2 = new PhoneNumber("+15559876543");

        // Act & Assert
        Assert.False(phone1.Equals(phone2));
    }
}
