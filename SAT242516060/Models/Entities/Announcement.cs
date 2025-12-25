using Attributes;


namespace Models.Entities;

public class Announcement
{
    public long RowNum { get; set; }
    public int Id { get; set; }

    [Title("Başlık")]
    public string Title { get; set; }

    [Title("İçerik")]
    public string Content { get; set; }

    [Title("Tarih")]
    public DateTime CreatedDate { get; set; }
}