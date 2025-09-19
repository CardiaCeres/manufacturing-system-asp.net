// Auto-converted from Java: JwtAuthenticationEntryPoint.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/config/JwtAuthenticationEntryPoint.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using java.io.IOException
// using Spring equivalent: org.springframework.security.core.AuthenticationException
// using Spring equivalent: org.springframework.security.web.AuthenticationEntryPoint
// using Spring equivalent: org.springframework.stereotype.Component
// using Servlet equivalent: jakarta.servlet.ServletException
// using Servlet equivalent: jakarta.servlet.http.HttpServletRequest
// using Servlet equivalent: jakarta.servlet.http.HttpServletResponse

namespace Manufacturing.Api.ConvertedFromJava.config
{
[ServiceDescriptor]
public class JwtAuthenticationEntryPoint : AuthenticationEntryPoint {
    // override
    public void commence(HttpServletRequest request,
                         HttpServletResponse response,
                         AuthenticationException authException) {
        response.sendError(HttpServletResponse.SC_UNAUTHORIZED, "未授權的存取");
    }
}
}
