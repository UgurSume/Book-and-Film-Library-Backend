using System.Net;
using System.Net.Mail;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Services
{
    public class EmailService : IEmailService
    {
  private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
     {
    _configuration = configuration;
  _logger = logger;
   }

   public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName)
  {
            var resetLink = $"{_configuration["App:BaseUrl"]}/reset-password?token={resetToken}";

            var subject = "Þifre Sýfýrlama Talebi - Sosyal Kütüphane";
  var body = $@"
    <html>
  <body style='font-family: Arial, sans-serif;'>
       <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
 <h2 style='color: #333;'>Merhaba {userName},</h2>
            <p>Þifrenizi sýfýrlamak için bir talepte bulundunuz.</p>
       <p>Aþaðýdaki butona týklayarak þifrenizi sýfýrlayabilirsiniz:</p>
    <div style='text-align: center; margin: 30px 0;'>
   <a href='{resetLink}' 
 style='background-color: #007bff; color: white; padding: 12px 30px; 
     text-decoration: none; border-radius: 5px; display: inline-block;'>
   Þifremi Sýfýrla
  </a>
          </div>
           <p style='color: #666; font-size: 14px;'>
         Veya aþaðýdaki linki tarayýcýnýza kopyalayýn:<br>
          <a href='{resetLink}'>{resetLink}</a>
    </p>
    <p style='color: #999; font-size: 12px; margin-top: 30px;'>
       Bu link 1 saat geçerlidir. Eðer bu talebi siz yapmadýysanýz, bu e-postayý görmezden gelebilirsiniz.
   </p>
         </div>
        </body>
  </html>
  ";

            await SendEmailAsync(toEmail, subject, body);
     }

public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
   var subject = "Hoþ Geldiniz - Sosyal Kütüphane";
        var body = $@"
     <html>
       <body style='font-family: Arial, sans-serif;'>
           <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
       <h2 style='color: #333;'>Hoþ geldiniz {userName}!</h2>
            <p>Sosyal Kütüphane platformumuza katýldýðýnýz için teþekkür ederiz.</p>
       <p>Artýk kitap ve filmlerinizi kataloglayabilir, puanlayabilir ve paylaþabilirsiniz.</p>
           <p>Ýyi eðlenceler!</p>
        </div>
    </body>
  </html>
     ";

            await SendEmailAsync(toEmail, subject, body);
        }

     private async Task SendEmailAsync(string toEmail, string subject, string body)
   {
            try
            {
     var smtpHost = _configuration["Email:SmtpHost"];
       var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
    var smtpUsername = _configuration["Email:SmtpUsername"];
    var smtpPassword = _configuration["Email:SmtpPassword"];
       var fromEmail = _configuration["Email:FromEmail"];
   var fromName = _configuration["Email:FromName"] ?? "Sosyal Kütüphane";

         // Eðer SMTP ayarlarý yoksa, console'a yaz (development için)
        if (string.IsNullOrEmpty(smtpHost))
  {
     _logger.LogWarning("SMTP ayarlarý yapýlandýrýlmamýþ. Email gönderilemiyor.");
  _logger.LogInformation($"[EMAIL] To: {toEmail}, Subject: {subject}");
     return;
     }

       using var client = new SmtpClient(smtpHost, smtpPort)
{
          Credentials = new NetworkCredential(smtpUsername, smtpPassword),
       EnableSsl = true
  };

    var mailMessage = new MailMessage
  {
       From = new MailAddress(fromEmail ?? smtpUsername!, fromName),
  Subject = subject,
                    Body = body,
       IsBodyHtml = true
       };

     mailMessage.To.Add(toEmail);

    await client.SendMailAsync(mailMessage);
_logger.LogInformation($"Email baþarýyla gönderildi: {toEmail}");
    }
     catch (Exception ex)
        {
     _logger.LogError(ex, $"Email gönderilirken hata oluþtu: {toEmail}");
throw;
            }
 }
    }
}
