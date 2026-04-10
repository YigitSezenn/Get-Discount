# İndirim Avcısı - C# Playwright Otomasyon PRD

## Problem Statement
Kullanıcı, Türkiye'nin popüler e-ticaret sitelerinde (Trendyol, Hepsiburada, Amazon TR, N11) indirim yakalayan C# Playwright tabanlı bir otomasyon aracı istedi.

## User Personas
- E-ticaret takipçileri
- İndirim avcıları
- Alışveriş karşılaştırma yapanlar

## Core Requirements (Static)
- 4 e-ticaret sitesi desteği: Trendyol, Hepsiburada, Amazon TR, N11
- 3 takip yöntemi: Anahtar kelime, kategori URL, ürün URL
- E-posta bildirimi (SMTP/Gmail)
- Konsol çıktısı
- %20 ve üzeri indirim filtresi
- Zamanlı otomatik tarama

## What's Been Implemented (10 Nisan 2026)
- ✅ C# .NET 8.0 Console uygulaması oluşturuldu
- ✅ Microsoft.Playwright entegrasyonu
- ✅ MailKit ile e-posta servisi
- ✅ 4 ayrı scraper sınıfı (Trendyol, Hepsiburada, Amazon TR, N11)
- ✅ İnteraktif menü sistemi
- ✅ Anahtar kelime, kategori ve ürün URL takibi
- ✅ Minimum indirim oranı filtreleme
- ✅ JSON formatında ayar ve geçmiş kaydetme
- ✅ HTML formatlı e-posta şablonu
- ✅ Zamanlı çalıştırma özelliği

## Tech Stack
- C# / .NET 8.0
- Microsoft.Playwright (headless browser)
- MailKit (SMTP e-posta)
- Newtonsoft.Json (JSON işleme)
- Microsoft.Extensions.Configuration

## File Structure
```
/app/DiscountHunter/DiscountHunter/
├── Models/Product.cs
├── Services/
│   ├── BaseScraper.cs
│   ├── TrendyolScraper.cs
│   ├── HepsiburadaScraper.cs
│   ├── AmazonTRScraper.cs
│   ├── N11Scraper.cs
│   ├── EmailService.cs
│   └── DiscountHunterEngine.cs
├── Program.cs
├── appsettings.json
└── README.md
```

## P0/P1/P2 Features

### P0 (MVP - Tamamlandı)
- ✅ 4 site scraping
- ✅ E-posta bildirimi
- ✅ Konsol çıktısı
- ✅ Anahtar kelime arama
- ✅ Minimum indirim filtresi

### P1 (Sonraki)
- [ ] Playwright tarayıcı otomatik kurulumu
- [ ] Docker desteği
- [ ] Telegram/Discord bildirimi
- [ ] Web arayüzü

### P2 (Gelecek)
- [ ] Fiyat geçmişi grafikleri
- [ ] Çoklu kullanıcı desteği
- [ ] Proxy rotasyonu
- [ ] CAPTCHA çözme

## Next Tasks
1. Playwright tarayıcılarını kurma rehberi
2. Test senaryoları
3. Docker containerization
