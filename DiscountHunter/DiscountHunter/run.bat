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
    echo ══════════════════════════════════════════════════════
    echo  HATA: .NET SDK bulunamadi!
    echo ══════════════════════════════════════════════════════
    echo.
    echo  Lutfen .NET 9.0 SDK indirin:
    echo  https://dotnet.microsoft.com/download/dotnet/9.0
    echo.
    echo  Kurulumdan sonra bu dosyayi tekrar calistirin.
    echo ══════════════════════════════════════════════════════
    echo.
    start https://dotnet.microsoft.com/download/dotnet/9.0
    pause
    exit /b 1
)
for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VER=%%i
echo       OK - .NET %DOTNET_VER% bulundu
echo.

REM ══════════════════════════════════════════════════════
REM  2. PAKET YUKLEME (dotnet restore)
REM ══════════════════════════════════════════════════════
echo [2/5] NuGet paketleri yukleniyor...
dotnet restore >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo       Paketler yukleniyor, bekleyin...
    dotnet restore
)
echo       OK - Paketler yuklendi
echo.

REM ══════════════════════════════════════════════════════
REM  3. PROJE DERLEME (dotnet build)
REM ══════════════════════════════════════════════════════
echo [3/5] Proje derleniyor...
dotnet build --configuration Release -v q >nul 2>nul
if %ERRORLEVEL% neq 0 (
    dotnet build --configuration Release
    if %ERRORLEVEL% neq 0 (
        echo HATA: Derleme basarisiz!
        pause
        exit /b 1
    )
)
echo       OK - Proje derlendi
echo.

REM ══════════════════════════════════════════════════════
REM  4. PLAYWRIGHT TARAYICI KURULUMU
REM ══════════════════════════════════════════════════════
echo [4/5] Playwright Chromium kontrol ediliyor...

REM Playwright script yolunu bul
set "PW_SCRIPT="
for %%F in (bin\Release\net9.0\playwright.ps1 bin\Release\net8.0\playwright.ps1 bin\Debug\net9.0\playwright.ps1) do (
    if exist "%%F" set "PW_SCRIPT=%%F"
)

REM Chromium kurulu mu kontrol et
set "CHROMIUM_EXISTS=0"
for /d %%D in ("%LOCALAPPDATA%\ms-playwright\chromium-*") do set "CHROMIUM_EXISTS=1"
for /d %%D in ("%LOCALAPPDATA%\ms-playwright\chromium_headless_shell-*") do (
    if exist "%%D\chrome-headless-shell-win64\chrome-headless-shell.exe" set "CHROMIUM_EXISTS=1"
)

if "%CHROMIUM_EXISTS%"=="0" (
    echo       Playwright Chromium kuruluyor...
    echo       Bu islem 2-5 dakika surebilir, lutfen bekleyin...
    echo.
    
    if defined PW_SCRIPT (
        powershell -ExecutionPolicy Bypass -Command "& {Set-Location '%cd%'; & './%PW_SCRIPT%' install chromium}"
    ) else (
        echo       Alternatif kurulum deneniyor...
        dotnet tool update --global Microsoft.Playwright.CLI >nul 2>nul
        powershell -Command "$env:Path = [System.Environment]::GetEnvironmentVariable('Path','Machine') + ';' + [System.Environment]::GetEnvironmentVariable('Path','User'); playwright install chromium"
    )
    
    if %ERRORLEVEL% neq 0 (
        echo.
        echo ══════════════════════════════════════════════════════
        echo  UYARI: Playwright otomatik kurulamadi!
        echo  Manuel kurulum icin PowerShell'de calistirin:
        echo.
        echo  cd "%cd%"
        echo  powershell -ExecutionPolicy Bypass -File "%PW_SCRIPT%" install chromium
        echo ══════════════════════════════════════════════════════
        echo.
        pause
    ) else (
        echo       OK - Playwright Chromium kuruldu
    )
) else (
    echo       OK - Playwright Chromium zaten kurulu
)
echo.

REM ══════════════════════════════════════════════════════
REM  5. UYGULAMAYI CALISTIR
REM ══════════════════════════════════════════════════════
echo [5/5] Uygulama baslatiliyor...
echo.
echo ╔══════════════════════════════════════════════════════════╗
echo ║     TUM KURULUMLAR TAMAMLANDI - BASLATILIYOR...          ║
echo ╚══════════════════════════════════════════════════════════╝
echo.

dotnet run --configuration Release --no-build

echo.
echo ══════════════════════════════════════════════════════
echo  Program sonlandi. Kapatmak icin bir tusa basin.
echo ══════════════════════════════════════════════════════
pause >nul
