using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Helpers
{
    public class MailJetHelper
    {
        public static async Task SendEmail()
        {
            MailjetClient client = new MailjetClient(
            Environment.GetEnvironmentVariable("$MJ_APIKEY_PUBLIC", EnvironmentVariableTarget.Machine),
            Environment.GetEnvironmentVariable("$MJ_APIKEY_PRIVATE", EnvironmentVariableTarget.Machine));

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            };

         // construct your email with builder
            var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact("brooks.kyle621@gmail.com"))
                .WithSubject("Test subject")
                .WithHtmlPart("<h1>Header</h1>")
                .WithTo(new SendContact("brooks.kyle621@gmail.com"))
                .Build();

            // invoke API to send email
            var response = await client.SendTransactionalEmailAsync(email);
        }
    }
}
