namespace DiscountHunter.Models;

public class Product
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public decimal OriginalPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public string Store { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime FoundAt { get; set; } = DateTime.UtcNow;
    public bool IsNotified { get; set; } = false;
}

public class TrackedItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public TrackingType Type { get; set; }
    public string Value { get; set; } = string.Empty; // URL, category name, or keyword
    public string Store { get; set; } = string.Empty; // trendyol, hepsiburada, amazon, n11, all
    public decimal MinDiscountPercent { get; set; } = 20;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum TrackingType
{
    ProductUrl,
    Category,
    Keyword
}

public class AppSettings
{
    public EmailSettings Email { get; set; } = new();
    public List<TrackedItem> TrackedItems { get; set; } = new();
    public ScheduleSettings Schedule { get; set; } = new();
}

public class EmailSettings
{
    public string SmtpServer { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderPassword { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
}

public class ScheduleSettings
{
    public int IntervalMinutes { get; set; } = 60;
    public bool RunOnStartup { get; set; } = true;
}

public class ScrapeResult
{
    public bool Success { get; set; }
    public List<Product> Products { get; set; } = new();
    public string ErrorMessage { get; set; } = string.Empty;
    public string Store { get; set; } = string.Empty;
}
