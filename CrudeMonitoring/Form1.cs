using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using Binance.API.Csharp.Client.Models.WebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CrudeMonitoring
{
    public partial class Form1 : Form
    {
        public ApiClient apiClient = null;
        public BinanceClient binanceClient = null;
        const string apikey = "z3ZYAyTdnIfPqNq0AkRviMKdTDF36GknKofFnQ5J0LrxQOdcfvuBVgNVBE4vBaCI";
        const string apisecret = "YGWPMVcjZVVHaiSDxNH3n7GaII3lrvNoTWefmNkq5T2b9wqXe2ibT3GIHTQukoV3";
        Coin btc = new Coin() { Currency = "BTC", MarketName = "BTCUSDT" };
        //Coin coin = new Coin { Balance = 100, Currency = "TRX", MarketName = "TRXBTC", LastPurchasePrice = 0.01m, ValueInBTC = 0.001m };
        public Form1()
        {
            btc = Coin.GetCurrencyValue(btc);
            InitializeComponent();
        }

        private async void Form1_LoadAsync(object sender, EventArgs e)
        {

            apiClient = new ApiClient(apikey, apisecret);
            binanceClient = new BinanceClient(apiClient);
            var res = await binanceClient.GetAccountInfo();



            foreach (var item in res.Balances.Where(b=>b.Free >0 && b.Asset != "BTC" && b.Asset != "USDT"))
            {
                var coin = new Coin { MarketName = item.Asset + "BTC", Balance = item.Free + item.Locked, Currency = item.Asset };

                coin = Coin.GetCurrencyValue(coin);
                var gb = await GenerateChart(coin);
                try
                {
                    flowLayoutPanel1.Controls.Add(gb);
                }
                catch (Exception) { };
                Coin.AllCoins.Add(coin);

            }
            Thread thread = new Thread(AddHandlers);
            thread.Start();
        }

        private async Task<GroupBox> GenerateChart(Coin coin) {
            var _candlesticks = await binanceClient.GetCandleSticks(coin.MarketName, TimeInterval.Minutes_5);
            List<CandleStick> candlesticks = new List<CandleStick>();

            foreach (var item in _candlesticks)
            {
                var cs = new CandleStick();
                cs.Open = Math.Round(item.Open * btc.ValueInUSD,4) * 100;
                cs.Close = Math.Round(item.Close * btc.ValueInUSD,4) * 100;
                cs.High = Math.Round(item.High * btc.ValueInUSD,4) * 100;
                cs.Low = Math.Round(item.Low * btc.ValueInUSD,4) * 100;
                cs.CloseTime = item.CloseTime;
                candlesticks.Add(cs);
                Console.WriteLine(cs.Timestamp);
            }


            GroupBox gb = new GroupBox() { Name = coin.MarketName, Text = coin.MarketName, Width=250, Height=250 };
            Chart chart1 = new Chart();
            chart1.Dock = DockStyle.Fill;
            gb.Dock = DockStyle.None;
            ChartArea chartArea1 = new ChartArea();
            chartArea1.Name = "ChartArea_" + coin.Currency;
            if (coin.LastPurchasePrice == 0) coin.LastPurchasePrice = coin.ValueInBTC;
            var max = (double)(candlesticks.Max(o => o.Open));
            var min = (double)(candlesticks.Min(o => o.Open));

            max = max * 1.01;
            min = min * 0.99;

            chartArea1.AxisY.Maximum = max;
            chartArea1.AxisY.Minimum = min;
            chartArea1.AxisX.MajorGrid.LineWidth = 0;
            chartArea1.AxisY.MajorGrid.LineWidth = 0;

            //chartArea1.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //chartArea1.AxisX.IntervalType = DateTimeIntervalType.Minutes;
            //chartArea1.AxisY.IntervalType = type;
            //chartArea1.AxisX.Interval = 0;
            //chartArea1.AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //chartArea1.AxisY.Interval = 0;

            //chartArea1.AxisY.LabelStyle.Format = "{0:X}";
            chartArea1.AxisX.LabelStyle.Format = "{0:hh-MM-ss}";

            chart1.ChartAreas.Add(chartArea1);
            chartArea1.AxisY.LabelStyle.Format = "E";

            Series series1 = new Series
            {
                Name = "Series_" + coin.Currency,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.Candlestick,
                Color = Color.Red,
                XValueType = ChartValueType.DateTime,
                YValueType = ChartValueType.String,
                //YAxisType = AxisType.Primary
  
            };

            // Set the style of the open-close marks
            series1["OpenCloseStyle"] = "Triangle";

            series1.XValueMember = "TimeStamp";
            series1.YValueMembers = "High, Low, Open, Close";
            series1.CustomProperties = "PrieceDownColor=Red,PriceUpColor=green";
            series1["ShowOpenClose"] = "Both";
            chart1.DataManipulator.IsStartFromFirst = true;

            // Set point width
            series1["PointWidth"] = "1.0";

            // Set colors bars
            series1["PriceUpColor"] = "Green"; // <<== use text indexer for series
            series1["PriceDownColor"] = "Red"; // <<== use text indexer for series
            chart1.Series.Clear();
            chart1.Series.Add(series1);

            coin.Candles = candlesticks.Take(50).ToList();

            chart1.DataSource = coin.Candles;
            chart1.DataBind();

            gb.Controls.Add(chart1);

            coin.chart = chart1;
            return gb;
        }
        public void AddHandlers() {
            foreach (var coin in Coin.AllCoins)
            {
                SubScribe(coin);
            }
        }
        public void SubScribe(Coin coin) {
            binanceClient.ListenKlineEndpoint(coin.MarketName, TimeInterval.Minutes_1, (o) =>
            {
                var i = o.KlineInfo;
                coin.Candles.Add(new CandleStick() { High = i.High, Close = i.Close, Low = i.Low, Open = i.Open, Volume = i.Volume, OpenTime = i.StartTime });
            });
        }
    }
    //public class CandleStick
    //{
    //    int id;
    //    string time_stamp;
    //    double open, close, high, low;
    //    public CandleStick(int id, string time_stamp, double open, double close, double high, double low)
    //    {
    //        this.id = id;
    //        this.time_stamp = time_stamp;
    //        this.open = open;
    //        this.close = close;
    //        this.high = high;
    //        this.low = low;
    //    }
    //    public int ID
    //    {
    //        get { return id; }
    //        set { id = value; }
    //    }
    //    public string TimeStamp
    //    {
    //        get { return time_stamp; }
    //        set { time_stamp = value; }
    //    }
    //    public double Open
    //    {
    //        get { return open; }
    //        set { open = value; }
    //    }
    //    public double Close
    //    {
    //        get { return close; }
    //        set { close = value; }
    //    }
    //    public double High
    //    {
    //        get { return high; }
    //        set { high = value; }
    //    }
    //    public double Low
    //    {
    //        get { return low; }
    //        set { low = value; }
    //    }

    //}
}
