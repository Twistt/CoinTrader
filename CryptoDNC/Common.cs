using System;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.IO;
using Trady;
using Trady.Analysis;
using Trady.Analysis.Indicator;
using Trady.Core.Infrastructure;
using Binance.API.Csharp.Client.Models.Market;

namespace CryptoDNC
{
    public static class Common
    {
        public static Currency CalculateFibStrategy(Currency C){
            var highestHigh = C.Candles.Max(c=>c.Close);
            var lowestLow = C.Candles.Min(c=>c.Close);

            var fib1 = highestHigh;
            var fib786 = highestHigh - ((highestHigh - lowestLow) * 0.214m);
            var fib618 = highestHigh - ((highestHigh - lowestLow) * 0.382m);
            var fib50 =  highestHigh -((highestHigh - lowestLow) * 0.500m);
            var fib382 = highestHigh -((highestHigh - lowestLow) * 0.618m);
            var fib236 =  highestHigh -((highestHigh - lowestLow) * 0.764m);
            var fib0 = lowestLow;

            return C;
        }

        private static decimal Frama(decimal a, int b, Currency C)
        {
            return 0;
            
            var w = -4.6;
            var highestHigh = C.Candles.OrderByDescending(d=>d.CloseTime).Take(b).Max(c=>c.Close);
            var lowestLow = C.Candles.OrderByDescending(d=>d.CloseTime).Take(b).Min(c=>c.Close);
            var n3    = (double)(highestHigh - lowestLow)/b;
            var hd2   = C.Candles.OrderByDescending(d=>d.CloseTime).Take(b/2).Max(c=>c.Close);
            var ld2   = C.Candles.OrderByDescending(d=>d.CloseTime).Take(b/2).Min(c=>c.Close);
            var n2    = (double)(hd2 - ld2)/(b/2);
            var n1    = (double)(C.Candles.OrderByDescending(d=>d.CloseTime).Skip(b/2).FirstOrDefault().High - C.Candles.OrderByDescending(d=>d.CloseTime).Skip(b/2).FirstOrDefault().Low )/(b/2);
            var dim   = (n1 > 0) && (n2 > 0) && (n3 > 0) ? (Math.Log(n1 + n2) - Math.Log(n3))/Math.Log(2) : 0;
            var alpha = Math.Exp(w*(dim - 1));
            var sc    = (alpha < 0.01 ? 0.01 : (alpha > 1 ? 1 : alpha));
            //return cum(1)<=2*b ? a : (a*sc) + nz(frama[1])*(1 - sc)
            return 0;

        }
        public static Currency TradeyCalcs(Currency c)
        {
            var tcandles = new List<IOhlcv>();
            foreach (var item in c.Candles){
                tcandles.Add(new TradyCandle(){  Open=item.Open, Close=item.Close, DateTime=FromUnixTime(item.CloseTime), Volume=item.Volume, High=item.High, Low=item.Low });
            }


            var comp = new SimpleMovingAverage(tcandles, 30);

            //sma - standard moving average
            var res = comp.ComputeSma(30);
             var calc = res.LastOrDefault();
             if (calc!= null && calc.Tick != null) c.sma= Math.Round((decimal)calc.Tick, 4);

            //sd - standard deviation
            res = comp.ComputeSd(30);
             calc = res.LastOrDefault();
             if (calc!= null && calc.Tick != null) c.sd= Math.Round((decimal)calc.Tick, 4);

            //smadif - standard moving average differential (i think)
            res = comp.ComputeDiff(30);
             calc = res.LastOrDefault();
             if (calc!= null && calc.Tick != null) c.smaDiff= Math.Round((decimal)calc.Tick, 4);

            //smadif - standard moving average differential (i think)
            res = comp.ComputeRDiff(30);
             calc = res.LastOrDefault();
             if (calc!= null && calc.Tick != null) c.smaRDiff= Math.Round((decimal)calc.Tick, 4);

            return c;

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
        public static DateTime FromUnixTime(long unixTime)
        {
            var date = DateTimeOffset.FromUnixTimeMilliseconds(unixTime).UtcDateTime;
            return date;
        }

    }
    public class TradyCandle: IOhlcv {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public DateTimeOffset DateTime {get;set;}
    }
}