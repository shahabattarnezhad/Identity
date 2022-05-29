using Web.ViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Configuration;

namespace Web.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendMail(EmailVm emailVm)
        {
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailSettings")
                                                       .GetValue<string>("EmailUserName")));

            email.To.Add(MailboxAddress.Parse(emailVm.To));

            email.Subject = emailVm.Subject;

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = emailVm.Body
            };

            using var smtp = new SmtpClient();

            smtp.Connect(host: _config.GetSection("EmailSettings").GetValue<string>("EmailHost"),
                         port: 587,
                         SecureSocketOptions.StartTls);

            //smtp.Connect("smtp.gmail.com");
            //smtp.Connect("smtp.live.com");
            //smtp.Connect("smtp.office365.com");

            smtp.Authenticate(userName: _config.GetSection("EmailSettings").GetValue<string>("EmailUserName"),
                              password: _config.GetSection("EmailSettings").GetValue<string>("EmailPassword"));
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}

