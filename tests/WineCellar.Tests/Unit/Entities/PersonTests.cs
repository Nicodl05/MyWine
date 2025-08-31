using WineCellar.Core.Entities;
using NUnit.Framework;

namespace WineCellar.Tests.Unit.Entities;

public class PersonTests
{
    [Test]
    public void Person_DefaultInitialization_ShouldHaveEmptyFields()
    {
        // Act
        var person = new Person();

        // Assert
        Assert.That(person.Id, Is.EqualTo(Guid.Empty));
        Assert.That(person.Name, Is.EqualTo(string.Empty));
        Assert.That(person.EncryptedEmail, Is.EqualTo(string.Empty));
        Assert.That(person.PasswordHash, Is.EqualTo(string.Empty));
        Assert.That(person.PreferredVarieties, Is.Not.Null);
        Assert.That(person.PreferredVarieties, Is.Empty);
    }

    [Test]
    public void Person_PreferredVarieties_ShouldAllowAddingVarieties()
    {
        // Arrange
        var person = new Person();

        // Act
        person.PreferredVarieties.Add(Variety.Merlot);
        person.PreferredVarieties.Add(Variety.Chardonnay);

        // Assert
        Assert.That(person.PreferredVarieties, Has.Count.EqualTo(2));
        Assert.That(person.PreferredVarieties, Has.Member(Variety.Merlot));
        Assert.That(person.PreferredVarieties, Has.Member(Variety.Chardonnay));
    }
}
