// Auto-converted from Java: WebConfig.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/config/WebConfig.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using Spring equivalent: org.springframework.context.annotation.Bean
// using Spring equivalent: org.springframework.context.annotation.Configuration
// using Spring equivalent: org.springframework.web.servlet.config.annotation.CorsRegistry
// using Spring equivalent: org.springframework.web.servlet.config.annotation.WebMvcConfigurer

namespace Manufacturing.Api.ConvertedFromJava.config
{
@Configuration
public class WebConfig {
    @Bean
    public WebMvcConfigurer corsConfigurer() {
        return new WebMvcConfigurer() {
            // override
            public void addCorsMappings(CorsRegistry registry) {
                registry.addMapping("/api/**")
                    .allowedOrigins("frontendUrl")
                    .allowedMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                    .allowedHeaders("*")
                    .allowCredentials(true);
            }
        };
    }
}
}
