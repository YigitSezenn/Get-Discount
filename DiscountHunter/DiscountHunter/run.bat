@echo off
chcp 65001 >nul
title İndirim Avcısı

echo ══════════════════════════════════════════════════════
echo    İNDİRİM AVCISI - C# Playwright Otomasyon
echo ══════════════════════════════════════════════════════
echo.

cd /d "%~dp0"

REM .NET kontrolü
echo [1/5] .NET SDK kontrol ediliyor...
where dotnet >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo.
    echo HATA: .NET SDK bulunamadi!
    echo.
    echo Lutfen asagidaki adresten .NET 8.0+ SDK indirin:
    echo https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
    pause
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VER=%%i
echo    .NET SDK bulundu: %DOTNET_VER%
echo.

REM Paketleri yükle
echo [2/5] NuGet paketleri yukleniyor...
dotnet restore --verbosity quiet
if %ERRORLEVEL% neq 0 (
    echo HATA: Paketler yuklenemedi!
    pause
    exit /b 1
)
echo    Paketler yuklendi.
echo.

REM Projeyi derle
echo [3/5] Proje derleniyor...
dotnet build --configuration Release --verbosity quiet
if %ERRORLEVEL% neq 0 (
    echo HATA: Proje derlenemedi!
    pause
    exit /b 1
)
echo    Proje derlendi.
echo.

REM Playwright tarayıcılarını kur
echo [4/5] Playwright tarayicilari kontrol ediliyor...

REM Playwright script yolunu bul
set "PLAYWRIGHT_SCRIPT="
if exist "bin\Release\net9.0\playwright.ps1" set "PLAYWRIGHT_SCRIPT=bin\Release\net9.0\playwright.ps1"
if exist "bin\Release\net8.0\playwright.ps1" set "PLAYWRIGHT_SCRIPT=bin\Release\net8.0\playwright.ps1"
if exist "bin\Debug\net9.0\playwright.ps1" set "PLAYWRIGHT_SCRIPT=bin\Debug\net9.0\playwright.ps1"
if exist "bin\Debug\net8.0\playwright.ps1" set "PLAYWRIGHT_SCRIPT=bin\Debug\net8.0\playwright.ps1"

REM Chromium kontrolü ve kurulumu
if not exist "%LOCALAPPDATA%\ms-playwright\chromium-*" (
    echo    Playwright Chromium tarayicisi kuruluyor...
    echo    Bu islem birkaç dakika surebilir, lutfen bekleyin...
    echo.
    
    if defined PLAYWRIGHT_SCRIPT (
        powershell -ExecutionPolicy Bypass -File "%PLAYWRIGHT_SCRIPT%" install chromium
    ) else (
        echo    playwright.ps1 bulunamadi, alternatif yontem deneniyor...
        dotnet tool install --global Microsoft.Playwright.CLI 2>nul
        playwright install chromium 2>nul
    )
    
    if %ERRORLEVEL% neq 0 (
        echo.
        echo UYARI: Playwright otomatik kurulamadi.
        echo Lutfen asagidaki komutu manuel calistirin:
        echo.
        echo    powershell -ExecutionPolicy Bypass -File "%PLAYWRIGHT_SCRIPT%" install chromium
        echo.
        pause
    ) else (
        echo    Playwright Chromium kuruldu!
    )
) else (
    echo    Playwright tarayicilari zaten kurulu.
)
echo.

REM Son kontrol
echo [5/5] Baslatma oncesi kontrol...
echo    Hersey hazir!
echo.

echo ══════════════════════════════════════════════════════
echo    UYGULAMA BASLATILIYOR
echo ══════════════════════════════════════════════════════
echo.

REM Uygulamayı çalıştır
dotnet run --configuration Release --no-build

echo.
echo ══════════════════════════════════════════════════════
echo    Program sonlandi. Kapatmak icin bir tusa basin.
echo ══════════════════════════════════════════════════════
pause >nul
