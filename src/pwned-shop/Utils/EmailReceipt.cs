using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using RazorLight;



namespace pwned_shop.Utils
{
    public class EmailReceipt
    {
        const string DOMAIN = "";
        const string API_KEY = "";

        public async static Task<IRestResponse> SendReceipt(string email, Receipt receipt)
        {
            string template = File.ReadAllText("Utils/Receipt_template.html", System.Text.Encoding.UTF8);
            const string key = "templateKey";

            // mock model for testing
            //var receipt1 = new Receipt
            //{
            //    OrderId = "fj32lkj322443sd",
            //    ReceiptItems = new List<ReceiptItem>
            //                        {
            //                            new ReceiptItem
            //                            {
            //                                ProductName = "Valheim",
            //                                ActivationCodes = new List<string> { "1234dsdf45", "231984u", "3209ufsldifj"},
            //                                UnitPrice = 18,
            //                                Qty = 3
            //                            },

            //                            new ReceiptItem
            //                            {
            //                                ProductName = "Three Kingdom",
            //                                ActivationCodes = new List<string> { "1dfdsf45", "32ds546di65fj"},
            //                                UnitPrice = 38,
            //                                Qty = 2
            //                            }
            //                        }
            //};

            var engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(typeof(Receipt))
                .SetOperatingAssembly(typeof(Receipt).Assembly)
                .Build();

            string msg = await engine.CompileRenderStringAsync(key, template, receipt);

            return await SendEmail(msg, email);
        }

        public async static Task<IRestResponse> SendEmail(string html, string email)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");

            client.Authenticator =
                new HttpBasicAuthenticator("api", API_KEY);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", DOMAIN, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", $"PwnedShop-NoReply <receipt-no-reply@{DOMAIN}>");
            request.AddParameter("to", email);
            request.AddParameter("subject", "Pwned Shop - Purchase Receipt");
            request.AddParameter("html", html);
            request.Method = Method.POST;
            return await client.ExecuteAsync(request);
        }
    }

    public class ReceiptItem
    {
        public string ProductName { get; set; }
        public List<string> ActivationCodes { get; set; }
        public float UnitPrice { get; set; }
        public int Qty { get; set; }
        public float Discount { get; set; }
    }

    public class Receipt
    {
        public Receipt()
        {
            ReceiptItems = new List<ReceiptItem>();
        }

        public string OrderId { get; set; }
        public List<ReceiptItem> ReceiptItems { get; set; }
        public float TotalPrice
        {
            get
            {
                return ReceiptItems.Sum(r => r.UnitPrice*(1 - r.Discount) * r.Qty);
            }
        }
    }
}

