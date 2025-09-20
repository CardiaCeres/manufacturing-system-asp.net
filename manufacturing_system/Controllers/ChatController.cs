using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
            _apiKey = config["Gemini:ApiKey"]; // 從 appsettings.json 或環境變數讀取
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
                        parts = new[] { new { text = $"你是一個智慧客服助理，請回答以下問題：{userMessage}" } }
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

            return Ok(new { reply });
        }
    }
}
