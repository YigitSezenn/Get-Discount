# 🛒 İndirim Avcısı / Discount Hunter

**TR:** Türkiye'nin popüler e-ticaret sitelerinde otomatik indirim takibi yapan C# Playwright otomasyon aracı.

**EN:** A C# Playwright automation tool that tracks discounts on Turkey's popular e-commerce sites.

---

## 🎯 Desteklenen Siteler / Supported Sites

- Trendyol
- Hepsiburada  
- Amazon Türkiye
- N11

---

## ✨ Özellikler / Features

| TR | EN |
|---|---|
| Anahtar kelime ile arama | Keyword search |
| Kategori URL takibi | Category URL tracking |
| Ürün URL takibi | Product URL tracking |
| E-posta bildirimi | Email notification |
| Konsol çıktısı | Console output |
| %20+ indirim filtresi | 20%+ discount filter |
| Zamanlı otomatik tarama | Scheduled auto-scan |

---

## 🚀 Kurulum / Installation

### Gereksinimler / Requirements

- **.NET 9.0 SDK** - [İndir / Download](https://dotnet.microsoft.com/download/dotnet/9.0)

### Adım Adım / Step by Step

#### 1. .NET SDK Kurulumu / Install .NET SDK

```
https://dotnet.microsoft.com/download/dotnet/9.0
```

#### 2. Projeyi İndir / Download Project

```bash
git clone https://github.com/KULLANICI_ADI/REPO_ADI.git
cd DiscountHunter/DiscountHunter
```

#### 3. Paketleri Yükle / Install Packages

```bash
dotnet restore
```

#### 4. Playwright Tarayıcısını Kur / Install Playwright Browser

**Windows (PowerShell):**
```powershell
powershell -ExecutionPolicy Bypass -File "bin\Release\net9.0\playwright.ps1" install chromium
```

**Linux/Mac:**
```bash
pwsh bin/Release/net9.0/playwright.ps1 install chromium
```

#### 5. Çalıştır / Run

**Windows:**
```cmd
run.bat
```
veya / or
```cmd
dotnet run
```

**Linux/Mac:**
```bash
./run.sh
```
veya / or
```bash
dotnet run
```

---

## 📧 E-posta Ayarları / Email Settings

`appsettings.json` dosyasını düzenleyin / Edit the file:

```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "xxxx-xxxx-xxxx-xxxx",
    "RecipientEmail": "recipient@email.com",
    "EnableSsl": true
  }
}
```

### Gmail için Uygulama Şifresi / Gmail App Password

1. **TR:** Google Hesabı → Güvenlik → 2 Adımlı Doğrulama → Uygulama Şifreleri
2. **EN:** Google Account → Security → 2-Step Verification → App Passwords

---

## 📋 Kullanım / Usage

Program başlatıldığında menü görüntülenir / Menu appears when started:

```
📋 MENÜ / MENU:
1. İndirim Taraması Başlat / Start Discount Scan
2. Anahtar Kelime Ekle / Add Keyword
3. Kategori URL Ekle / Add Category URL
4. Ürün URL Ekle / Add Product URL
5. E-posta Ayarlarını Yapılandır / Configure Email
6. Takip Listesini Görüntüle / View Tracking List
7. Zamanlı Çalıştırma / Scheduled Run
8. Çıkış / Exit
```

---

## 📁 Proje Yapısı / Project Structure

```
DiscountHunter/
├── Models/
│   └── Product.cs              # Veri modelleri / Data models
├── Services/
│   ├── BaseScraper.cs          # Temel scraper / Base scraper
│   ├── TrendyolScraper.cs      # Trendyol
│   ├── HepsiburadaScraper.cs   # Hepsiburada
│   ├── AmazonTRScraper.cs      # Amazon TR
│   ├── N11Scraper.cs           # N11
│   ├── EmailService.cs         # E-posta / Email
│   └── DiscountHunterEngine.cs # Ana motor / Main engine
├── Program.cs                  # Giriş noktası / Entry point
├── appsettings.json            # Ayarlar / Settings
├── run.bat                     # Windows başlatıcı / Windows launcher
├── run.sh                      # Linux/Mac başlatıcı / Linux/Mac launcher
└── README.md
```

---

## ⚠️ Notlar / Notes

| TR | EN |
|---|---|
| Web sitelerinin yapısı değişebilir | Website structure may change |
| Çok sık tarama IP engellemesine neden olabilir | Frequent scanning may cause IP blocking |
| Gmail için "Uygulama Şifresi" kullanın | Use "App Password" for Gmail |

---

## 🔧 Sorun Giderme / Troubleshooting

### Playwright Hatası / Playwright Error

```
Executable doesn't exist at...
```

**Çözüm / Solution:**
```powershell
powershell -ExecutionPolicy Bypass -File "bin\Release\net9.0\playwright.ps1" install chromium
```

### .NET Bulunamadı / .NET Not Found

**Çözüm / Solution:** .NET 9.0 SDK yükleyin / Install .NET 9.0 SDK

---

## 📝 Lisans / License

MIT License

---

## 👨‍💻 Geliştirici / Developer

C# Playwright Discount Hunter Automation Tool
