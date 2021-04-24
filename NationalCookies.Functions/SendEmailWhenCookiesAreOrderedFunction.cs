using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NationalCookies.Data;
using SendGrid.Helpers.Mail;

namespace NationalCookies.Functions
{
    public static class SendEmailWhenCookiesAreOrderedFunction
    {
        [FunctionName("SendEmailWhenCookiesAreOrderedFunction")]
        public static async void Run([CosmosDBTrigger(
            databaseName: "CookiesDatabase",
            collectionName: "Orders",
            ConnectionStringSetting = "CosmosDBConnectionString",
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists =true)]
            IReadOnlyList<Document> input,
            [SendGrid(ApiKey ="SendGridKey")] IAsyncCollector<SendGridMessage> messageCollector,
            ExecutionContext context)
        {
            if (input != null && input.Count > 0)
            {
                Order order = (Order)(dynamic)input[0];
                IConfigurationRoot config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var websiteUrl = config["NationalCookiesUrl"];

                var content = "You have a new order <br/><br/>" +
                    "Oder date: " + order.Date.ToString("ddMMyyyy") + "<br/>" +
                    "Price: €" + order.Price + "<br/><br/>" +
                    "More details <a href='" + websiteUrl + "/Order/Detail?id=" + order.Id + "'> here</a>";

                SendGridMessage message = new SendGridMessage();
                message.AddTo("naj.sheikh94@gmail.com");
                message.AddContent("text/html", content);
                message.SetFrom(new EmailAddress("naj.sheikh94@gmail.com"));
                message.SetSubject("You ordered cookies!");

                await messageCollector.AddAsync(message);
                    
            }
        }
    }
}
