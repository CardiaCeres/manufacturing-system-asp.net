// Auto-converted from Java: EmailService.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/service/EmailService.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using jakarta.mail.MessagingException
// using jakarta.mail.internet.MimeMessage
// using Spring equivalent: org.springframework.beans.factory.annotation.Autowired
// using Spring equivalent: org.springframework.mail.javamail.JavaMailSender
// using Spring equivalent: org.springframework.mail.javamail.MimeMessageHelper
// using Spring equivalent: org.springframework.stereotype.Service

namespace Manufacturing.Api.ConvertedFromJava.service
{
[ServiceDescriptor] // Map Service to DI registration
public class EmailService {

    // Autowired -> use constructor injection
    private JavaMailSender mailSender;

    public void sendResetPasswordEmail(string toEmail, string resetUrl) {
        try {
            MimeMessage mimeMessage = mailSender.createMimeMessage();
            
            // 第二個參數 true 代表這是 multipart (支援 HTML)
            MimeMessageHelper helper = new MimeMessageHelper(mimeMessage, true, "UTF-8");

            helper.setFrom("no-reply@yourapp.com"); // ✅ 寄件人 (要在 SendGrid 驗證過)
            helper.setTo(toEmail);
            helper.setSubject("重設您的密碼");

            // HTML 內容
            string htmlContent = """
                    <div style="font-family: Arial, sans-serif; line-height: 1.6;">
                        <h2>🔐 重設密碼通知</h2>
                        <p>您好，</p>
                        <p>我們收到了您重設密碼的請求，請點擊下方按鈕以設定新密碼：</p>
                        <p>
                            <a href="%s" style="display:inline-block; padding:10px 20px; 
                                background-color:#667eea; color:#fff; 
                                text-decoration:none; border-radius:8px;">
                                👉 重設密碼
                            </a>
                        </p>
                        <p>如果不是您本人操作，請忽略這封信件。</p>
                        <hr/>
                        <small>智慧訂單管理系統 · 請勿回覆此信件</small>
                    </div>
                    """.formatted(resetUrl);

            helper.setText(htmlContent, true); // 第二個參數 true = HTML

            mailSender.send(mimeMessage);

        } catch (MessagingException e) {
            throw new RuntimeException("寄送郵件失敗: " + e.getMessage(), e);
        }
    }
}
}
