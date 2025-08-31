using NUnit.Framework;
using WineCellar.Core.Entities;

namespace WineCellar.Tests.Unit.Entities;

public class WineEntityTests
{
    [Test]
    public void Wine_DefaultInitialization_ShouldHaveEmptyNotesList()
    {
        // Act
        var wine = new Wine();

        // Assert
        Assert.That(wine.Notes, Is.Not.Null);
        Assert.That(wine.Notes, Is.Empty);
        Assert.That(wine.Id, Is.EqualTo(Guid.Empty));
        Assert.That(wine.Name, Is.EqualTo(string.Empty));
        Assert.That(wine.Year, Is.EqualTo(0));
        Assert.That(wine.EstimatedPrice, Is.EqualTo(0m));
        Assert.That(wine.Quantity, Is.EqualTo(0));
        Assert.That(wine.Variety, Is.EqualTo(Variety.None));
        Assert.That(wine.Description, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Wine_WithNotes_ShouldMaintainNotesList()
    {
        // Arrange
        var wine = new Wine { Name = "Test Wine" };
        var note = new Note { Reviewer = "John", Score = 85 };

        // Act
        wine.Notes.Add(note);

        // Assert
        Assert.That(wine.Notes, Has.Count.EqualTo(1));
        Assert.That(wine.Notes.First().Reviewer, Is.EqualTo("John"));
        Assert.That(wine.Notes.First().Score, Is.EqualTo(85));
    }

    [Test]
    public void Wine_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var wine = new Wine();
        var id = Guid.NewGuid();

        // Act
        wine.Id = id;
        wine.Name = "Bordeaux 2020";
        wine.Producer = "Chateau Test";
        wine.Variety = Variety.Medoc;
        wine.Description = "A full bodied red with dark fruit notes";
        wine.Year = 2020;
        wine.Region = "Bordeaux";
        wine.Type = "Red";
        wine.EstimatedPrice = 45.99m;
        wine.Quantity = 5;

        // Assert
        Assert.That(wine.Id, Is.EqualTo(id));
        Assert.That(wine.Name, Is.EqualTo("Bordeaux 2020"));
        Assert.That(wine.Producer, Is.EqualTo("Chateau Test"));
        Assert.That(wine.Variety, Is.EqualTo(Variety.Medoc));
        Assert.That(wine.Description, Is.EqualTo("A full bodied red with dark fruit notes"));
        Assert.That(wine.Year, Is.EqualTo(2020));
        Assert.That(wine.Region, Is.EqualTo("Bordeaux"));
        Assert.That(wine.Type, Is.EqualTo("Red"));
        Assert.That(wine.EstimatedPrice, Is.EqualTo(45.99m));
        Assert.That(wine.Quantity, Is.EqualTo(5));
    }

    [Test]
    public void Wine_WithMultipleNotes_ShouldMaintainOrder()
    {
        // Arrange
        var wine = new Wine { Name = "Test Wine" };
        var note1 = new Note { Reviewer = "Reviewer 1", Score = 85 };
        var note2 = new Note { Reviewer = "Reviewer 2", Score = 90 };

        // Act
        wine.Notes.Add(note1);
        wine.Notes.Add(note2);

        // Assert
        Assert.That(wine.Notes, Has.Count.EqualTo(2));
        Assert.That(wine.Notes.First().Reviewer, Is.EqualTo("Reviewer 1"));
        Assert.That(wine.Notes.Last().Reviewer, Is.EqualTo("Reviewer 2"));
    }

    [Test]
    public void Wine_EmptyStrings_ShouldBeHandledCorrectly()
    {
        // Arrange & Act
        var wine = new Wine
        {
            Name = "",
            Producer = null,
            Region = "   ",
            Type = ""
        };

        // Assert
        Assert.That(wine.Name, Is.EqualTo(""));
        Assert.That(wine.Producer, Is.Null);
        Assert.That(wine.Region, Is.EqualTo("   "));
        Assert.That(wine.Type, Is.EqualTo(""));
    }
}