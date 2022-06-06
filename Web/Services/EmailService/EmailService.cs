using Web.ViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace Web.Services.EmailService
{
    public class EmailService : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var readyToSendEmail = new MimeMessage();

            readyToSendEmail.From.Add(MailboxAddress.Parse(_config.GetSection("EmailSettings")
                                                    .GetValue<string>("EmailUserName")));

            readyToSendEmail.To.Add(MailboxAddress.Parse(email));

            readyToSendEmail.Subject = subject;

            readyToSendEmail.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
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
            smtp.Send(readyToSendEmail);
            smtp.Disconnect(true);

            return Task.CompletedTask;
            //throw new System.NotImplementedException();
        }

        //public void SendMail(EmailVm emailVm)
        //{
        //    var email = new MimeMessage();

        //    email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailSettings")
        //                                               .GetValue<string>("EmailUserName")));

        //    email.To.Add(MailboxAddress.Parse(emailVm.To));

        //    email.Subject = emailVm.Subject;

        //    email.Body = new TextPart(TextFormat.Html)
        //    {
        //        Text = emailVm.Body
        //    };

        //    using var smtp = new SmtpClient();

        //    smtp.Connect(host: _config.GetSection("EmailSettings").GetValue<string>("EmailHost"),
        //                 port: 587,
        //                 SecureSocketOptions.StartTls);

        //    smtp.Connect("smtp.gmail.com");
        //    smtp.Connect("smtp.live.com");
        //    smtp.Connect("smtp.office365.com");

        //    smtp.Authenticate(userName: _config.GetSection("EmailSettings").GetValue<string>("EmailUserName"),
        //                      password: _config.GetSection("EmailSettings").GetValue<string>("EmailPassword"));
        //    smtp.Send(email);
        //    smtp.Disconnect(true);
        //}
    }
}

