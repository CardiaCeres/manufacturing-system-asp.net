// Auto-converted from Java: User.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/model/User.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using jakarta.persistence.Entity
// using jakarta.persistence.GeneratedValue
// using jakarta.persistence.GenerationType
// using jakarta.persistence.Id
// using jakarta.persistence.Table
// using java.time.LocalDateTime

namespace Manufacturing.Api.ConvertedFromJava.model
{
@Entity
@Table(name = "users")
public class User {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private long id;

    private string username;
    private string password;
    private string email;

    // 重設密碼用
    private string resetToken;
    private LocalDateTime tokenExpiry;

    // Getter 和 Setter 方法
    public long getId() {
        return id;
    }

    public void setId(long id) {
        this.id = id;
    }

    public string getUsername() {
        return username;
    }

    public void setUsername(string username) {
        this.username = username;
    }

    public string getPassword() {
        return password;
    }

    public void setPassword(string password) {
        this.password = password;
    }

    public string getEmail() {
        return email;
    }

    public void setEmail(string email) {
        this.email = email;
    }

    public string getResetToken() {
        return resetToken;
    }

    public void setResetToken(string resetToken) {
        this.resetToken = resetToken;
    }

    public LocalDateTime getTokenExpiry() {
        return tokenExpiry;
    }

    public void setTokenExpiry(LocalDateTime tokenExpiry) {
        this.tokenExpiry = tokenExpiry;
    }
}
}
