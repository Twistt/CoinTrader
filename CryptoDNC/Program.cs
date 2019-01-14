using System;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
namespace CryptoDNC
{
    class Program
    {
        public static bool Updating = false;
        public static Timer timer = new Timer(TimerTick, null, 1000, 1000);
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
            var HorizontalMax = Console.WindowWidth - 25;
            var VerticalMax = Console.WindowHeight;
            foreach (var item in res.Balances.ToList().Where(a => a.Free > 0).OrderByDescending(o => o.Free))
            {
                Currency c = new Currency() { SymbolPos = new ConsolePosition() { top = height, left = pos }, Symbol = item.Asset, Balance = Math.Round(item.Free,2), PreviousLast = 0, Market = (item.Asset + "usdt").ToLower()  };
                if (c.Symbol.ToLower() == "btc") continue;
                if (c.Symbol.ToLower() == "usdt") c.Market = "btcusdt";
                var ticker = tickerPrices.Where(s => s.Symbol.ToLower() == c.Market.ToLower()).FirstOrDefault();
                if (ticker != null) c.Last = ticker.Price;

                Console.SetCursorPosition(c.SymbolPos.left, height);
                Console.Write(item.Asset);
                Console.SetCursorPosition(c.SymbolPos.left + 7, height);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(Math.Round(item.Free,4));

                Console.SetCursorPosition(c.SymbolPos.left, height + 1);
                Console.ForegroundColor = ConsoleColor.Black;
                if (c.PreviousLast < c.Last) Console.BackgroundColor = ConsoleColor.Green;
                if (c.PreviousLast > c.Last) Console.BackgroundColor = ConsoleColor.Red;
                Console.Write("Last: " + Math.Round(c.Last,6));
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;

                Currencies.Add(c);
                //if (c.Symbol.ToLower() == "btc") continue; // we only need the balance not the trade data
                List<Candlestick> candlesticks = new List<Candlestick>();
                try{
                candlesticks = binanceClient.GetCandleSticks(c.Market, TimeInterval.Minutes_15).Result.ToList();
                }
                catch (Exception err){
                    c.Market = c.Market = (c.Symbol + "btc").ToLower();
                    candlesticks = binanceClient.GetCandleSticks(c.Market, TimeInterval.Minutes_15).Result.ToList();
                }

                c.Candles.AddRange(candlesticks);



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
                    height += 6;
                    pos = 0;
                }
            }

            foreach (var c in Currencies){
                binanceClient.ListenKlineEndpoint(c.Market, TimeInterval.Minutes_15, (o) =>
                {
                    var i = o.KlineInfo;
                    c.Candles.Add(new Binance.API.Csharp.Client.Models.Market.Candlestick() { High = i.High, Close = i.Close, Low = i.Low, Open = i.Open, Volume = i.Volume });
                });
            }
            Updating = false;
            //Console.WriteLine("\r\n All Cusdt");
            while (true){
                var input = Console.ReadLine();
                if (input == "quit") break;
            }
        }
        static void TimerTick(object state)
        {
            if (Updating) return;
            else Updating = true;
            Console.SetCursorPosition(Console.WindowWidth > 0 ? Console.WindowWidth - 25 : 0, Console.WindowHeight > 0 ? Console.WindowHeight - 5 : 0);
            Console.WriteLine(DateTime.Now);
            var tickerPrices = binanceClient.GetAllPrices().Result;
            foreach (var c in Currencies)
            {
                c.PreviousLast = c.Last;
                var ticker = tickerPrices.Where(s => s.Symbol.ToLower() == c.Market).FirstOrDefault();
                if (ticker != null) c.Last = ticker.Price;
                UpdateTicker(c);
            }
            Console.SetCursorPosition(Console.WindowWidth > 0 ? Console.WindowWidth - 25 : 0, Console.WindowHeight > 0 ? Console.WindowHeight - 5 : 0);
            Updating = false;
        }
        static void UpdateTicker(Currency c)
        {
            Console.SetCursorPosition(c.SymbolPos.left, c.SymbolPos.top + 1);
            if (c.PreviousLast < c.Last) Console.BackgroundColor = ConsoleColor.Green;
            if (c.PreviousLast > c.Last) Console.BackgroundColor = ConsoleColor.Red;
            if (c.PreviousLast != c.Last) Console.Write("Last: " + Math.Round(c.Last,6));
            //reset view
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            try {
                c = Common.TradeyCalcs(c);
            }
            catch (Exception err){
                c.sma = 0;
            }

            if (c.sma != null){
                Console.SetCursorPosition(c.SymbolPos.left, c.SymbolPos.top + 2);
                Console.Write("SMA: " + c.sma);
                Console.SetCursorPosition(c.SymbolPos.left, c.SymbolPos.top + 3);
                Console.Write("SD: " + c.sd);
                Console.SetCursorPosition(c.SymbolPos.left, c.SymbolPos.top + 4);
                Console.Write("Diff: " + c.smaDiff);
                Console.SetCursorPosition(Console.WindowWidth > 0 ? Console.WindowWidth - 25 : 0, Console.WindowHeight > 0 ? Console.WindowHeight - 5 : 0);
            }
        }

        public static Currency GetCurrencyValue(Currency coin)
        {
            var json = Common.WebRequest($"https://min-api.cryptocompare.com/data/price?fsym={coin.Symbol}&tsyms=USD,BTC");

            if (json == string.Empty) return coin;
            if (json.Contains("There is no data"))
            {
                return coin;
            }

            var split = json.Split(',');
            coin.ValueInUSD = decimal.Parse(split[0].Replace("{\"USD\":", "").Replace("}", ""));
            coin.ValueInBTC = (decimal)float.Parse(split[1].Replace("\"BTC\":", "").Replace("}", ""));
            return coin;
        }

    }
}
