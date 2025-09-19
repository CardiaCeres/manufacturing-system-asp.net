// Auto-converted from Java: UserService.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/service/UserService.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using java.time.LocalDateTime
using System.Collections.Generic;
using System.Collections.Generic;
// using Spring equivalent: org.springframework.beans.factory.annotation.Autowired
// using Spring equivalent: org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder
// using Spring equivalent: org.springframework.stereotype.Service
// using com.manufacturing.model.User
// using com.manufacturing.repository.UserRepository

namespace Manufacturing.Api.ConvertedFromJava.service
{
[ServiceDescriptor] // Map Service to DI registration
public class UserService {

    // Autowired -> use constructor injection
    private UserRepository userRepository;

    private final BCryptPasswordEncoder passwordEncoder = new BCryptPasswordEncoder();

    // 儲存新使用者或更新密碼
    public User saveUser(User user) {
        if (user.getPassword() != null) {
            user.setPassword(passwordEncoder.encode(user.getPassword()));
        }
        return userRepository.save(user);
    }

    // 驗證使用者登入
    public boolean validateUser(string username, string rawPassword) {
        User user = userRepository.findByUsername(username);
        if (user == null) return false;
        return passwordEncoder.matches(rawPassword, user.getPassword());
    }

    // 透過 username 取得使用者
    public User getUserByUsername(string username) {
        return userRepository.findByUsername(username);
    }

    // 透過 email 取得使用者
    public User> getUserByEmail(string email) {
        return userRepository.findByEmail(email);
    }

    // 透過 resetToken 取得使用者
    public User> getUserByResetToken(string token) {
        return userRepository.findByResetToken(token);
    }

    // 產生重設密碼 Token
    public string generateResetToken(User user) {
        string token = UUID.randomUUID().toString();
        user.setResetToken(token);
        user.setTokenExpiry(LocalDateTime.now().plusHours(1)); // Token 有效 1 小時
        userRepository.save(user);
        return token;
    }

    // 驗證 Token 是否有效
    public boolean isResetTokenValid(User user, string token) {
        return token.equals(user.getResetToken()) &&
               user.getTokenExpiry() != null &&
               user.getTokenExpiry().isAfter(LocalDateTime.now());
    }

    // 重設密碼
    public void resetPassword(User user, string newPassword) {
        user.setPassword(passwordEncoder.encode(newPassword));
        user.setResetToken(null);
        user.setTokenExpiry(null);
        userRepository.save(user);
    }
}
}
