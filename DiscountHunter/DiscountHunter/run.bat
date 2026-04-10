@echo off
chcp 65001 >nul
title İndirim Avcısı - Discount Hunter

echo.
echo ╔══════════════════════════════════════════════════════════╗
echo ║     İNDİRİM AVCISI / DISCOUNT HUNTER                     ║
echo ║     C# Playwright Otomasyon                              ║
echo ╚══════════════════════════════════════════════════════════╝
echo.

cd /d "%~dp0"

REM ══════════════════════════════════════════════════════
REM  1. .NET SDK KONTROLÜ
REM ══════════════════════════════════════════════════════
echo [1/5] .NET SDK kontrol ediliyor...
where dotnet >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo.
    echo  HATA: .NET SDK bulunamadi!
    echo  https://dotnet.microsoft.com/download/dotnet/9.0
    start https://dotnet.microsoft.com/download/dotnet/9.0
    pause
    exit /b 1
)
for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VER=%%i
echo       OK - .NET %DOTNET_VER%
echo.

REM ══════════════════════════════════════════════════════
REM  2. PAKET YUKLEME
REM ══════════════════════════════════════════════════════
echo [2/5] NuGet paketleri yukleniyor...
dotnet restore
echo       OK - Paketler yuklendi
echo.

REM ══════════════════════════════════════════════════════
REM  3. PROJE DERLEME
REM ══════════════════════════════════════════════════════
echo [3/5] Proje derleniyor...
dotnet build --configuration Release
if %ERRORLEVEL% neq 0 (
    echo HATA: Derleme basarisiz!
    pause
    exit /b 1
)
echo       OK - Proje derlendi
echo.

REM ══════════════════════════════════════════════════════
REM  4. PLAYWRIGHT TARAYICI KURULUMU
REM ══════════════════════════════════════════════════════
echo [4/5] Playwright Chromium kuruluyor/kontrol ediliyor...
echo       Bu islem ilk seferde 2-5 dakika surebilir...
echo.

REM Playwright script yolunu bul ve çalıştır
if exist "bin\Release\net9.0\playwright.ps1" (
    echo       playwright.ps1 bulundu: bin\Release\net9.0\
    powershell -ExecutionPolicy Bypass -File "bin\Release\net9.0\playwright.ps1" install chromium
) else if exist "bin\Release\net8.0\playwright.ps1" (
    echo       playwright.ps1 bulundu: bin\Release\net8.0\
    powershell -ExecutionPolicy Bypass -File "bin\Release\net8.0\playwright.ps1" install chromium
) else if exist "bin\Debug\net9.0\playwright.ps1" (
    echo       playwright.ps1 bulundu: bin\Debug\net9.0\
    powershell -ExecutionPolicy Bypass -File "bin\Debug\net9.0\playwright.ps1" install chromium
) else if exist "bin\Debug\net8.0\playwright.ps1" (
    echo       playwright.ps1 bulundu: bin\Debug\net8.0\
    powershell -ExecutionPolicy Bypass -File "bin\Debug\net8.0\playwright.ps1" install chromium
) else (
    echo       playwright.ps1 bulunamadi, dotnet build tekrar yapiliyor...
    dotnet build
    if exist "bin\Debug\net9.0\playwright.ps1" (
        powershell -ExecutionPolicy Bypass -File "bin\Debug\net9.0\playwright.ps1" install chromium
    )
)

if %ERRORLEVEL% neq 0 (
    echo.
    echo  UYARI: Playwright kurulumunda sorun olabilir.
    echo  Devam etmek icin bir tusa basin...
    pause >nul
)
echo.
echo       OK - Playwright hazir
echo.

REM ══════════════════════════════════════════════════════
REM  5. UYGULAMAYI CALISTIR
REM ══════════════════════════════════════════════════════
echo [5/5] Uygulama baslatiliyor...
echo.
echo ╔══════════════════════════════════════════════════════════╗
echo ║     KURULUM TAMAMLANDI - BASLATILIYOR...                 ║
echo ╚══════════════════════════════════════════════════════════╝
echo.

dotnet run --configuration Release --no-build

echo.
echo ══════════════════════════════════════════════════════
echo  Program sonlandi. Kapatmak icin bir tusa basin.
echo ══════════════════════════════════════════════════════
pause >nul
