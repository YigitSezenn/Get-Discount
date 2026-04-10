using Microsoft.Playwright;
using DiscountHunter.Models;

namespace DiscountHunter.Services;

public abstract class BaseScraper : IAsyncDisposable
{
    protected IPlaywright? _playwright;
    protected IBrowser? _browser;
    protected IPage? _page;
    protected readonly decimal _minDiscountPercent;

    public abstract string StoreName { get; }

    protected BaseScraper(decimal minDiscountPercent = 20)
    {
        _minDiscountPercent = minDiscountPercent;
    }

    protected async Task InitBrowserAsync()
    {
        if (_playwright == null)
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage" }
            });
            _page = await _browser.NewPageAsync();
            
            // Set Turkish locale
            await _page.SetExtraHTTPHeadersAsync(new Dictionary<string, string>
            {
                ["Accept-Language"] = "tr-TR,tr;q=0.9"
            });
        }
    }

    public abstract Task<ScrapeResult> ScrapeByKeywordAsync(string keyword);
    public abstract Task<ScrapeResult> ScrapeByCategoryAsync(string categoryUrl);
    public abstract Task<ScrapeResult> ScrapeProductUrlAsync(string productUrl);

    protected decimal CalculateDiscountPercent(decimal original, decimal discounted)
    {
        if (original <= 0) return 0;
        return Math.Round(((original - discounted) / original) * 100, 2);
    }

    protected decimal ParsePrice(string priceText)
    {
        if (string.IsNullOrEmpty(priceText)) return 0;
        
        // Remove currency symbols and non-numeric characters except comma and dot
        var cleaned = new string(priceText.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
        
        // Handle Turkish format (1.234,56) vs US format (1,234.56)
        if (cleaned.Contains(',') && cleaned.IndexOf(',') > cleaned.LastIndexOf('.'))
        {
            cleaned = cleaned.Replace(".", "").Replace(",", ".");
        }
        else if (cleaned.Contains(','))
        {
            cleaned = cleaned.Replace(",", ".");
        }

        if (decimal.TryParse(cleaned, System.Globalization.NumberStyles.Any, 
            System.Globalization.CultureInfo.InvariantCulture, out var price))
        {
            return price;
        }
        
        return 0;
    }

    public async ValueTask DisposeAsync()
    {
        if (_page != null) await _page.CloseAsync();
        if (_browser != null) await _browser.CloseAsync();
        _playwright?.Dispose();
    }
}
