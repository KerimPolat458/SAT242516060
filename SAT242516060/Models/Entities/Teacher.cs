using Attributes;

namespace Models.Entities;

public class Teacher
{
    // Listeleme sırasında sıra numarası göstermek için (SQL'den gelecek)
    public long RowNum { get; set; }

    public int Id { get; set; }

    [Title("Ad")] // Senin Attribute yapın
    public string FirstName { get; set; }

    [Title("Soyad")]
    public string LastName { get; set; }

    [Title("E-Posta")]
    public string Email { get; set; }

    [Title("Ünvan")]
    public string Title { get; set; }
}