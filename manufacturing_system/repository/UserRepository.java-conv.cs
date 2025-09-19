// Auto-converted from Java: UserRepository.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/repository/UserRepository.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using Spring equivalent: org.springframework.data.jpa.repository.JpaRepository
// using Spring equivalent: org.springframework.stereotype.Repository
// using com.manufacturing.model.User
using System.Collections.Generic;

namespace Manufacturing.Api.ConvertedFromJava.repository
{
[Repository] // Map Repository to DI registration
public interface UserRepository : JpaRepository<User, long> {

    // 根據 username 查找用戶
    User findByUsername(string username);

    // 根據 email 查找用戶
    User> findByEmail(string email);

    // 根據 resetToken 查找用戶
    User> findByResetToken(string resetToken);
}
}
