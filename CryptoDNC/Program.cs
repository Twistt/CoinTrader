using System;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
namespace CryptoDNC
{
    class Program
    {
        public static bool Updating = false;
        public static Timer timer = new Timer(TimerTick,null,1000, 1000);
        public static List<Currency> Currencies = new List<Currency>();
        public static ApiClient apiClient = null;
        public static BinanceClient binanceClient = null;
        const string apikey = "z3ZYAyTdnIfPqNq0AkRviMKdTDF36GknKofFnQ5J0LrxQOdcfvuBVgNVBE4vBaCI";
        const string apisecret = "YGWPMVcjZVVHaiSDxNH3n7GaII3lrvNoTWefmNkq5T2b9wqXe2ibT3GIHTQukoV3";
        static void Main(string[] args)
        {
            Updating = true;
            //Console.WriteLine("Hello World!");
            apiClient = new ApiClient(apikey, apisecret);
            binanceClient = new BinanceClient(apiClient);
            var res = binanceClient.GetAccountInfo().Result;

            var tickerPrices = binanceClient.GetAllPrices().Result;

            var pos = 0;
            var height = 2;
            Console.Clear();
            var HorizontalMax = Console.WindowWidth-25;
            var VerticalMax = Console.WindowHeight;
            foreach (var item in res.Balances.ToList().Where(a => a.Free > 0).OrderByDescending(o => o.Free))
            {
                Currency c = new Currency() { SymbolPos = new ConsolePosition() { top = height, left = pos }, Symbol = item.Asset, Balance = item.Free, PreviousLast=0 };
                var market = (c.Symbol + "btc").ToLower();
                if (c.Symbol.ToLower() == "usdt") market = "btcusdt";
                var ticker = tickerPrices.Where(s=>s.Symbol.ToLower() == market.ToLower()).FirstOrDefault();
                if (ticker != null) c.Last = ticker.Price;

                Console.SetCursorPosition(c.SymbolPos.left, height);
                Console.Write(item.Asset);
                Console.SetCursorPosition(c.SymbolPos.left + 7, height);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(item.Free);
                
                Console.SetCursorPosition(c.SymbolPos.left, height+1);
                Console.ForegroundColor = ConsoleColor.Black;
                if (c.PreviousLast < c.Last) Console.BackgroundColor = ConsoleColor.Green;
                if (c.PreviousLast > c.Last) Console.BackgroundColor = ConsoleColor.Red;
                Console.Write(c.Last);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;

                Currencies.Add(c);
                if (c.Symbol.ToLower() == "btc") continue; // we only need the balance not the trade data

                var candlestick = binanceClient.GetCandleSticks(market, TimeInterval.Minutes_15).Result;
                c.Candles.AddRange(candlestick);

                binanceClient.ListenKlineEndpoint(market, TimeInterval.Minutes_15, (o) =>
                {
                    var i = o.KlineInfo;
                    c.Candles.Add(new Binance.API.Csharp.Client.Models.Market.Candlestick(){High = i.High, Close = i.Close, Low=i.Low, Open = i.Open, Volume = i.Volume });
                });

                /* Order book (not really neccessary yet)
                binanceClient.ListenTradeEndpoint(market, (o) =>
                {
                    Console.WriteLine(o);
                }); */

                if (pos + 25 < HorizontalMax)
                {
                    pos += 25;
                }
                else
                {
                    height += 3;
                    pos = 0;
                }
            }
            Updating = false;
            //Console.WriteLine("\r\n All Currencies accounted for.");
            Console.ReadKey();
        }
        static void TimerTick(object state){
            if (Updating) return;
            else Updating = true;
            Console.SetCursorPosition(Console.WindowWidth >0? Console.WindowWidth-25:0, Console.WindowHeight >0? Console.WindowHeight-5:0);
            Console.WriteLine(DateTime.Now);
            var tickerPrices = binanceClient.GetAllPrices().Result;
            foreach (var c in Currencies){
                c.PreviousLast = c.Last;
                var market = (c.Symbol + "btc").ToLower();
                if (c.Symbol.ToLower() == "usdt") market = "btcusdt";
                var ticker = tickerPrices.Where(s=>s.Symbol.ToLower() == market.ToLower()).FirstOrDefault();
                if (ticker != null) c.Last = ticker.Price;
                UpdateTicker(c);
            }
            Console.SetCursorPosition(0, Console.WindowHeight);
            Updating = false;
        }
        static void UpdateTicker(Currency c)
        {
            Console.SetCursorPosition(c.SymbolPos.left, c.SymbolPos.top+1);
            if (c.PreviousLast < c.Last) Console.BackgroundColor = ConsoleColor.Green;
            if (c.PreviousLast > c.Last) Console.BackgroundColor = ConsoleColor.Red;
            Console.Write(c.Last);
            //reset view
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
