using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;
using System;

namespace ASPIdentityManager.Service
{
    public class MailJetEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public MailJetOptions _mailJetOptions { get; set; }  
        public MailJetEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _mailJetOptions = _configuration.GetSection("MailJet").Get<MailJetOptions>();

            MailjetClient client = new MailjetClient(_mailJetOptions.ApiKey, _mailJetOptions.SecretKey);
            MailjetRequest request = new MailjetRequest { Resource = Send.Resource, }.Property(Send.Messages, new JArray {
                //new JObject
                //{
                //    {"From",new JObject {{"Email", "peterwanga@protonmail.com"}, {"Name", "Peter"}}},
                //    {"To",new JArray {new JObject {{"Email", email }, {"Name","Peter"}}}},
                //    {"Subject",subject}, 
                //    {"HTMLPart",htmlMessage}
                //}
               new JObject
                {
                    {"FromEmail", "peterwanga@protonmail.com"},
                    {"FromName", "Peter"},
                    {"Recipients", new JArray {new JObject {{"Email", email }, {"Name", "Peter"}}}},
                    {"Subject", subject},
                    {"HTMLPart", htmlMessage} // Make sure htmlMessage contains the actual HTML content
                }



            });
            MailjetResponse response = await client.PostAsync(request);
            //if (response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
            //    Console.WriteLine(response.GetData());
            //}
            //else
            //{
            //    Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
            //    Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
            //    Console.WriteLine(response.GetData());
            //    Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
            //}

        }
    }
}
