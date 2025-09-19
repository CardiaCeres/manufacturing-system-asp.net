// Auto-converted from Java: IndexController.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/controller/IndexController.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using Spring equivalent: org.springframework.stereotype.Controller
// using Spring equivalent: org.springframework.web.bind.annotation.RequestMapping

namespace Manufacturing.Api.ConvertedFromJava.controller
{
[ApiController]
public class IndexController {
 
    @RequestMapping(value = {
        "/", "/login","/register","/orders"
    })
    public string forward() {
        return "forward:/index.html";
    }
}
}
