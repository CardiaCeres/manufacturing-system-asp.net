using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManufacturingSystem.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _apiKey = "GEMINI_API_KEY";

        public ChatController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] JsonElement payload)
        {
            if (!payload.TryGetProperty("message", out var msg)) return BadRequest("Message required");
            var userMessage = msg.GetString();

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var contentJson = JsonSerializer.Serialize(new
            {
                contents = new[]
                {
                    new {
                        role = "user",
                        parts = new[] { new { text = $"你是一個智慧客服助理...問題：{userMessage}" } }
                    }
                }
            });

            var client = _clientFactory.CreateClient();
            var response = await client.PostAsync(url, new StringContent(contentJson, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode) return StatusCode(500, "呼叫 Gemini API 發生錯誤");

            var resultJson = await response.Content.ReadAsStringAsync();
            return Ok(resultJson);
        }
    }
}
