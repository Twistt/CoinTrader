using ACPROTO.ACT.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACPROTO.ACT
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
        public decimal Profit {
            get {
                if (TickerData.Count > 0)
                {
                    var item = TickerData.Last();
                    if (item != null)
                    {
                        var rawProfit = (item.Bid - LastPurchasePrice) * (Balance - TxFee);
                        return (rawProfit );
                    }
                }
                return 0;
            }
        }
        public Image Icon { get {
                if (File.Exists($"Icons\\{Currency}.png")) return Image.FromFile($"Icons\\{Currency}.png");
                else return Image.FromFile($"Icons\\BTC.png");

            } }

        public string BaseAddress { get; internal set; }
        public string CoinType { get; internal set; }
        public string CurrencyLong { get; internal set; }
        public int MinConfirmation { get; internal set; }
        public bool IsActive { get; internal set; }

        public static List<Coin> AllCoins = new List<Coin>();
        public List<Ticker> TickerData = new List<Ticker>();

    }
}
