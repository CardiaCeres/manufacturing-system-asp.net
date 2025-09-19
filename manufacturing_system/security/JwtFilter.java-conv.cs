// Auto-converted from Java: JwtFilter.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/security/JwtFilter.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using java.io.IOException
// using Spring equivalent: org.springframework.beans.factory.annotation.Autowired
// using Spring equivalent: org.springframework.security.authentication.UsernamePasswordAuthenticationToken
// using Spring equivalent: org.springframework.security.core.context.SecurityContextHolder
// using Spring equivalent: org.springframework.security.web.authentication.WebAuthenticationDetailsSource
// using Spring equivalent: org.springframework.stereotype.Component
// using Spring equivalent: org.springframework.web.filter.OncePerRequestFilter
// using com.manufacturing.model.User
// using com.manufacturing.service.UserService
// using Servlet equivalent: jakarta.servlet.FilterChain
// using Servlet equivalent: jakarta.servlet.ServletException
// using Servlet equivalent: jakarta.servlet.http.HttpServletRequest
// using Servlet equivalent: jakarta.servlet.http.HttpServletResponse

namespace Manufacturing.Api.ConvertedFromJava.security
{
// 假設 User 是你的 model

[ServiceDescriptor]
public class JwtFilter : OncePerRequestFilter {

    // Autowired -> use constructor injection
    private UserService userService;

    // override
    protected void doFilterInternal(HttpServletRequest request,
                                    HttpServletResponse response,
                                    FilterChain filterChain)
            {

        string authHeader = request.getHeader("Authorization");

        if (authHeader != null && authHeader.startsWith("Bearer ")) {
            string token = authHeader.substring(7);
            string username = JwtUtil.extractUsername(token);

            if (username != null && SecurityContextHolder.getContext().getAuthentication() == null) {
                User user = userService.getUserByUsername(username);  // 改成明確類型 User
                if (JwtUtil.validateToken(token, username)) {
                    UsernamePasswordAuthenticationToken authToken =
                            new UsernamePasswordAuthenticationToken(user, null, null);
                    authToken.setDetails(new WebAuthenticationDetailsSource().buildDetails(request));

                    SecurityContextHolder.getContext().setAuthentication(authToken);
                }
            }
        }

        filterChain.doFilter(request, response);
    }
}
}
