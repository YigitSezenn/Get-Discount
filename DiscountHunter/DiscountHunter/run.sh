#!/bin/bash

# İndirim Avcısı - Başlatma Script'i
# Bu script otomatik olarak .NET SDK kontrolü, paket yükleme ve uygulamayı başlatır

echo "🛒 İndirim Avcısı Başlatılıyor..."

# Proje dizinine git
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

# .NET SDK kontrolü
if ! command -v dotnet &> /dev/null; then
    echo "⚠️ .NET SDK bulunamadı. Kurulum yapılıyor..."
    
    # .NET 8.0 SDK kurulumu
    wget -q https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh
    chmod +x /tmp/dotnet-install.sh
    /tmp/dotnet-install.sh --channel 8.0 --install-dir $HOME/.dotnet
    
    export PATH="$HOME/.dotnet:$PATH"
    export DOTNET_ROOT="$HOME/.dotnet"
    
    # PATH'i kalıcı yap
    echo 'export PATH="$HOME/.dotnet:$PATH"' >> ~/.bashrc
    echo 'export DOTNET_ROOT="$HOME/.dotnet"' >> ~/.bashrc
    
    echo "✅ .NET SDK kuruldu: $(dotnet --version)"
fi

# Paketleri yükle
echo "📦 Paketler kontrol ediliyor..."
dotnet restore

# Projeyi derle
echo "🔨 Proje derleniyor..."
dotnet build --configuration Release

# Playwright tarayıcılarını kontrol et ve kur
if [ ! -d "$HOME/.cache/ms-playwright" ]; then
    echo "🌐 Playwright tarayıcıları kuruluyor..."
    dotnet tool install --global Microsoft.Playwright.CLI 2>/dev/null || true
    playwright install chromium 2>/dev/null || dotnet exec bin/Release/net8.0/Microsoft.Playwright.dll install chromium 2>/dev/null || echo "⚠️ Playwright tarayıcıları manuel kurulabilir"
fi

# Uygulamayı çalıştır
echo ""
echo "🚀 Uygulama başlatılıyor..."
echo ""
dotnet run --configuration Release
