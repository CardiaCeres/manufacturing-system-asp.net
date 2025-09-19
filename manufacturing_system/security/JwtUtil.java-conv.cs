// Auto-converted from Java: JwtUtil.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/security/JwtUtil.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using java.nio.charset.StandardCharsets
using System.Collections.Generic;
// using javax.crypto.SecretKey
// using io.jsonwebtoken.JwtException
// using io.jsonwebtoken.Jwts
// using io.jsonwebtoken.SignatureAlgorithm
// using io.jsonwebtoken.security.Keys

namespace Manufacturing.Api.ConvertedFromJava.security
{
public class JwtUtil {

    // 建立 SecretKey (HS256)
    private static final string SECRET = "yourSecretKey123yourSecretKey123"; // 建議長度 >= 256 bits (32 bytes)
    private static final SecretKey SECRET_KEY = Keys.hmacShaKeyFor(SECRET.getBytes(StandardCharsets.UTF_8));

    private static final long EXPIRATION_TIME = 86400000L; // 1 天毫秒

    public static string generateToken(string username) {
        return Jwts.builder()
                .setSubject(username)
                .setIssuedAt(new Date())
                .setExpiration(new Date(System.currentTimeMillis() + EXPIRATION_TIME))
                .signWith(SECRET_KEY, SignatureAlgorithm.HS256)
                .compact();
    }

    public static string extractUsername(string token) {
        try {
            return Jwts.parserBuilder()
                    .setSigningKey(SECRET_KEY)
                    .build()
                    .parseClaimsJws(token)
                    .getBody()
                    .getSubject();
        } catch (JwtException e) {
            // token 無效或解析失敗
            // e.printStackTrace(); // 可加入日誌
            return null;
        }
    }

    public static boolean validateToken(string token, string username) {
        string extractedUsername = extractUsername(token);
        return extractedUsername != null && extractedUsername.equals(username)
                && !isTokenExpired(token);
    }

    public static boolean isTokenExpired(string token) {
        try {
            Date expiration = Jwts.parserBuilder()
                    .setSigningKey(SECRET_KEY)
                    .build()
                    .parseClaimsJws(token)
                    .getBody()
                    .getExpiration();
            return expiration.before(new Date());
        } catch (JwtException e) {
            return true;
        }
    }
}
}
