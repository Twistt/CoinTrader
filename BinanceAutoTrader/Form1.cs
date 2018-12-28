using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Threading.Timer;
using System.Windows.Forms.DataVisualization.Charting;
using Binance.API.Csharp.Client.Models.Market;

namespace BinanceAutoTrader
{
    public partial class Form1 : Form
    {
        public bool Updating = false;
        public Timer timer = null;
        public List<Currency> Currencies = new List<Currency>();
        public ApiClient apiClient = null;
        public BinanceClient binanceClient = null;
        const string apikey = "z3ZYAyTdnIfPqNq0AkRviMKdTDF36GknKofFnQ5J0LrxQOdcfvuBVgNVBE4vBaCI";
        const string apisecret = "YGWPMVcjZVVHaiSDxNH3n7GaII3lrvNoTWefmNkq5T2b9wqXe2ibT3GIHTQukoV3";
        public decimal BTCValue = 0;
        public bool needUpdateUI { get; set; }

        public Form1()
        {
            timer = new Timer(TimerTick, null, 1000, 1000);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Main();
        }

        private async void Main()
        {
            Updating = true;
            //Console.WriteLine("Hello World!");
            apiClient = new ApiClient(apikey, apisecret);
            binanceClient = new BinanceClient(apiClient);
            var res = await binanceClient.GetAccountInfo();

            var tickerPrices = await binanceClient.GetAllPrices();

            foreach (var item in res.Balances.ToList().Where(a => a.Free > 0).OrderByDescending(o => o.Free))
            {
                Currency c = new Currency() { Symbol = item.Asset, Balance = item.Free, PreviousLast = 0 };
                var market = (c.Symbol + "btc").ToLower();
                if (c.Symbol.ToLower() == "usdt") market = "btcusdt";
                c.Market = market;
                var ticker = tickerPrices.Where(s => s.Symbol.ToLower() == market.ToLower()).FirstOrDefault();
                if (ticker != null) c.Last = ticker.Price;

                Currencies.Add(c);
                if (c.Symbol.ToLower() == "btc")
                {
                    BTCValue = c.Last;
                    continue; // we only need the balance not the trade data
                }

                binanceClient.ListenKlineEndpoint(market, TimeInterval.Minutes_15, (o) =>
                {
                    var i = o.KlineInfo;
                    c.Candles.Add(new Binance.API.Csharp.Client.Models.Market.Candlestick() { High = i.High, Close = i.Close, Low = i.Low, Open = i.Open, Volume = i.Volume });
                });

                /* Order book (not really neccessary yet)
                binanceClient.ListenTradeEndpoint(market, (o) =>
                {
                    Console.WriteLine(o);
                }); */
                if (c.Symbol == "USDT") continue;
                flowLayoutPanel1.Controls.Add(await CreateCurrencyGroup(c));
            }
            Updating = false;
            //Console.WriteLine("\r\n All Currencies accounted for.");
        }

