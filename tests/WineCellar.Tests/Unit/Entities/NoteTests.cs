using WineCellar.Core.Entities;
using NUnit.Framework;
namespace WineCellar.Tests.Unit.Entities;

public class NoteTests
{
    [Test]
    public void Note_DefaultInitialization_ShouldHaveEmptyValues()
    {
        // Act
        var note = new Note();

        // Assert
        Assert.That(note.Reviewer, Is.EqualTo(string.Empty));
        Assert.That(note.Score, Is.EqualTo(0));
    }

    [Test]
    public void Score_WithValidValue_ShouldSetCorrectly()
    {
        // Arrange
        var note = new Note { Reviewer = "Test Reviewer" };

        // Act
        note.Score = 85;

        // Assert
        Assert.That(note.Score, Is.EqualTo(85));
    }

    [Theory]
    [TestCase(0)]
    [TestCase(50)]
    [TestCase(100)]
    public void Score_WithValidBoundaryValues_ShouldSetCorrectly(int score)
    {
        // Arrange
        var note = new Note();

        // Act
        note.Score = score;

        // Assert
        Assert.That(note.Score, Is.EqualTo(score));
    }

    [Theory]
    [TestCase(-1)]
    [TestCase(101)]
    [TestCase(-10)]
    [TestCase(150)]
    public void Score_WithInvalidValue_ShouldThrowException(int invalidScore)
    {
        // Arrange
        var note = new Note();

        // Act & Assert
        Assert.That(() => note.Score = invalidScore, Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void Note_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var note = new Note();

        // Act
        note.Reviewer = "Wine Expert";
        note.Score = 92;

        // Assert
        Assert.That(note.Reviewer, Is.EqualTo("Wine Expert"));
        Assert.That(note.Score, Is.EqualTo(92));
    }
}