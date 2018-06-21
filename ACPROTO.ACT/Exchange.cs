using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using ACPROTO.ACT.Models;

namespace ACPROTO.ACT
{
    public class Exchange
    {
        public static Coin GetCurrencyValue(Coin coin)
        {
            var json = WebRequest($"https://min-api.cryptocompare.com/data/price?fsym={coin.Currency}&tsyms=USD,BTC");

            if (json == string.Empty) return coin;
            if (json.Contains("There is no data"))
            {
                return coin;
            }
            if (coin.Currency == "ADA")
                Console.Write("...");

            var split = json.Split(',');
            coin.ValueInUSD = decimal.Parse(split[0].Replace("{\"USD\":", "").Replace("}", ""));
            coin.ValueInBTC = (decimal)float.Parse(split[1].Replace("\"BTC\":", "").Replace("}", ""));
            return coin;
        }
        public static decimal TotalPortfolioValue { get; internal set; }

        public static void UpdateTickers(Coin coin)
        {
            var result = WebRequest("https://bittrex.com/api/v1.1/public/getticker?market=" + coin.MarketName);

            try
            {
                coin.TickerData.Add(new Ticker(result, coin.MarketName));
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        internal static void GetOrderHistory()
        {
            //https://bittrex.com/api/v1.1/account/getorderhistory&count=10
            var json = AdvancedAuth("https://bittrex.com/api/v1.1/account/getorderhistory");

            if (json == string.Empty) return;

            JObject jObject = JObject.Parse(json);
            JToken jresult = jObject["result"];

            foreach (JObject jo in jresult)
            {
                Order order = new Order();

                order.OrderUuid = jo["OrderUuid"].ToString();
                order.Exchange = jo["Exchange"].ToString();
                order.OrderType = jo["OrderType"].ToString();
                order.Quantity = decimal.Parse(jo["Quantity"].ToString());
                order.QuantityRemaining = decimal.Parse(jo["QuantityRemaining"].ToString());
                order.Limit = (decimal)float.Parse(jo["Limit"].ToString());
                order.Commission = (decimal)float.Parse(jo["Commission"].ToString());
                order.Price = decimal.Parse(jo["Price"].ToString());
                order.PricePerUnit = (decimal)float.Parse(jo["PricePerUnit"].ToString());
                order.Opened = jo["Opened"] != null ? DateTime.Parse(jo["Opened"].ToString()) : DateTime.Now;
                order.Closed = jo["Closed"] != null ? DateTime.Parse(jo["Closed"].ToString()) : DateTime.Now;
                order.CancelInitiated = jo["CancelInitiated"] != null ? bool.Parse(jo["CancelInitiated"].ToString()) : false;
                order.ImmediateOrCancel = bool.Parse(jo["ImmediateOrCancel"].ToString());
                order.IsConditional = bool.Parse(jo["IsConditional"].ToString());

                var coin = Coin.AllCoins.Where(c => c.Currency == order.Exchange.Replace("BTC-", "")).FirstOrDefault();
                //if (order.OrderType == "LIMIT_BUY" && coin != null)
                {
                    //var oldOrder = Order.AllOrders.Where(o => o.Market == order.Market && o.TimeStamp < order.TimeStamp).FirstOrDefault();
                    //if (oldOrder != null) Order.AllOrders.Remove(oldOrder);
                    Order.AllOrders.Remove(order);
                    Order.AllOrders.Add(order);
                }
            }
        }

        internal static void GetBalances()
        {
            var json = AdvancedAuth("https://bittrex.com/api/v1.1/account/getbalances");

            if (json == string.Empty) return;

            JObject jObject = JObject.Parse(json);
            JToken jresult = jObject["result"];

            foreach (JObject jo in jresult)
            {
                Coin coin = Coin.AllCoins.Where(c => c.Currency == jo["Currency"].ToString()).FirstOrDefault();

                if (coin == null)
                {
                    coin = new Coin()
                    {
                        Currency = jo["Currency"].ToString(),
                        Balance = (decimal)float.Parse(jo["Balance"].ToString()),
                        Available = (decimal)float.Parse(jo["Available"].ToString()),
                        Pending = (decimal)float.Parse(jo["Pending"].ToString()),
                        CryptoAddress = jo["Currency"].ToString(),
                        MarketName = "BTC-" + jo["Currency"].ToString()
                    };
                    coin = GetCurrencyValue(coin);
                    Coin.AllCoins.Add(coin);
                }
                else
                {
                    coin = Coin.AllCoins.Where(c => c.Currency == jo["Currency"].ToString()).FirstOrDefault();
                    coin.Balance = (decimal)float.Parse(jo["Balance"].ToString());
                    coin.Available = (decimal)float.Parse(jo["Available"].ToString());
                    coin.Pending = (decimal)float.Parse(jo["Pending"].ToString());
                    coin = GetCurrencyValue(coin);
                }
            }
        }

        public static void GetOpenOrders()
        {
            var json = AdvancedAuth("https://bittrex.com/api/v1.1/market/getopenorders");
            JObject jObject = JObject.Parse(json);
            JToken jresult = jObject["result"];

            foreach (JObject jo in jresult)
            {
                Order order = new Order
                {
                    Uuid = jo["Uuid"].ToString(),
                    OrderUuid = jo["OrderUuid"].ToString(),
                    Exchange = jo["Exchange"].ToString(),
                    OrderType = jo["OrderType"].ToString(),
                    Quantity = decimal.Parse(jo["Quantity"].ToString()),
                    QuantityRemaining = decimal.Parse(jo["QuantityRemaining"].ToString()),
                    Limit = decimal.Parse(jo["Limit"].ToString()),
                    Commission = decimal.Parse(jo["CommissionPaid"].ToString()),
                    Price = decimal.Parse(jo["Price"].ToString()),
                    PricePerUnit = decimal.Parse(jo["PricePerUnit"].ToString()),
                    Opened = DateTime.Parse(jo["Opened"].ToString()),
                    Closed = null, //no idea what this is used for
                    CancelInitiated = bool.Parse(jo["CancelInitiated"].ToString()),
                    ImmediateOrCancel = bool.Parse(jo["ImmediateOrCancel"].ToString()),
                    IsConditional = bool.Parse(jo["IsConditional"].ToString())
                };
                Order.AllOrders.Add(order);
            }
        }
        ///market/buylimit
        //    https://bittrex.com/api/v1.1/market/buylimit?apikey=API_KEY&market=BTC-LTC&quantity=1.2&rate=1.3    
        public static void SubmitBuyOrder(Coin coin)
        {
            //var json = AdvancedAuth($"https://bittrex.com/api/v1.1/market/buylimit?market={coin.MarketName}&quantity={coin.Balance}&rate={coin.TickerData.Last().Ask}");
            //JObject jObject = JObject.Parse(json);
            //JToken jresult = jObject["result"];
        }

        public static void SellAll(Coin coin)
        {
            var json = AdvancedAuth($"https://bittrex.com/api/v1.1/market/selllimit?market={coin.MarketName}&quantity={coin.Balance - coin.TxFee}&rate={coin.TickerData.Last().Bid}");
            JObject jObject = JObject.Parse(json);
            JToken jresult = jObject["result"];
        }

        public static void SubmitSellOrder(Coin coin, decimal amount, decimal price)
        {
            //-coin.TxFee
            var json = AdvancedAuth($"https://bittrex.com/api/v1.1/market/selllimit?market={coin.MarketName}&quantity={amount}&rate={price}");
            JObject jObject = JObject.Parse(json);
            JToken jresult = jObject["result"];
        }

        public static void GetMarkets()
        {
            //var json = WebRequest("https://bittrex.com/api/v1.1/public/getmarkets");
            //JObject jObject = JObject.Parse(json);
            //JToken jresult = jObject["result"];
            //foreach (JObject jo in jresult)
            //{
            //    Market market = new Market()
            //    {
            //        BaseCurrency = jo["BaseCurrency"].ToString(),
            //        BaseCurrencyLong = jo["BaseCurrencyLong"].ToString(),
            //        MarketCurrency = jo["MarketCurrency"].ToString(),
            //        MarketCurrencyLong = jo["MarketCurrencyLong"].ToString(),
            //        MarketName = jo["MarketName"].ToString(),
            //        MinTradeSize = decimal.Parse(jo["MinTradeSize"].ToString()),
            //        IsActive = bool.Parse(jo["IsActive"].ToString())
            //    };
            //    mareketsList.Add(market);
            //}
        }

        public static void GetCurrencies()
        {
            var json = WebRequest("https://bittrex.com/api/v1.1/public/getcurrencies");
            JObject jObject = JObject.Parse(json);
            JToken jresult = jObject["result"];

            foreach (JObject jo in jresult)
            {
                var coin = Coin.AllCoins.Where(c => c.Currency == jo["Currency"].ToString()).FirstOrDefault();

                if (coin == null)
                {
                    Coin currency = new Coin()
                    {
                        Currency = jo["Currency"].ToString(),
                        BaseAddress = jo["BaseAddress"].ToString(),
                        CoinType = jo["CoinType"].ToString(),
                        CurrencyLong = jo["CurrencyLong"].ToString(),
                        MinConfirmation = int.Parse(jo["MinConfirmation"].ToString()),
                        TxFee = (decimal)float.Parse(jo["TxFee"].ToString()),
                        IsActive = bool.Parse(jo["IsActive"].ToString())
                    };
                    Coin.AllCoins.Add(currency);
                }
                else
                {
                    coin.Currency = jo["Currency"].ToString();
                    coin.BaseAddress = jo["BaseAddress"].ToString();
                    coin.CoinType = jo["CoinType"].ToString();
                    coin.CurrencyLong = jo["CurrencyLong"].ToString();
                    coin.MinConfirmation = int.Parse(jo["MinConfirmation"].ToString());
                    coin.TxFee = decimal.Parse(jo["TxFee"].ToString());
                    coin.IsActive = bool.Parse(jo["IsActive"].ToString());
                }
            }
        }

        public static string WebRequest(string url, string addheader = null, string addvalue = null)
        {
            HttpWebRequest webRequest = null;
            string responseData = string.Empty;

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            if (addheader != null) webRequest.Headers.Add(addheader, addvalue);
            webRequest.Method = "GET";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 200000;
            responseData = WebResponseGet(webRequest);
            webRequest = null;
            return responseData;
        }

        public static string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = string.Empty;

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch (Exception)
            {
                //throw;
            }
            finally
            {
                if (responseReader != null)
                {
                    responseReader.Close();
                    responseReader = null;
                }
            }
            return responseData;
        }

        public static string AdvancedAuth(string uri)
        {
            var apikey = "";
            var apisecret = "";
            var nonce = DateTime.UtcNow;
            string web = "";

            if (uri.Contains("?"))
            {
                web = uri + "&apikey=" + apikey + "&nonce=" + nonce;
            }
            else
            {
                web = uri + "?apikey=" + apikey + "&nonce=" + nonce;
                if (uri == "https://bittrex.com/api/v1.1/account/getorderhistory") web += "&count=20";
            }

            var sign = Encryption.Encrypt(web, apisecret);

            var test = WebRequest(web, "apisign", sign);

            //add header called "apisign:" with sign
            /// var ch = curl_init($uri);
            // curl_setopt($ch, CURLOPT_HTTPHEADER, array('apisign:'.$sign));
            return test;
        }
    }
}