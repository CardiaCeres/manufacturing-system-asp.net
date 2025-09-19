// Auto-converted from Java: ChatController.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/controller/ChatController.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using Spring equivalent: org.springframework.beans.factory.annotation.Value
// using Spring equivalent: org.springframework.http.*
// using Spring equivalent: org.springframework.web.bind.annotation.*
// using Spring equivalent: org.springframework.web.client.RestTemplate
using System.Collections.Generic;

namespace Manufacturing.Api.ConvertedFromJava.controller
{
[ApiController]
@CrossOrigin(origins = "frontend.url") 
[Route("/api")]
public class ChatController {

    @Value("${google.gemini.api_key}")
    private string apiKey;

    private final RestTemplate restTemplate = new RestTemplate();

    [HttpPost("/chat")]
    public string chat([FromBody] Dictionary<string, string> payload) {
        string userMessage = payload.get("message");

        string url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=" + apiKey;

        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);

        Dictionary<string, Object> content = new HashDictionary<>();
        content.put("role", "user");

        // 把引導語 + 使用者問題合併成一段話
        string prompt = """
你是一個智慧客服助理，負責回答有關訂單管理系統的問題，請用中文正確回覆問題，僅回答與問題直接相關的部分，功能包括：登入、登出、註冊、查詢、新增、修改、刪除訂單，帳號密碼保護，資料加密，安全有保障。登入:輸入帳號與密碼即可登入系統。登出:點選登出，註冊：填寫帳號、密碼與Email建立帳戶。查詢訂單:登入系統後可查詢訂單狀態。新增訂單:登入後點選新增訂單，填寫商品與數量後送出。修改訂單:於訂單列表點選編輯進行修改。刪除訂單:點選刪除後確認即可移除該筆訂單。
問題：""" + userMessage;

        content.put("parts", List.of(Map.of("text", prompt)));

        Dictionary<string, Object> requestBody = new HashDictionary<>();
        requestBody.put("contents", List.of(content));

        HttpEntity<Dictionary<string, Object>> requestEntity = new HttpEntity<>(requestBody, headers);

        try {
            ResponseEntity<Map> response = restTemplate.postForEntity(url, requestEntity, Map.class);
            Dictionary<?, ?> responseBody = response.getBody();

            if (responseBody != null && responseBody.containsKey("candidates")) {
                List<?> candidates = (List<?>) responseBody.get("candidates");
                if (!candidates.isEmpty()) {
                    Dictionary<?, ?> candidate = (Dictionary<?, ?>) candidates.get(0);
                    Dictionary<?, ?> contentMap = (Dictionary<?, ?>) candidate.get("content");
                    List<?> partsList = (List<?>) contentMap.get("parts");
                   if (!partsList.isEmpty()) {
                     Dictionary<?, ?> part = (Dictionary<?, ?>) partsList.get(0);
                     string reply = part.get("text").toString();
                     // 使用正則表達式移除成對的 **粗體**
                     reply = reply.replaceAll("\\*\\*(.*?)\\*\\*", "$1");
                     // 或若還有單獨出現的 *（非成對），也一併移除
                     reply = reply.replace("*", "");
                     return reply.trim();
                   }
                }
            }
            return "抱歉，未收到有效回覆。";
        } catch (Exception e) {
            e.printStackTrace();
            return "呼叫 Gemini API 發生錯誤：" + e.getMessage();
        }
    }
}
}