        private async Task<GroupBox> CreateCurrencyGroup(Currency coin)
        {
            //if ((coin.ValueInUSD * coin.Balance) < 1) return null;
            GroupBox gbCurrency = new System.Windows.Forms.GroupBox();
            PictureBox pictureBox1 = new System.Windows.Forms.PictureBox()
            {
                Location = new System.Drawing.Point(6, 19),
                Name = "pb" + coin.Symbol,
                Size = new System.Drawing.Size(56, 50),
                Image = coin.Icon,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            pictureBox1.Click += (o, i) =>
            {
                System.Diagnostics.Process.Start("https://www.binance.com/en/trade/pro/" + coin.Symbol);
            };

            Label lblBuyPrice = new System.Windows.Forms.Label()
            {
                AutoSize = true,
                Location = new System.Drawing.Point(130, 20),
                Name = "lblBuyPrice",
                Size = new System.Drawing.Size(35, 13),
                Text = coin.LastPurchasePrice.ToString()
            };
            lblBuyPrice.Click += (o, ev) => {
                coin.ShowinUSD = !coin.ShowinUSD;
                needUpdateUI = true;
            };
            Label lblUSDValue = new System.Windows.Forms.Label()
            {
                AutoSize = true,
                Location = new System.Drawing.Point(130, 38),
                Name = "lblUSDValue",
                Size = new System.Drawing.Size(35, 13),
                Text = coin.ValueInUSD.ToString()
            };
            lblUSDValue.Click += (o, ev) => {
                coin.ShowinUSD = !coin.ShowinUSD;
                needUpdateUI = true;
            };
            Label lblProfit = new System.Windows.Forms.Label()
            {
                AutoSize = true,
                Location = new System.Drawing.Point(110, 56),
                Name = "lblProfit",
                Size = new System.Drawing.Size(35, 13),
                TabIndex = 4,
                Text = (coin.Profit).ToString(),
                ForeColor = coin.Profit >= 0 ? Color.Green : Color.Red
            };
            #region Static Labels
            Label label1 = new System.Windows.Forms.Label()
            {
                Name = "lblBuy",
                AutoSize = true,
                Location = new System.Drawing.Point(65, 20),
                Size = new System.Drawing.Size(55, 13),
                Text = "Buy Price:"
            };
            Label label2 = new System.Windows.Forms.Label()
            {
                AutoSize = true,
                Location = new System.Drawing.Point(65, 38),
                Name = "label2",
                Size = new System.Drawing.Size(57, 13),
                TabIndex = 3,
                Text = "USD Value:"
            };
            Label lblTotalProfit = new System.Windows.Forms.Label()
            {
                AutoSize = true,
                Location = new System.Drawing.Point(65, 56),
                Name = "lblTotalProfit",
                Size = new System.Drawing.Size(57, 13),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Text = "Profit:"
            };
            lblTotalProfit.Click += (o, ev) => {
                coin.ShowinUSD = !coin.ShowinUSD;
                needUpdateUI = true;
            };

            Label label7 = new System.Windows.Forms.Label()
            {
                AutoSize = true,
                Location = new System.Drawing.Point(69, 81),
                Name = "label7",
                Size = new System.Drawing.Size(25, 13),
                TabIndex = 3,
                Text = "Bid:"
            };
            Label label8 = new System.Windows.Forms.Label()
            {
                AutoSize = true,
                Location = new System.Drawing.Point(69, 99),
                Name = "label8",
                Size = new System.Drawing.Size(28, 13),
                TabIndex = 3,
                Text = "Ask:"
            };
            Label label9 = new System.Windows.Forms.Label()
            {
                AutoSize = true,
                Location = new System.Drawing.Point(69, 116),
                Name = "label9",
                Size = new System.Drawing.Size(30, 13),
                TabIndex = 3,
                Text = "Last:",
            };
            #endregion

            Label label12 = new System.Windows.Forms.Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(100, 116),
                Name = "lblLast",
                Size = new System.Drawing.Size(35, 13),
                TabIndex = 4,
                Text = coin.TickerData.Count > 0 ? coin.TickerData.Last().Price.ToString() : "0"

            };
            Button button1 = new System.Windows.Forms.Button()
            {
                Location = new System.Drawing.Point(7, 81),
                Name = "btnCurrencyDisplay",
                Size = new System.Drawing.Size(56, 23),
                TabIndex = 5,
                Text = "Btc/$$",
                UseVisualStyleBackColor = true
            };
            button1.Click += (o, ev) =>
            {
                coin.ShowinUSD = !coin.ShowinUSD;
                needUpdateUI = true;
            };
            Panel panel1 = new System.Windows.Forms.Panel();


            // 
            // gbCurrency
            // 
            gbCurrency.Location = new System.Drawing.Point(33, 23);
            gbCurrency.Name = "gb" + coin.Symbol;
            gbCurrency.Size = new System.Drawing.Size(201, 256);
            gbCurrency.TabIndex = 1;
            gbCurrency.TabStop = false;
            gbCurrency.Text = $"{coin.Market} Tikr({coin.Symbol}) Amt({coin.Balance})";

            // 
            // label10
            // 
            Label lblBid = new System.Windows.Forms.Label()
            {
                AutoSize = true,
                Location = new System.Drawing.Point(100, 81),
                Name = "lblBid",
                Size = new System.Drawing.Size(35, 13),
                TabIndex = 4,
                Text = coin.Candles.Count > 0 ? coin.Candles.Last().Close.ToString() : 0.ToString()
            };
            Label btnSell = new Label { Text = "Sell", Location = new System.Drawing.Point(5, 75), Size = new System.Drawing.Size(56, 20), FlatStyle = FlatStyle.Flat, BorderStyle = BorderStyle.FixedSingle };
            btnSell.Click += (o, ev) =>
            {
                //Exchange.SellAll(coin);
            };

            Label lblAsk = new System.Windows.Forms.Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(100, 99),
                Name = "lblAsk",
                Size = new System.Drawing.Size(35, 13),
                TabIndex = 4,
                Text = coin.TickerData.Count > 0 ? coin.Candles.Last().High.ToString() : "0",

            };
            Label btnBuy = new Label { Text = "Buy", Location = new System.Drawing.Point(5, 95), Size = new System.Drawing.Size(56, 20), FlatStyle = FlatStyle.Flat, BorderStyle = BorderStyle.FixedSingle };
            btnBuy.Click += (o, ev) =>
            {
                //Exchange.SubmitBuyOrder(coin, coin.Balance);
            };

