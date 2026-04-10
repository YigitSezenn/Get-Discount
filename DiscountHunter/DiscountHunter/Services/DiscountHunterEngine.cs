using DiscountHunter.Models;
using Newtonsoft.Json;

namespace DiscountHunter.Services;

public class DiscountHunterEngine
{
    private readonly AppSettings _settings;
    private readonly EmailService _emailService;
    private readonly List<Product> _foundDiscounts = new();
    private readonly string _dataFilePath;

    public DiscountHunterEngine(AppSettings settings)
    {
        _settings = settings;
        _emailService = new EmailService(settings.Email);
        _dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "discount_history.json");
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("🎯 İNDİRİM AVCISI BAŞLATILDI");
        Console.WriteLine($"⏰ Tarih: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
        Console.WriteLine($"📊 Minimum İndirim: %{_settings.TrackedItems.FirstOrDefault()?.MinDiscountPercent ?? 20}");
        Console.WriteLine(new string('=', 60) + "\n");

        _foundDiscounts.Clear();
        var previousDiscounts = LoadPreviousDiscounts();

        foreach (var item in _settings.TrackedItems.Where(i => i.IsActive))
        {
            await ProcessTrackedItemAsync(item);
        }

        // Filter out already notified products
        var newDiscounts = _foundDiscounts
            .Where(p => !previousDiscounts.Any(pd => pd.Url == p.Url && pd.DiscountedPrice == p.DiscountedPrice))
            .ToList();

        // Print summary
        PrintSummary(newDiscounts);

        // Send email notification for new discounts
        if (newDiscounts.Any() && !string.IsNullOrEmpty(_settings.Email.SenderEmail))
        {
            await _emailService.SendDiscountNotificationAsync(newDiscounts);
        }

        // Save found discounts
        SaveDiscounts(_foundDiscounts);
    }

    private async Task ProcessTrackedItemAsync(TrackedItem item)
    {
        var stores = GetStores(item.Store, item.MinDiscountPercent);

        foreach (var scraper in stores)
        {
            await using (scraper)
            {
                ScrapeResult? result = null;

                switch (item.Type)
                {
                    case TrackingType.Keyword:
                        result = await scraper.ScrapeByKeywordAsync(item.Value);
                        break;
                    case TrackingType.Category:
                        result = await scraper.ScrapeByCategoryAsync(item.Value);
                        break;
                    case TrackingType.ProductUrl:
                        if (IsMatchingStore(item.Value, scraper.StoreName))
                        {
                            result = await scraper.ScrapeProductUrlAsync(item.Value);
                        }
                        break;
                }

                if (result?.Success == true && result.Products.Any())
                {
                    _foundDiscounts.AddRange(result.Products);
                }
            }
        }
    }

    private List<BaseScraper> GetStores(string storeFilter, decimal minDiscount)
    {
        var stores = new List<BaseScraper>();
        var filter = storeFilter.ToLower();

        if (filter == "all" || filter.Contains("trendyol"))
            stores.Add(new TrendyolScraper(minDiscount));
        
        if (filter == "all" || filter.Contains("hepsiburada"))
            stores.Add(new HepsiburadaScraper(minDiscount));
        
        if (filter == "all" || filter.Contains("amazon"))
            stores.Add(new AmazonTRScraper(minDiscount));
        
        if (filter == "all" || filter.Contains("n11"))
            stores.Add(new N11Scraper(minDiscount));

        return stores;
    }

    private bool IsMatchingStore(string url, string storeName)
    {
        var urlLower = url.ToLower();
        return storeName.ToLower() switch
        {
            "trendyol" => urlLower.Contains("trendyol"),
            "hepsiburada" => urlLower.Contains("hepsiburada"),
            "amazon tr" => urlLower.Contains("amazon.com.tr"),
            "n11" => urlLower.Contains("n11.com"),
            _ => false
        };
    }

    private void PrintSummary(List<Product> newDiscounts)
    {
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("📊 ÖZET RAPOR");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine($"✅ Toplam Bulunan İndirim: {_foundDiscounts.Count}");
        Console.WriteLine($"🆕 Yeni İndirim: {newDiscounts.Count}");

        if (newDiscounts.Any())
        {
            Console.WriteLine("\n🔥 YENİ İNDİRİMLER:");
            Console.WriteLine(new string('-', 60));
            
            foreach (var group in newDiscounts.GroupBy(p => p.Store))
            {
                Console.WriteLine($"\n📦 {group.Key} ({group.Count()} ürün):");
                foreach (var product in group.Take(5))
                {
                    Console.WriteLine($"   • {Truncate(product.Name, 40)}");
                    Console.WriteLine($"     💰 {product.OriginalPrice:N2} TL → {product.DiscountedPrice:N2} TL (%{product.DiscountPercentage:N0} indirim)");
                }
                if (group.Count() > 5)
                    Console.WriteLine($"   ... ve {group.Count() - 5} ürün daha");
            }
        }
        else
        {
            Console.WriteLine("\n⚠️ Yeni indirim bulunamadı.");
        }

        Console.WriteLine(new string('=', 60) + "\n");
    }

    private string Truncate(string value, int maxLength)
    {
        return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
    }

    private List<Product> LoadPreviousDiscounts()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                var json = File.ReadAllText(_dataFilePath);
                return JsonConvert.DeserializeObject<List<Product>>(json) ?? new List<Product>();
            }
        }
        catch { }
        return new List<Product>();
    }

    private void SaveDiscounts(List<Product> discounts)
    {
        try
        {
            var json = JsonConvert.SerializeObject(discounts, Formatting.Indented);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[HATA] Veri kaydedilemedi: {ex.Message}");
        }
    }
}
