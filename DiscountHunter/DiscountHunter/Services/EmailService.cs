using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using DiscountHunter.Models;

namespace DiscountHunter.Services;

public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(EmailSettings settings)
    {
        _settings = settings;
    }

    public async Task<bool> SendDiscountNotificationAsync(List<Product> discounts)
    {
        if (!discounts.Any()) return true;

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("İndirim Avcısı", _settings.SenderEmail));
            message.To.Add(new MailboxAddress("", _settings.RecipientEmail));
            message.Subject = $"🔥 {discounts.Count} Yeni İndirim Bulundu! - {DateTime.Now:dd.MM.yyyy HH:mm}";

            var bodyBuilder = new BodyBuilder();
            
            // HTML Email Body
            var htmlBody = GenerateHtmlEmail(discounts);
            bodyBuilder.HtmlBody = htmlBody;
            
            // Plain text fallback
            bodyBuilder.TextBody = GenerateTextEmail(discounts);

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, 
                _settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            
            await client.AuthenticateAsync(_settings.SenderEmail, _settings.SenderPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            Console.WriteLine($"[EMAIL] ✅ {discounts.Count} indirim e-posta ile gönderildi: {_settings.RecipientEmail}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EMAIL] ❌ E-posta gönderme hatası: {ex.Message}");
            return false;
        }
    }

    private string GenerateHtmlEmail(List<Product> discounts)
    {
        var productRows = string.Join("", discounts.Select(p => $@"
            <tr style='border-bottom: 1px solid #eee;'>
                <td style='padding: 15px;'>
                    <a href='{p.Url}' style='color: #1a73e8; text-decoration: none; font-weight: bold;'>{p.Name}</a>
                    <br/>
                    <span style='color: #666; font-size: 12px;'>{p.Store} | {p.Category}</span>
                </td>
                <td style='padding: 15px; text-align: right;'>
                    <span style='text-decoration: line-through; color: #999;'>{p.OriginalPrice:N2} TL</span>
                    <br/>
                    <span style='color: #d32f2f; font-weight: bold; font-size: 18px;'>{p.DiscountedPrice:N2} TL</span>
                </td>
                <td style='padding: 15px; text-align: center;'>
                    <span style='background: #4caf50; color: white; padding: 5px 10px; border-radius: 20px; font-weight: bold;'>
                        %{p.DiscountPercentage:N0} İNDİRİM
                    </span>
                </td>
            </tr>"));

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; margin: 0; padding: 20px; background: #f5f5f5; }}
        .container {{ max-width: 700px; margin: 0 auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #ff6b6b 0%, #ff8e53 100%); color: white; padding: 30px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .header p {{ margin: 10px 0 0; opacity: 0.9; }}
        table {{ width: 100%; border-collapse: collapse; }}
        .footer {{ background: #f9f9f9; padding: 20px; text-align: center; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🛒 İndirim Avcısı</h1>
            <p>{discounts.Count} yeni indirim bulundu!</p>
        </div>
        <table>
            <thead>
                <tr style='background: #f5f5f5;'>
                    <th style='padding: 15px; text-align: left;'>Ürün</th>
                    <th style='padding: 15px; text-align: right;'>Fiyat</th>
                    <th style='padding: 15px; text-align: center;'>İndirim</th>
                </tr>
            </thead>
            <tbody>
                {productRows}
            </tbody>
        </table>
        <div class='footer'>
            <p>Bu e-posta İndirim Avcısı otomasyon aracı tarafından gönderilmiştir.</p>
            <p>Tarih: {DateTime.Now:dd.MM.yyyy HH:mm}</p>
        </div>
    </div>
</body>
</html>";
    }

    private string GenerateTextEmail(List<Product> discounts)
    {
        var lines = new List<string>
        {
            "=== İNDİRİM AVCISI ===",
            $"{discounts.Count} yeni indirim bulundu!",
            "",
            "---"
        };

        foreach (var p in discounts)
        {
            lines.Add($"📦 {p.Name}");
            lines.Add($"   Mağaza: {p.Store}");
            lines.Add($"   Eski Fiyat: {p.OriginalPrice:N2} TL");
            lines.Add($"   Yeni Fiyat: {p.DiscountedPrice:N2} TL");
            lines.Add($"   İndirim: %{p.DiscountPercentage:N0}");
            lines.Add($"   Link: {p.Url}");
            lines.Add("---");
        }

        lines.Add($"Tarih: {DateTime.Now:dd.MM.yyyy HH:mm}");
        return string.Join(Environment.NewLine, lines);
    }
}
