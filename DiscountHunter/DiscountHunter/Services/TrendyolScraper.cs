using Microsoft.Playwright;
using DiscountHunter.Models;

namespace DiscountHunter.Services;

public class TrendyolScraper : BaseScraper
{
    public override string StoreName => "Trendyol";

    public TrendyolScraper(decimal minDiscountPercent = 20) : base(minDiscountPercent) { }

    public override async Task<ScrapeResult> ScrapeByKeywordAsync(string keyword)
    {
        var result = new ScrapeResult { Store = StoreName };
        
        try
        {
            await InitBrowserAsync();
            var searchUrl = $"https://www.trendyol.com/sr?q={Uri.EscapeDataString(keyword)}&qt={Uri.EscapeDataString(keyword)}&st={Uri.EscapeDataString(keyword)}";
            
            Console.WriteLine($"[{StoreName}] 🔍 Arama: {keyword}");
            await _page!.GotoAsync(searchUrl, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });
            await _page.WaitForTimeoutAsync(2000);

            result.Products = await ExtractProductsAsync();
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            Console.WriteLine($"[{StoreName}] ❌ Hata: {ex.Message}");
        }
        
        return result;
    }

    public override async Task<ScrapeResult> ScrapeByCategoryAsync(string categoryUrl)
    {
        var result = new ScrapeResult { Store = StoreName };
        
        try
        {
            await InitBrowserAsync();
            
            if (!categoryUrl.StartsWith("http"))
                categoryUrl = $"https://www.trendyol.com{categoryUrl}";

            Console.WriteLine($"[{StoreName}] 📁 Kategori: {categoryUrl}");
            await _page!.GotoAsync(categoryUrl, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });
            await _page.WaitForTimeoutAsync(2000);

            result.Products = await ExtractProductsAsync();
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            Console.WriteLine($"[{StoreName}] ❌ Hata: {ex.Message}");
        }
        
        return result;
    }

    public override async Task<ScrapeResult> ScrapeProductUrlAsync(string productUrl)
    {
        var result = new ScrapeResult { Store = StoreName };
        
        try
        {
            await InitBrowserAsync();
            Console.WriteLine($"[{StoreName}] 🔗 Ürün: {productUrl}");
            await _page!.GotoAsync(productUrl, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });
            await _page.WaitForTimeoutAsync(2000);

            var product = await ExtractSingleProductAsync(productUrl);
            if (product != null && product.DiscountPercentage >= _minDiscountPercent)
            {
                result.Products.Add(product);
            }
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            Console.WriteLine($"[{StoreName}] ❌ Hata: {ex.Message}");
        }
        
        return result;
    }

    private async Task<List<Product>> ExtractProductsAsync()
    {
        var products = new List<Product>();

        try
        {
            var productCards = await _page!.QuerySelectorAllAsync(".p-card-wrppr");
            Console.WriteLine($"[{StoreName}] 📦 {productCards.Count} ürün bulundu");

            foreach (var card in productCards.Take(50)) // Limit to 50 products
            {
                try
                {
                    var nameEl = await card.QuerySelectorAsync(".prdct-desc-cntnr-name");
                    var priceEl = await card.QuerySelectorAsync(".prc-box-dscntd");
                    var originalPriceEl = await card.QuerySelectorAsync(".prc-box-orgnl");
                    var linkEl = await card.QuerySelectorAsync("a");

                    if (nameEl == null || priceEl == null) continue;

                    var name = await nameEl.InnerTextAsync();
                    var priceText = await priceEl.InnerTextAsync();
                    var discountedPrice = ParsePrice(priceText);

                    decimal originalPrice = discountedPrice;
                    if (originalPriceEl != null)
                    {
                        var origText = await originalPriceEl.InnerTextAsync();
                        originalPrice = ParsePrice(origText);
                    }

                    if (originalPrice <= discountedPrice) continue;

                    var discountPercent = CalculateDiscountPercent(originalPrice, discountedPrice);
                    if (discountPercent < _minDiscountPercent) continue;

                    var url = "";
                    if (linkEl != null)
                    {
                        var href = await linkEl.GetAttributeAsync("href");
                        url = href?.StartsWith("http") == true ? href : $"https://www.trendyol.com{href}";
                    }

                    products.Add(new Product
                    {
                        Name = name.Trim(),
                        Url = url,
                        OriginalPrice = originalPrice,
                        DiscountedPrice = discountedPrice,
                        DiscountPercentage = discountPercent,
                        Store = StoreName,
                        Category = "Arama Sonucu"
                    });
                }
                catch { continue; }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{StoreName}] ⚠️ Ürün çıkarma hatası: {ex.Message}");
        }

        return products;
    }

    private async Task<Product?> ExtractSingleProductAsync(string url)
    {
        try
        {
            var nameEl = await _page!.QuerySelectorAsync(".pr-new-br span");
            var priceEl = await _page.QuerySelectorAsync(".prc-dsc");
            var originalPriceEl = await _page.QuerySelectorAsync(".prc-org");

            if (nameEl == null || priceEl == null) return null;

            var name = await nameEl.InnerTextAsync();
            var discountedPrice = ParsePrice(await priceEl.InnerTextAsync());

            decimal originalPrice = discountedPrice;
            if (originalPriceEl != null)
            {
                originalPrice = ParsePrice(await originalPriceEl.InnerTextAsync());
            }

            if (originalPrice <= discountedPrice) return null;

            return new Product
            {
                Name = name.Trim(),
                Url = url,
                OriginalPrice = originalPrice,
                DiscountedPrice = discountedPrice,
                DiscountPercentage = CalculateDiscountPercent(originalPrice, discountedPrice),
                Store = StoreName,
                Category = "Ürün Sayfası"
            };
        }
        catch
        {
            return null;
        }
    }
}
