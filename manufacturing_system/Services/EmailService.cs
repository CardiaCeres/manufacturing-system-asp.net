using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManufacturingSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY")
                                           ?? throw new InvalidOperationException("RESEND_API_KEY not set.");
        private readonly HttpClient _httpClient;

        public EmailService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.resend.com")
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task SendResetPasswordEmailAsync(string toEmail, string resetUrl)
        {
            var payload = new
            {
                from = "no-reply@yourapp.com",
                to = toEmail,
                subject = "重設您的密碼",
                html = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.6'>
                    <h2>🔐 重設密碼通知</h2>
                    <p>請點擊下方按鈕設定新密碼：</p>
                    <p><a href='{resetUrl}' style='display:inline-block;padding:10px 20px;background-color:#667eea;color:#fff;text-decoration:none;border-radius:8px;'>重設密碼</a></p>
                </div>"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/emails", content);

            if (!response.IsSuccessStatusCode)
            {
                var respContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Resend API 發信失敗: {response.StatusCode}, {respContent}");
            }
        }
    }
}
