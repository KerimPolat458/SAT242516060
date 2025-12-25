using Microsoft.EntityFrameworkCore;
using SAT242516060.Data; // ApplicationDbContext için

namespace SAT242516060.Services;

// Loglama Arayüzü
public interface ICustomLogger
{
    Task LogAsync(string tableName, string operation, int recordId, string details);
}

// 19-b: Veritabanına Yazan Logger
public class DbLogger : ICustomLogger
{
    private readonly ApplicationDbContext _context;

    public DbLogger(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(string tableName, string operation, int recordId, string details)
    {
        // Logs_Table tablosuna Raw SQL ile basıyoruz çünkü Entity modelini oluşturmadık
        string sql = "INSERT INTO Logs_Table (TableName, Operation, RecordId, LogDate, Details) VALUES ({0}, {1}, {2}, GETDATE(), {3})";
        await _context.Database.ExecuteSqlRawAsync(sql, tableName, operation, recordId, details);
    }
}

// 19-c: Dosyaya Yazan Logger (Metin Belgesi)
public class FileLogger : ICustomLogger
{
    private readonly string _filePath;

    public FileLogger(IWebHostEnvironment env)
    {
        // Ana dizinde "Logs" klasörü yoksa oluştur
        string folderPath = Path.Combine(env.ContentRootPath, "Logs");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        _filePath = Path.Combine(folderPath, "app_logs.txt");
    }

    public async Task LogAsync(string tableName, string operation, int recordId, string details)
    {
        string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {tableName} - {operation} - ID:{recordId} - {details}{Environment.NewLine}";
        await File.AppendAllTextAsync(_filePath, logLine);
    }
}