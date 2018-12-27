using System;
using Binance.API.Csharp.Client;
using System.Linq;
using System.Collections.Generic;
namespace CryptoDNC
{
    class Program
    {
        public static List<Currency> Currencies = new List<Currency>();
        const string apikey = "z3ZYAyTdnIfPqNq0AkRviMKdTDF36GknKofFnQ5J0LrxQOdcfvuBVgNVBE4vBaCI";
        const string apisecret = "YGWPMVcjZVVHaiSDxNH3n7GaII3lrvNoTWefmNkq5T2b9wqXe2ibT3GIHTQukoV3";
        static void Main(string[] args)
        {

            //Console.WriteLine("Hello World!");
            var apiClient = new ApiClient(apikey, apisecret);
            var binanceClient = new BinanceClient(apiClient);
            var res = binanceClient.GetAccountInfo().Result;
            var pos = 0;
            var height = 0;
            Console.Clear();
            var HorizontalMax = Console.WindowWidth;
            var VerticalMax = Console.WindowHeight;
            foreach (var item in res.Balances.ToList().Where(a=>a.Free > 0).OrderByDescending(o=>o.Free)){
                Currency c = new Currency(){ SymbolPos = new ConsolePosition(){ top=height,left=pos  }, Symbol=item.Asset, Balance=item.Free };
                Console.SetCursorPosition(c.left,height);
                Console.Write(item.Asset);
                Console.SetCursorPosition(c.left+7,height);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(item.Free);
                Console.ForegroundColor = ConsoleColor.White;

                

                if (pos+25 < HorizontalMax) {
                    pos += 25;
                }
                else {
                    height +=3;
                    pos = 0;
                }
            }
            Console.WriteLine("\r\n All Currencies accounted for.");
        }
    }
}
