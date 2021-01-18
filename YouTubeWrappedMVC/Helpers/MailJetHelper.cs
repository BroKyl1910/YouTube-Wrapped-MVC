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
        public static async Task SendEmail(string emailAddress, string url)
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
                .WithSubject("YouTube Wrapped data ready to view")
                .WithHtmlPart("<p>Hey there!</p> <p>We are pleased to inform you that your data is ready to view at: <br/>"+url+"</p>")
                .WithTo(new SendContact(emailAddress))
                .Build();

            // invoke API to send email
            var response = await client.SendTransactionalEmailAsync(email);
        }
    }
}
