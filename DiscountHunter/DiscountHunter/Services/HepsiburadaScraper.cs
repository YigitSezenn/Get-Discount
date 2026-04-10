using Microsoft.Playwright;
using DiscountHunter.Models;

namespace DiscountHunter.Services;

public class HepsiburadaScraper : BaseScraper
{
    public override string StoreName => "Hepsiburada";

    public HepsiburadaScraper(decimal minDiscountPercent = 20) : base(minDiscountPercent) { }

    public override async Task<ScrapeResult> ScrapeByKeywordAsync(string keyword)
    {
        var result = new ScrapeResult { Store = StoreName };
        
        try
        {
            await InitBrowserAsync();
            var searchUrl = $"https://www.hepsiburada.com/ara?q={Uri.EscapeDataString(keyword)}";
            
            Console.WriteLine($"[{StoreName}] 🔍 Arama: {keyword}");
            await _page!.GotoAsync(searchUrl, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });
            await _page.WaitForTimeoutAsync(3000);

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
                categoryUrl = $"https://www.hepsiburada.com{categoryUrl}";

            Console.WriteLine($"[{StoreName}] 📁 Kategori: {categoryUrl}");
            await _page!.GotoAsync(categoryUrl, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });
            await _page.WaitForTimeoutAsync(3000);

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
            await _page.WaitForTimeoutAsync(3000);

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
            // Multiple selector attempts for different Hepsiburada layouts
            var productCards = await _page!.QuerySelectorAllAsync("[data-test-id='product-card-item'], .productListContent-item, .moria-ProductCard-gyqBb");
            
            if (productCards.Count == 0)
            {
                productCards = await _page.QuerySelectorAllAsync("li[class*='productListContent']");
            }

            Console.WriteLine($"[{StoreName}] 📦 {productCards.Count} ürün bulundu");

            foreach (var card in productCards.Take(50))
            {
                try
                {
                    var nameEl = await card.QuerySelectorAsync("[data-test-id='product-card-name'], .product-title, h3");
                    var priceEl = await card.QuerySelectorAsync("[data-test-id='price-current-price'], .price-value, .product-price");
                    var originalPriceEl = await card.QuerySelectorAsync("[data-test-id='price-old-price'], .price-old, .product-old-price");
                    var linkEl = await card.QuerySelectorAsync("a");

                    if (nameEl == null || priceEl == null) continue;

                    var name = await nameEl.InnerTextAsync();
                    var discountedPrice = ParsePrice(await priceEl.InnerTextAsync());

                    decimal originalPrice = discountedPrice;
                    if (originalPriceEl != null)
                    {
                        originalPrice = ParsePrice(await originalPriceEl.InnerTextAsync());
                    }

                    if (originalPrice <= discountedPrice) continue;

                    var discountPercent = CalculateDiscountPercent(originalPrice, discountedPrice);
                    if (discountPercent < _minDiscountPercent) continue;

                    var url = "";
                    if (linkEl != null)
                    {
                        var href = await linkEl.GetAttributeAsync("href");
                        url = href?.StartsWith("http") == true ? href : $"https://www.hepsiburada.com{href}";
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
            var nameEl = await _page!.QuerySelectorAsync("h1[data-test-id='title'], #product-name, h1.product-name");
            var priceEl = await _page.QuerySelectorAsync("[data-test-id='price-current-price'], .product-price-container .price-value");
            var originalPriceEl = await _page.QuerySelectorAsync("[data-test-id='price-old-price'], .product-price-container .price-old");

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
