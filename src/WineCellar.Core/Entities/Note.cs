namespace WineCellar.Core.Entities;

public class Note
{
    private int _score;
    public string Reviewer { get; set; } = string.Empty;

    public int Score
    {
        get => _score;
        set => _score = value >= 0 && value <= 100
            ? value
            : throw new ArgumentException("Score must be between 0 and 100");
    }
}