using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManufacturingSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly HttpClient _httpClient;

        public EmailService()
        {
            _apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY")
                      ?? throw new InvalidOperationException("❌ RESEND_API_KEY not set in environment variables.");

            _fromEmail = Environment.GetEnvironmentVariable("FROM_EMAIL")
                         ?? throw new InvalidOperationException("❌ FROM_EMAIL not set in environment variables.");

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.resend.com")
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // 發送重設密碼信
        public async Task SendResetPasswordEmailAsync(string toEmail, string resetUrl)
        {
            string htmlContent = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.6'>
                    <h2>🔐 重設密碼通知</h2>
                    <p>請點擊下方按鈕設定新密碼：</p>
                    <p>
                        <a href='{resetUrl}'
                           style='display:inline-block;padding:10px 20px;background-color:#667eea;color:#fff;text-decoration:none;border-radius:8px;'>
                           重設密碼
                        </a>
                    </p>
                    <p>如果您沒有申請此操作，請忽略此郵件。</p>
                </div>";

            await SendCustomEmailAsync(toEmail, "重設您的密碼", htmlContent);
        }

        // 發送一般通知信
        public async Task SendNotificationEmailAsync(string toEmail, string subject, string htmlContent)
        {
            await SendCustomEmailAsync(toEmail, subject, htmlContent);
        }

        // 發送自訂內容信件
        public async Task SendCustomEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var payload = new
            {
                from = _fromEmail,  // 讀取環境變數設定的寄件人信箱
                to = toEmail,
                subject = subject,
                html = htmlContent
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/emails", content);

            if (!response.IsSuccessStatusCode)
            {
                var respContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"❌ Resend API 發信失敗: {response.StatusCode}\n內容: {respContent}");
            }
            else
            {
                Console.WriteLine($"✅ 郵件已成功發送給 {toEmail}");
            }
        }
    }
}
