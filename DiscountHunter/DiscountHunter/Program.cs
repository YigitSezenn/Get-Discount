using Microsoft.Extensions.Configuration;
using DiscountHunter.Models;
using DiscountHunter.Services;
using Newtonsoft.Json;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Banner
Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════╗
║                                                              ║
║   🛒  İ N D İ R İ M   A V C I S I  🛒                       ║
║                                                              ║
║   C# Playwright Tabanlı İndirim Takip Otomasyonu            ║
║   Desteklenen Siteler: Trendyol, Hepsiburada, Amazon, N11   ║
║                                                              ║
╚══════════════════════════════════════════════════════════════╝
");

// Load configuration
var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// Load or create settings
var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
AppSettings settings;

if (File.Exists(settingsPath))
{
    var json = await File.ReadAllTextAsync(settingsPath);
    settings = JsonConvert.DeserializeObject<AppSettings>(json) ?? CreateDefaultSettings();
    Console.WriteLine("✅ Ayarlar yüklendi: appsettings.json");
}
else
{
    settings = CreateDefaultSettings();
    await SaveSettingsAsync(settings, settingsPath);
    Console.WriteLine("📝 Varsayılan ayarlar oluşturuldu. Lütfen appsettings.json dosyasını düzenleyin.");
}

// Display menu
while (true)
{
    Console.WriteLine("\n📋 MENÜ:");
    Console.WriteLine("1. İndirim Taraması Başlat");
    Console.WriteLine("2. Anahtar Kelime Ekle");
    Console.WriteLine("3. Kategori URL Ekle");
    Console.WriteLine("4. Ürün URL Ekle");
    Console.WriteLine("5. E-posta Ayarlarını Yapılandır");
    Console.WriteLine("6. Takip Listesini Görüntüle");
    Console.WriteLine("7. Zamanlı Çalıştırma");
    Console.WriteLine("8. Çıkış");
    Console.Write("\nSeçiminiz (1-8): ");

    var choice = Console.ReadLine()?.Trim();

    switch (choice)
    {
        case "1":
            await RunScanAsync(settings);
            break;
        case "2":
            await AddKeywordAsync(settings, settingsPath);
            break;
        case "3":
            await AddCategoryAsync(settings, settingsPath);
            break;
        case "4":
            await AddProductUrlAsync(settings, settingsPath);
            break;
        case "5":
            await ConfigureEmailAsync(settings, settingsPath);
            break;
        case "6":
            ShowTrackedItems(settings);
            break;
        case "7":
            await RunScheduledAsync(settings);
            break;
        case "8":
            Console.WriteLine("\n👋 Görüşmek üzere!");
            return;
        default:
            Console.WriteLine("❌ Geçersiz seçim!");
            break;
    }
}

// Helper methods
async Task RunScanAsync(AppSettings settings)
{
    if (!settings.TrackedItems.Any())
    {
        Console.WriteLine("⚠️ Takip listesi boş. Önce anahtar kelime, kategori veya ürün URL ekleyin.");
        return;
    }

    var engine = new DiscountHunterEngine(settings);
    await engine.RunAsync();
}

async Task AddKeywordAsync(AppSettings settings, string path)
{
    Console.Write("\n🔍 Aranacak anahtar kelime: ");
    var keyword = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(keyword)) return;

    Console.Write("Minimum indirim oranı (%) [varsayılan: 20]: ");
    var discountInput = Console.ReadLine()?.Trim();
    var minDiscount = decimal.TryParse(discountInput, out var d) ? d : 20;

    Console.WriteLine("Hangi mağazalarda aransın?");
    Console.WriteLine("1. Tüm mağazalar");
    Console.WriteLine("2. Trendyol");
    Console.WriteLine("3. Hepsiburada");
    Console.WriteLine("4. Amazon TR");
    Console.WriteLine("5. N11");
    Console.Write("Seçim (1-5): ");
    var storeChoice = Console.ReadLine()?.Trim();

    var store = storeChoice switch
    {
        "1" => "all",
        "2" => "trendyol",
        "3" => "hepsiburada",
        "4" => "amazon",
        "5" => "n11",
        _ => "all"
    };

    settings.TrackedItems.Add(new TrackedItem
    {
        Type = TrackingType.Keyword,
        Value = keyword,
        Store = store,
        MinDiscountPercent = minDiscount
    });

    await SaveSettingsAsync(settings, path);
    Console.WriteLine($"✅ '{keyword}' anahtar kelimesi eklendi.");
}

