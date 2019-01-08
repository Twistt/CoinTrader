using Binance.API.Csharp.Client.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace CrudeMonitoring
{
    public class Coin
    {
        public string Currency { get; set; }
        public string MarketName { get; set; }
        public decimal Balance { get; set; }
        public decimal Available { get; set; }
        public decimal Pending { get; set; }
        public string CryptoAddress { get; set; }
        public bool Requested { get; set; }
        public Guid Uuid { get; set; }
        public decimal TxFee { get; set; }
        public decimal ValueInBTC { get; set; }
        public decimal ValueInUSD { get; set; }
        public decimal LastPurchasePrice { get; set; }
        public bool ShowinUSD { get; set; }



        public string BaseAddress { get; internal set; }
        public string CoinType { get; internal set; }
        public string CurrencyLong { get; internal set; }
        public int MinConfirmation { get; internal set; }
        public bool IsActive { get; internal set; }

        public static List<Coin> AllCoins = new List<Coin>();
        public Chart chart = null;
        public List<Candlestick> Candles = new List<Candlestick>();

        public static Coin GetCurrencyValue(Coin coin)
        {
            var json = Common.WebRequest($"https://min-api.cryptocompare.com/data/price?fsym={coin.Currency}&tsyms=USD,BTC");

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
