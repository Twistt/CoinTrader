using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACPROTO.ACT.Models
{
    public class Ticker
    {
        public Ticker(string json, string currency)
        {
            try
            {
                JObject jObject = JObject.Parse(json);
                JToken jresult = jObject["result"];
                Bid = (decimal)float.Parse(jresult["Bid"].ToString());
                Ask = (decimal)float.Parse(jresult["Ask"].ToString());
                Last = (decimal)float.Parse(jresult["Last"].ToString());
                TimeStamp = DateTime.Now;
                Currency = currency;
            }
            catch (Exception) { }
        }
        public Ticker() { }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public decimal Last { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Currency { get; set; }
        public decimal USDValue { get; set; }
        public decimal Quantity { get; set; }
    }
}
