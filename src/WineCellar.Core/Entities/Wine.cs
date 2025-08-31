namespace WineCellar.Core.Entities;

public class Wine
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Producer { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal EstimatedPrice { get; set; }
    public int Quantity { get; set; }
    public List<Note> Notes { get; set; } = new();
    public string Variety { get; set; } = string.Empty; // grape variety
    public string Description { get; set; } = string.Empty; // free text description
}