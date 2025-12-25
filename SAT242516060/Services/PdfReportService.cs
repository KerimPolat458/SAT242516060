using Models.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace SAT242516060.Services;

public class PdfReportService
{
    // 1. ÖĞRETMEN RAPORU
    public byte[] GenerateTeacherReport(List<Teacher> teachers)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Element(header => ComposeHeader(header, "Öğretmen Listesi Raporu"));
                page.Content().Element(content => ComposeTeacherTable(content, teachers));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    // 2. ÖĞRENCİ RAPORU (İkinci Tablo Şartı İçin)
    public byte[] GenerateStudentReport(List<Student> students)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Element(header => ComposeHeader(header, "Öğrenci Listesi Raporu"));
                page.Content().Element(content => ComposeStudentTable(content, students));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    // --- YARDIMCI METOTLAR (Tasarım) ---

    private void ComposeHeader(IContainer container, string title)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(title).SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                column.Item().Text(text =>
                {
                    text.Span("Oluşturulma Tarihi: ").SemiBold();
                    text.Span($"{DateTime.Now:dd.MM.yyyy HH:mm}");
                });
            });
        });
    }

    private void ComposeTeacherTable(IContainer container, List<Teacher> teachers)
    {
        container.PaddingTop(10).Table(table =>
        {
            // Kolon Tanımları
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(40); // ID
                columns.RelativeColumn();   // Ad
                columns.RelativeColumn();   // Soyad
                columns.RelativeColumn(2);  // Email
                columns.RelativeColumn();   // Ünvan
            });

            // Başlık Satırı
            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("ID");
                header.Cell().Element(CellStyle).Text("Ad");
                header.Cell().Element(CellStyle).Text("Soyad");
                header.Cell().Element(CellStyle).Text("E-Posta");
                header.Cell().Element(CellStyle).Text("Ünvan");

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                }
            });

            // Veri Satırları
            foreach (var item in teachers)
            {
                table.Cell().Element(CellStyle).Text(item.Id.ToString());
                table.Cell().Element(CellStyle).Text(item.FirstName);
                table.Cell().Element(CellStyle).Text(item.LastName);
                table.Cell().Element(CellStyle).Text(item.Email);
                table.Cell().Element(CellStyle).Text(item.Title);

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                }
            }
        });
    }

    private void ComposeStudentTable(IContainer container, List<Student> students)
    {
        container.PaddingTop(10).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn(2);
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("No");
                header.Cell().Element(CellStyle).Text("Ad");
                header.Cell().Element(CellStyle).Text("Soyad");
                header.Cell().Element(CellStyle).Text("E-Posta");

                static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).BorderBottom(1).BorderColor(Colors.Black);
            });

            foreach (var item in students)
            {
                table.Cell().Element(CellStyle).Text(item.StudentNumber);
                table.Cell().Element(CellStyle).Text(item.FirstName);
                table.Cell().Element(CellStyle).Text(item.LastName);
                table.Cell().Element(CellStyle).Text(item.Email);

                static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
            }
        });
    }
}