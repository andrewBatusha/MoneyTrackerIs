using BLL.Dto.BankSpecificExpenseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BLL.Helpers
{
    public class PrivatHelper
    {
        public async Task<IEnumerable<PrivatApiExpense>> GetRecentExpensesAsync(string merchentId, string card, string password)
        {
            DateTime from = DateTime.Now.AddMonths(-1);
            DateTime to = DateTime.Now;

            var response = await SendRequestAsync(merchentId, card, password, from, to);

            var xdoc = XDocument.Parse(response.OuterXml);

            if(!ResponseIsValid(xdoc))
                return null;

            IEnumerable<PrivatApiExpense> expenses = ParseExpenses(xdoc);
            return expenses.Where(e => e.Cardamount < 0).OrderBy(e => e.TranDateTime).ToList();
        }

        private IEnumerable<PrivatApiExpense> ParseExpenses(XDocument xdoc)
        {
            var xresponse = xdoc.Element("response");
            var xdata = xresponse.Element("data");
            var xinfo = xdata.Element("info");
            var xstatements = xinfo.Element("statements");

            var expenses = new List<PrivatApiExpense>();
            foreach (XElement statement in xstatements.Elements("statement"))
            {
                var expense = new PrivatApiExpense()
                {
                    Card = statement.Attribute("card").Value,
                    Appcode = statement.Attribute("appcode").Value,
                    TranDateTime = ParseDateTime(statement.Attribute("trandate").Value, statement.Attribute("trantime").Value),
                    Amount = ParseCurrency(statement.Attribute("amount").Value),
                    Cardamount = ParseCurrency(statement.Attribute("cardamount").Value),
                    Rest = ParseCurrency(statement.Attribute("rest").Value),
                    Terminal = statement.Attribute("terminal").Value,
                    Description = statement.Attribute("description").Value,
                };
                expenses.Add(expense);
            }

            return expenses;
        }

        private DateTime ParseDateTime(string dateStr, string timeStr)
        {
            var date = DateTime.Parse(dateStr + " " + timeStr);
            return date;
        }

        private double ParseCurrency(string value)
        {
            var amount = value.Split(" ")[0];
            var parsedAmount = Double.Parse(amount);
            return parsedAmount;
        }

        private bool ResponseIsValid(XDocument xdoc)
        {
            var xresponse = xdoc.Element("response");
            var xmerchant = xresponse.Element("merchant");
            if (xmerchant == null)
                return false;
            return true;
        }

        private async Task<XmlDocument> SendRequestAsync(string merchentId, string card, string password, DateTime from, DateTime to)
        {
            XmlDocument requestXml = BuildRequestBody(merchentId, card, password, from, to);

            // build XML request 

            var httpRequest = HttpWebRequest.Create("https://api.privatbank.ua/p24api/rest_fiz");
            httpRequest.Method = "POST";
            httpRequest.ContentType = "text/xml";

            // set appropriate headers

            using (var requestStream = httpRequest.GetRequestStream())
            {
                requestXml.Save(requestStream);
            }

            using (var response = (HttpWebResponse) await httpRequest.GetResponseAsync())
            using (var responseStream = response.GetResponseStream())
            {
                // may want to check response.StatusCode to
                // see if the request was successful

                var responseXml = new XmlDocument();
                responseXml.Load(responseStream);
                return responseXml;
            }
        }

        private XmlDocument BuildRequestBody(string merchentId, string card, string password, DateTime from, DateTime to)
        {
            var doc = new XmlDocument();
            var fromSrting = from.ToString("dd.MM.yyyy");
            var toSrting = to.ToString("dd.MM.yyyy");
            var data = @"<oper>cmt</oper>
    <wait>30</wait>
    <test>0</test>
    <payment id="""">
      <prop name=""sd"" value=""" + fromSrting + @""" />
      <prop name=""ed"" value=""" + toSrting + @""" />
      <prop name=""card"" value=""" + card + @""" />
    </payment>";

            var signature = GetSignature(password, data);
            var body = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<request version=""1.0"">
  <merchant>
    <id>" + merchentId + @"</id>
    <signature>" + signature + @"</signature>
  </merchant>
  <data>
    " + data + @"
  </data>
</request>";
            doc.LoadXml(body);
            return doc;
        }

        private string GetSignature(string password, string data)
        {
            var md5Hash = Hasher.MD5(data + password).ToLower();

            return Hasher.SHA1(md5Hash);
        }
    }
}
