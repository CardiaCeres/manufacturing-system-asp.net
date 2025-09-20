using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ManufacturingSystem.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _apiKey;

        public ChatController(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _apiKey = config["GEMINI_API_KEY"]; // 從 Render 環境變數讀取
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] JsonElement payload)
        {
            if (!payload.TryGetProperty("message", out var msg)) 
                return BadRequest("Message required");

            var userMessage = msg.GetString();

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var contentJson = JsonSerializer.Serialize(new
            {
                contents = new[]
                {
                    new {
                        parts = new[] { new { text = $"你是一個智慧客服助理，請用中文回答以下問題：{userMessage}" } }
                    }
                }
            });

            var client = _clientFactory.CreateClient();
            var response = await client.PostAsync(url, new StringContent(contentJson, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode) 
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var resultJson = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonNode.Parse(resultJson);

            var reply = jsonDoc?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

            // 🔥 清理文字 (移除 Markdown 與換行符號)
            if (!string.IsNullOrEmpty(reply))
            {
                reply = Regex.Replace(reply, @"\*\*(.*?)\*\*", "$1"); // 移除 **粗體**
                reply = Regex.Replace(reply, @"\*", "");              // 移除單獨 *
                reply = reply.Replace("\n", " ");                     // 把換行轉成空格
                reply = reply.Trim();
            }

            // ✅ 直接回傳純文字，不包 JSON
            return Content(reply ?? "", "text/plain", Encoding.UTF8);
        }
    }
}
