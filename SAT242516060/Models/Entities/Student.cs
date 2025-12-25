using Attributes;


namespace Models.Entities;

public class Student
{
    public long RowNum { get; set; } // SQL'den gelecek sıra no
    public int Id { get; set; }

    [Title("Öğrenci No")]
    public string StudentNumber { get; set; }

    [Title("Ad")]
    public string FirstName { get; set; }

    [Title("Soyad")]
    public string LastName { get; set; }

    [Title("E-Posta")]
    public string Email { get; set; }

    public bool IsActive { get; set; }
}