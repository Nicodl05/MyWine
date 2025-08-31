namespace WineCellar.Core.Entities;

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Store email encrypted at rest. Use a repository/service layer to encrypt/decrypt.
    // Stored as base64 or ciphertext string.
    public string EncryptedEmail { get; set; } = string.Empty;

    // Store password as a secure hash (e.g. bcrypt/argon2) — not the plain password.
    // Keep as string (hash output, e.g. base64) for simplicity.
    public string PasswordHash { get; set; } = string.Empty;

    // Preferred grape varieties (use strings to avoid needing a separate Variety entity).
    public List<Variety> PreferredVarieties { get; set; } = new List<Variety>();
}