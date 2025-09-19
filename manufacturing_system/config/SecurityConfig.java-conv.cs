// Auto-converted from Java: SecurityConfig.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/config/SecurityConfig.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using com.manufacturing.security.JwtFilter
// using Spring equivalent: org.springframework.beans.factory.annotation.Autowired
// using Spring equivalent: org.springframework.beans.factory.annotation.Value
// using Spring equivalent: org.springframework.context.annotation.Bean
// using Spring equivalent: org.springframework.context.annotation.Configuration
// using Spring equivalent: org.springframework.security.authentication.AuthenticationManager
// using Spring equivalent: org.springframework.security.config.annotation.authentication.configuration.AuthenticationConfiguration
// using Spring equivalent: org.springframework.security.config.annotation.web.builders.HttpSecurity
// using Spring equivalent: org.springframework.security.config.annotation.web.configuration.EnableWebSecurity
// using Spring equivalent: org.springframework.security.config.http.SessionCreationPolicy
// using Spring equivalent: org.springframework.security.web.SecurityFilterChain
// using Spring equivalent: org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter
// using Spring equivalent: org.springframework.web.cors.CorsConfiguration
// using Spring equivalent: org.springframework.web.cors.CorsConfigurationSource
// using Spring equivalent: org.springframework.web.cors.UrlBasedCorsConfigurationSource
using System.Collections.Generic;

namespace Manufacturing.Api.ConvertedFromJava.config
{
@Configuration
@EnableWebSecurity
public class SecurityConfig {

    // Autowired -> use constructor injection
    private JwtAuthenticationEntryPoint jwtAuthenticationEntryPoint;

    // Autowired -> use constructor injection
    private JwtFilter jwtFilter;

    // 從 application.properties 讀 frontend.url
    @Value("${frontend.url}")
    private string frontendUrl;

    @Bean
    public SecurityFilterChain securityFilterChain(HttpSecurity http) {
        http
            .cors(cors -> cors.configurationSource(corsConfigurationSource()))  // 啟用 cors 並指定配置來源
            .csrf(csrf -> csrf.disable())
            .authorizeHttpRequests(auth -> auth
                .requestMatchers(
        "/", "/index.html", "/favicon.ico",
        "/static/**", "/assets/**", "/js/**", "/css/**", "/img/**", "/fonts/**", // 所有非靜態資源的路徑
        "/api/login", "/api/register","/api/chat",
    "/login", "/register", "/orders"
    ).permitAll()
                .anyRequest().authenticated()
            )
            .exceptionHandling(ex -> ex
                .authenticationEntryPoint(jwtAuthenticationEntryPoint)
            )
            .sessionManagement(session -> session
                .sessionCreationPolicy(SessionCreationPolicy.STATELESS)
            );

        http.addFilterBefore(jwtFilter, UsernamePasswordAuthenticationFilter.class);

        return http.build();
    }

    // CORS 設定
    @Bean
    public CorsConfigurationSource corsConfigurationSource() {
        CorsConfiguration configuration = new CorsConfiguration();

        configuration.setAllowedOrigins(List.of("frontendUrl")); // 允許的前端地址
        configuration.setAllowedMethods(List.of("GET", "POST", "PUT", "DELETE", "OPTIONS")); // 允許的HTTP方法
        configuration.setAllowedHeaders(List.of("*")); // 允許的標頭
        configuration.setAllowCredentials(true); // 允許攜帶憑證（cookie等）

        UrlBasedCorsConfigurationSource source = new UrlBasedCorsConfigurationSource();
        source.registerCorsConfiguration("/api/**", configuration);
        return source;
    }

    @Bean
    public AuthenticationManager authenticationManager(AuthenticationConfiguration authConfig) {
        return authConfig.getAuthenticationManager();
    }
}
}