            Label btnSell10percent = new Label { Text = "Sell@10%", Location = new System.Drawing.Point(5, 115), Size = new System.Drawing.Size(56, 20), FlatStyle = FlatStyle.Flat, BorderStyle = BorderStyle.FixedSingle };
            btnSell10percent.Click += (o, ev) =>
            {
                //Add 10% to the purchase cost and enter the sell order.
                //Exchange.SubmitSellOrder(coin, coin.Balance, coin.LastPurchasePrice + (coin.LastPurchasePrice * 0.1m));
            };


            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(3, 132);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(195, 121);
            panel1.TabIndex = 6;



            panel1.Controls.Add(await GenerateChart(coin));
            gbCurrency.Controls.Add(btnSell10percent);
            gbCurrency.Controls.Add(panel1);
            //gbCurrency.Controls.Add(button1);
            gbCurrency.Controls.Add(lblBid);
            gbCurrency.Controls.Add(lblProfit);
            gbCurrency.Controls.Add(lblAsk);
            gbCurrency.Controls.Add(lblUSDValue);
            gbCurrency.Controls.Add(label12);
            gbCurrency.Controls.Add(lblBuyPrice);
            gbCurrency.Controls.Add(lblTotalProfit);
            gbCurrency.Controls.Add(label2);
            gbCurrency.Controls.Add(label7);
            gbCurrency.Controls.Add(label9);
            gbCurrency.Controls.Add(label8);
            gbCurrency.Controls.Add(label1);
            gbCurrency.Controls.Add(pictureBox1);
            gbCurrency.Controls.Add(btnSell);
            gbCurrency.Controls.Add(btnBuy);

