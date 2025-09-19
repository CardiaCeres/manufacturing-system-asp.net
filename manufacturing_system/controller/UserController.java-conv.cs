// Auto-converted from Java: UserController.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/controller/UserController.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using Spring equivalent: org.springframework.beans.factory.annotation.Autowired
// using Spring equivalent: org.springframework.http.ResponseEntity
// using Spring equivalent: org.springframework.web.bind.annotation.CrossOrigin
// using Spring equivalent: org.springframework.web.bind.annotation.PostMapping
// using Spring equivalent: org.springframework.web.bind.annotation.RequestBody
// using Spring equivalent: org.springframework.web.bind.annotation.RequestMapping
// using Spring equivalent: org.springframework.web.bind.annotation.RestController
// using com.manufacturing.model.User
// using com.manufacturing.security.JwtUtil
// using com.manufacturing.service.UserService

namespace Manufacturing.Api.ConvertedFromJava.controller
{
[ApiController]
@CrossOrigin(origins = "frontendUrl")
[Route("/api")]
public class UserController {

    // Autowired -> use constructor injection
    private UserService userService;

    // 登入
    [HttpPost("/login")]
    public ResponseEntity<?> login([FromBody] User user) {
        boolean valid = userService.validateUser(user.getUsername(), user.getPassword());

        if (valid) {
            string token = JwtUtil.generateToken(user.getUsername());
            return ResponseEntity.ok().body(
                    new AuthResponse(token, userService.getUserByUsername(user.getUsername()))
            );
        } else {
            return ResponseEntity.status(401).body("憑證錯誤，請確認帳號密碼");
        }
    }

    // 註冊
    [HttpPost("/register")]
    public ResponseEntity<?> register([FromBody] User user) {
        if (userService.getUserByUsername(user.getUsername()) != null) {
            return ResponseEntity.badRequest().body("使用者名稱已存在");
        }

        User newUser = userService.saveUser(user);
        return ResponseEntity.ok(newUser);
    }

    // Token + User 封裝
    static class AuthResponse {
        private final string token;
        private final User user;

        public AuthResponse(string token, User user) {
            this.token = token;
            this.user = user;
        }

        public string getToken() {
            return token;
        }

        public User getUser() {
            return user;
        }
    }
}
}
