using WineCellar.Core.Entities;

namespace WineCellar.Tests;

public class NoteTests
{
    [Fact]
    public void Score_WithValidValue_ShouldSetCorrectly()
    {
        var note = new Note { Reviewer = "Test Reviewer" };

        note.Score = 85;

        Assert.Equal(85, note.Score);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void Score_WithValidBoundaryValues_ShouldSetCorrectly(int score)
    {
        var note = new Note();

        note.Score = score;
        Assert.Equal(score, note.Score);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(-10)]
    [InlineData(150)]
    public void Score_WithInvalidValue_ShouldThrowException(int invalidScore)
    {
        var note = new Note();

        Assert.Throws<ArgumentException>(() => note.Score = invalidScore);
    }
}