            return gbCurrency;
        }
        public async void UpdateUI()
        {
            foreach (var coin in Currencies.Where(C => C.Balance == 0))
            {
                GroupBox gb = (GroupBox)flowLayoutPanel1.Controls["gb" + coin.Symbol];
                if (gb != null)
                {
                    flowLayoutPanel1.Controls.Remove(gb);
                }
            }
            foreach (var coin in Currencies.Where(c => c.Balance > 0))
            {
                GroupBox gb = (GroupBox)flowLayoutPanel1.Controls["gb" + coin.Symbol];
                if (gb != null && coin.TickerData.Count > 0)
                {
                    if (gb.Text != $"{coin.Market}-{coin.Symbol} (Amt({Math.Round(coin.Balance, 4)}))") {
                        gb.Text = $"{coin.Market}-{coin.Symbol} (Amt({Math.Round(coin.Balance, 4)}))";
                    }
                    //try
                    //{
                    //    var mostrecentorder = Order.AllOrders.Where(o => o.Exchange == "BTC-" + coin.Symbol).OrderByDescending(o => o.TimeStamp).FirstOrDefault();
                    //    if (mostrecentorder != null) coin.LastPurchasePrice = mostrecentorder.PricePerUnit;
                    //}
                    //catch (Exception) { }
                    ((Control)gb.Controls.Find("lblBuyPrice", false).FirstOrDefault()).Text = coin.ShowinUSD ? "$" + Math.Round(coin.LastPurchasePrice * BTCValue, 4).ToString() : Math.Round(coin.LastPurchasePrice, 10).ToString();
                    ((Control)gb.Controls.Find("lblBuyPrice", false).FirstOrDefault()).ForeColor = coin.ShowinUSD ? Color.Green : Color.Blue;

                    ((Control)gb.Controls.Find("lblBid", false).FirstOrDefault()).Text = coin.TickerData.Last().Price.ToString();
                    ((Control)gb.Controls.Find("lblAsk", false).FirstOrDefault()).Text = coin.TickerData.Last().Price.ToString();
                    ((Control)gb.Controls.Find("lblLast", false).FirstOrDefault()).Text = coin.TickerData.Last().Price.ToString();
                    ((Control)gb.Controls.Find("lblProfit", false).FirstOrDefault()).Text = coin.ShowinUSD ? "$" + Math.Round(coin.Profit * BTCValue, 4).ToString() : Math.Round(coin.Profit, 10).ToString();
                    ((Control)gb.Controls.Find("lblProfit", false).FirstOrDefault()).ForeColor = coin.Profit >= 0 ? Color.Green : Color.Red;
                    //((Control)gb.Controls.Find("btnCurrencyDisplay", false).FirstOrDefault()).ForeColor = coin.ShowinUSD ? Color.Green : Color.Blue;
                    ((Control)gb.Controls.Find("lblUSDValue", false).FirstOrDefault()).Text = "$" + Math.Round(coin.ValueInUSD * coin.Balance, 4).ToString();
                    lblTotalPortfolioValueUSD.Text = "Total Value in USD: $" + Math.Round(Currencies.Sum(c => (c.Last * c.Balance) * BTCValue), 4).ToString();
                    lblTotalPortfolioValueBTC.Text = "Total Value in BTC: " + Currencies.Sum(c => (c.Last * c.Balance)).ToString();

                    Chart chart = (Chart)gb.Controls.Find("Chart_" + coin.Symbol, true).FirstOrDefault();
                    if (chart != null)
                    {
                        var series_bid = chart.Series[0];
                        series_bid.Points.AddY((decimal)coin.TickerData.Last().Price);

                    }
                }
                else
                {
                    //Exchange.UpdateTickers(coin);
                    //Exchange.GetOrderHistory();
                    flowLayoutPanel1.Controls.Add(await CreateCurrencyGroup(coin));
                }
            }
            needUpdateUI = false;
        }
        private async void TimerTick(object state)
        {
            if (Updating) return;
            else Updating = true;

            var tickerPrices = await binanceClient.GetAllPrices();
            foreach (var c in Currencies)
            {
                try
                {
                    c.PreviousLast = c.Last;
                    var market = (c.Symbol + "BTC").ToLower();
                    if (c.Symbol.ToLower() == "usdt") market = "btcusdt";
                    var ticker = tickerPrices.Where(s => s.Symbol.ToLower() == market.ToLower()).FirstOrDefault();
                    if (ticker != null) c.Last = ticker.Price;
                    //UpdateTicker(c);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err);
                }
            }
            UpdateUI();
            Updating = false;
        }
        private async void UpdateTicker(Currency c)
        {
            Console.SetCursorPosition(c.SymbolPos.left, c.SymbolPos.top + 1);
            if (c.PreviousLast < c.Last) Console.BackgroundColor = ConsoleColor.Green;
            if (c.PreviousLast > c.Last) Console.BackgroundColor = ConsoleColor.Red;
            Console.Write(c.Last);
            //reset view
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
        private async Task<Chart> GenerateChart(Currency coin)
        {
            var candlestick = await binanceClient.GetCandleSticks(coin.Market, TimeInterval.Minutes_15);
            coin.Candles.AddRange(candlestick);
            //if (coin.Currency == "BTC" || coin.Currency == "USDT") return null;
            Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "Series_" + coin.Symbol,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.Candlestick,
                Color = Color.Red
            };

            Legend legend1 = new Legend();
            legend1.ForeColor = Color.LightGreen;
            legend1.BackColor = Color.Red;
            legend1.TitleForeColor = Color.White;

            ChartArea chartArea1 = new ChartArea();
            chartArea1.Name = "ChartArea_" + coin.Symbol;
            if (coin.Last == 0) coin.LastPurchasePrice = coin.ValueInBTC ;
            chartArea1.AxisY.Maximum = (double)(coin.Last + (coin.Last * 0.10m));
            chartArea1.AxisY.Minimum = (double)(coin.Last - (coin.Last * (decimal)0.10));
            chartArea1.AxisX.MajorGrid.LineWidth = 0;
            chartArea1.AxisY.MajorGrid.LineWidth = 0;
            chartArea1.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chartArea1.AxisX.Interval = 0;
            chartArea1.AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chartArea1.AxisY.Interval = 0;

            Chart chart1 = new Chart();

            chart1.DataManipulator.IsStartFromFirst = true;
            chart1.ChartAreas.Add(chartArea1);
            chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            //chart1.BackColor = Color.Black;
            chart1.ForeColor = Color.White;

            legend1.Name = "Legend_" + coin.Symbol;
            chart1.Legends.Add(legend1);
            chart1.Location = new System.Drawing.Point(0, 0);
            chart1.Name = "Chart_" + coin.Symbol;
            chart1.TabIndex = 0;
            chart1.Text = coin.Symbol;
            chart1.Series.Add(series1);



            //chart1.Series.Add(series2);

            //chart1.DataBind();

            //var StartDate = DateTime.Now.AddDays(-100).Ticks;
            //var EndDate = coin.Candles.OrderBy(d => d.CloseTime).Last().CloseTime;
            //var subsectionDate = StartDate;
            //Candlestick lastCandle = null;
            //var count = 1;
            //while (subsectionDate < EndDate)
            //{
            //    var relevantPrices = coin.Candles.Where(s => s.CloseTime >= subsectionDate && s.CloseTime <= subsectionDate.AddMinutes(5));
            //    if (relevantPrices.Count() == 0)
            //    {
            //        subsectionDate = subsectionDate.AddMinutes(5);
            //        count++;
            //        continue;
            //    }
            //    Candle candle = new Candle();
            //    candle.Open = Math.Round(relevantPrices.OrderBy(d => d.TimeStamp).FirstOrDefault().Last * coin.ValueInUSD);
            //    candle.Close = Math.Round(relevantPrices.OrderBy(d => d.TimeStamp).Last().Last * coin.ValueInUSD);
            //    candle.High = Math.Round(relevantPrices.Max(d => d.Last) * coin.ValueInUSD);
            //    candle.Low = Math.Round(relevantPrices.Min(d => d.Last) * coin.ValueInUSD);
            //    candle.TimeStamp = relevantPrices.OrderBy(d => d.TimeStamp).Last().TimeStamp;
            //    StockCandles.Add(candle);
            //    lastCandle = candle;
            //    subsectionDate = subsectionDate.AddMinutes(5);
            //}
            chart1.DataSource = coin.Candles;
            chart1.DataBind();
            return chart1;
        }
    }
}
