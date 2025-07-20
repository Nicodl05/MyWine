namespace WineCellar.Core.Entities;

public class Note
{
    public string Reviewer { get; set; } = string.Empty;
    private int _score;

    public int Score
    {
        get => _score;
        set => _score = value >= 0 && value <= 100 ? value : throw new ArgumentException("Score must be between 0 and 100");
    }
}