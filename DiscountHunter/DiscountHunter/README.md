# İndirim Avcısı - C# Playwright Otomasyon

Türkiye'nin en popüler e-ticaret sitelerinde indirim yakalayan C# Playwright tabanlı otomasyon aracı.

## ✨ Özellikler

- **4 Farklı E-Ticaret Sitesi Desteği:**
  - Trendyol
  - Hepsiburada
  - Amazon Türkiye
  - N11

- **3 Farklı Takip Yöntemi:**
  - Anahtar kelime ile arama
  - Kategori URL'si takibi
  - Belirli ürün URL'si takibi

- **Bildirim Seçenekleri:**
  - Konsol çıktısı (detaylı raporlama)
  - E-posta bildirimi (HTML formatlı)

- **Ek Özellikler:**
  - Minimum indirim oranı filtreleme (%20 ve üzeri)
  - Zamanlı otomatik tarama
  - Daha önce bildirilen indirimleri filtreleme
  - JSON formatında veri kaydetme

## 🚀 Kurulum

### Gereksinimler
- .NET 8.0 SDK
- Playwright tarayıcıları

### Adım 1: Projeyi Derleyin
```bash
cd /app/DiscountHunter/DiscountHunter
dotnet build
```

### Adım 2: Playwright Tarayıcılarını Kurun
```bash
dotnet exec bin/Debug/net8.0/playwright.ps1 install chromium
# veya
pwsh bin/Debug/net8.0/playwright.ps1 install chromium
```

### Adım 3: Uygulamayı Çalıştırın
```bash
dotnet run
```

## 📧 E-posta Yapılandırması (Gmail)

1. Gmail hesabınızda 2 adımlı doğrulamayı etkinleştirin
2. Google Hesabı > Güvenlik > Uygulama Şifreleri bölümüne gidin
3. "Posta" ve cihaz seçin, şifre oluşturun
4. Oluşturulan 16 karakterli şifreyi `appsettings.json` dosyasına ekleyin:

```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "sizin-email@gmail.com",
    "SenderPassword": "xxxx-xxxx-xxxx-xxxx",
    "RecipientEmail": "bildirim-alacak@email.com",
    "EnableSsl": true
  }
}
```

## 📋 Kullanım

### İnteraktif Menü
Uygulama çalıştırıldığında interaktif bir menü görüntülenir:

```
📋 MENÜ:
1. İndirim Taraması Başlat
2. Anahtar Kelime Ekle
3. Kategori URL Ekle
4. Ürün URL Ekle
5. E-posta Ayarlarını Yapılandır
6. Takip Listesini Görüntüle
7. Zamanlı Çalıştırma
8. Çıkış
```

### Örnek Kullanımlar

**Anahtar Kelime Ekleme:**
```
Seçim: 2
Aranacak anahtar kelime: iphone 15
Minimum indirim oranı (%): 25
Mağaza: 1 (Tüm mağazalar)
```

**Kategori Takibi:**
```
Seçim: 3
Kategori URL: https://www.trendyol.com/laptop-x-c103108
Minimum indirim oranı (%): 20
```

**Zamanlı Tarama:**
```
Seçim: 7
Tarama aralığı (dakika): 30
```

## 📁 Dosya Yapısı

```
DiscountHunter/
├── Models/
│   └── Product.cs          # Veri modelleri
├── Services/
│   ├── BaseScraper.cs      # Temel scraper sınıfı
│   ├── TrendyolScraper.cs  # Trendyol scraper
│   ├── HepsiburadaScraper.cs
│   ├── AmazonTRScraper.cs
│   ├── N11Scraper.cs
│   ├── EmailService.cs     # E-posta servisi
│   └── DiscountHunterEngine.cs # Ana motor
├── Program.cs              # Giriş noktası
├── appsettings.json        # Ayarlar
└── discount_history.json   # Bulunan indirimler (otomatik oluşturulur)
```

## ⚠️ Notlar

- Web sitelerinin yapısı değişebilir, bu durumda selector'lar güncellenmeli
- Çok sık tarama yapmak IP engellemesine neden olabilir
- E-posta için Gmail kullanıyorsanız "Uygulama Şifresi" kullanmalısınız

## 📝 Lisans

MIT License
