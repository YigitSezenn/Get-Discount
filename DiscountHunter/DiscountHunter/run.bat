@echo off
chcp 65001 >nul
title İndirim Avcısı

echo ══════════════════════════════════════════════════════
echo    İNDİRİM AVCISI - C# Playwright Otomasyon
echo ══════════════════════════════════════════════════════
echo.

cd /d "%~dp0"

REM .NET kontrolü
echo [1/4] .NET SDK kontrol ediliyor...
where dotnet >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo.
    echo HATA: .NET SDK bulunamadi!
    echo.
    echo Lutfen asagidaki adresten .NET 8.0 SDK indirin:
    echo https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
    echo Kurulumdan sonra bu dosyayi tekrar calistirin.
    echo.
    pause
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VER=%%i
echo    .NET SDK bulundu: %DOTNET_VER%
echo.

REM Paketleri yükle
echo [2/4] NuGet paketleri yukleniyor...
dotnet restore --verbosity quiet
if %ERRORLEVEL% neq 0 (
    echo HATA: Paketler yuklenemedi!
    pause
    exit /b 1
)
echo    Paketler yuklendi.
echo.

REM Projeyi derle
echo [3/4] Proje derleniyor...
dotnet build --configuration Release --verbosity quiet
if %ERRORLEVEL% neq 0 (
    echo HATA: Proje derlenemedi!
    pause
    exit /b 1
)
echo    Proje derlendi.
echo.

REM Playwright tarayıcılarını kur
echo [4/4] Playwright tarayicilari kontrol ediliyor...
if not exist "%USERPROFILE%\.cache\ms-playwright" (
    echo    Tarayicilar kuruluyor, bu birkaç dakika surebilir...
    powershell -Command "& {$env:Path += ';' + $env:USERPROFILE + '\.dotnet\tools'; playwright install chromium}" 2>nul
)
echo    Playwright hazir.
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
