@echo off
REM İndirim Avcısı - Windows Başlatma Script'i

echo 🛒 İndirim Avcısı Başlatılıyor...

cd /d "%~dp0"

REM .NET kontrolü
where dotnet >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo ⚠️ .NET SDK bulunamadı!
    echo Lütfen https://dotnet.microsoft.com/download adresinden .NET 8.0 SDK'yı indirin.
    pause
    exit /b 1
)

REM Paketleri yükle
echo 📦 Paketler kontrol ediliyor...
dotnet restore

REM Projeyi derle
echo 🔨 Proje derleniyor...
dotnet build --configuration Release

REM Playwright tarayıcılarını kur (ilk çalıştırmada)
echo 🌐 Playwright kontrol ediliyor...
dotnet tool install --global Microsoft.Playwright.CLI 2>nul
playwright install chromium 2>nul

REM Uygulamayı çalıştır
echo.
echo 🚀 Uygulama başlatılıyor...
echo.
dotnet run --configuration Release

pause