async Task AddCategoryAsync(AppSettings settings, string path)
{
    Console.Write("\n📁 Kategori URL'si: ");
    var url = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(url)) return;

    Console.Write("Minimum indirim oranı (%) [varsayılan: 20]: ");
    var discountInput = Console.ReadLine()?.Trim();
    var minDiscount = decimal.TryParse(discountInput, out var d) ? d : 20;

    var store = DetectStore(url);

    settings.TrackedItems.Add(new TrackedItem
    {
        Type = TrackingType.Category,
        Value = url,
        Store = store,
        MinDiscountPercent = minDiscount
    });

    await SaveSettingsAsync(settings, path);
    Console.WriteLine($"✅ Kategori eklendi: {url}");
}

async Task AddProductUrlAsync(AppSettings settings, string path)
{
    Console.Write("\n🔗 Ürün URL'si: ");
    var url = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(url)) return;

    Console.Write("Minimum indirim oranı (%) [varsayılan: 20]: ");
    var discountInput = Console.ReadLine()?.Trim();
    var minDiscount = decimal.TryParse(discountInput, out var d) ? d : 20;

    var store = DetectStore(url);

    settings.TrackedItems.Add(new TrackedItem
    {
        Type = TrackingType.ProductUrl,
        Value = url,
        Store = store,
        MinDiscountPercent = minDiscount
    });

    await SaveSettingsAsync(settings, path);
    Console.WriteLine($"✅ Ürün eklendi: {url}");
}

async Task ConfigureEmailAsync(AppSettings settings, string path)
{
    Console.WriteLine("\n📧 E-POSTA AYARLARI");
    Console.WriteLine("─────────────────────");
    
    Console.Write("SMTP Sunucusu [smtp.gmail.com]: ");
    var smtp = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(smtp)) settings.Email.SmtpServer = smtp;

    Console.Write("SMTP Port [587]: ");
    var portInput = Console.ReadLine()?.Trim();
    if (int.TryParse(portInput, out var port)) settings.Email.SmtpPort = port;

    Console.Write("Gönderen E-posta: ");
    var sender = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(sender)) settings.Email.SenderEmail = sender;

    Console.Write("Uygulama Şifresi (Gmail için App Password): ");
    var password = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(password)) settings.Email.SenderPassword = password;

    Console.Write("Alıcı E-posta: ");
    var recipient = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(recipient)) settings.Email.RecipientEmail = recipient;

    await SaveSettingsAsync(settings, path);
    Console.WriteLine("✅ E-posta ayarları kaydedildi.");
}

void ShowTrackedItems(AppSettings settings)
{
    Console.WriteLine("\n📋 TAKİP LİSTESİ");
    Console.WriteLine("─────────────────");

    if (!settings.TrackedItems.Any())
    {
        Console.WriteLine("⚠️ Liste boş.");
        return;
    }

    var index = 1;
    foreach (var item in settings.TrackedItems)
    {
        var typeIcon = item.Type switch
        {
            TrackingType.Keyword => "🔍",
            TrackingType.Category => "📁",
            TrackingType.ProductUrl => "🔗",
            _ => "•"
        };
        Console.WriteLine($"{index}. {typeIcon} [{item.Store}] {item.Value} (min %{item.MinDiscountPercent})");
        index++;
    }
}

async Task RunScheduledAsync(AppSettings settings)
{
    Console.Write("\nTarama aralığı (dakika) [60]: ");
    var intervalInput = Console.ReadLine()?.Trim();
    var interval = int.TryParse(intervalInput, out var i) ? i : 60;

    Console.WriteLine($"\n⏰ Zamanlı tarama başlatıldı. Her {interval} dakikada bir tarama yapılacak.");
    Console.WriteLine("Durdurmak için Ctrl+C tuşlarına basın.\n");

    while (true)
    {
        var engine = new DiscountHunterEngine(settings);
        await engine.RunAsync();

        Console.WriteLine($"\n⏳ Sonraki tarama: {DateTime.Now.AddMinutes(interval):HH:mm:ss}");
        await Task.Delay(TimeSpan.FromMinutes(interval));
    }
}

string DetectStore(string url)
{
    var urlLower = url.ToLower();
    if (urlLower.Contains("trendyol")) return "trendyol";
    if (urlLower.Contains("hepsiburada")) return "hepsiburada";
    if (urlLower.Contains("amazon.com.tr")) return "amazon";
    if (urlLower.Contains("n11.com")) return "n11";
    return "all";
}

async Task SaveSettingsAsync(AppSettings settings, string path)
{
    var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
    await File.WriteAllTextAsync(path, json);
}

AppSettings CreateDefaultSettings()
{
    return new AppSettings
    {
        Email = new EmailSettings
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            EnableSsl = true
        },
        Schedule = new ScheduleSettings
        {
            IntervalMinutes = 60,
            RunOnStartup = true
        },
        TrackedItems = new List<TrackedItem>
        {
            new TrackedItem
            {
                Type = TrackingType.Keyword,
                Value = "laptop",
                Store = "all",
                MinDiscountPercent = 20
            }
        }
    };
}
