using ACPROTO.ACT.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ACPROTO.ACT
{
    public partial class ACT : Form
    {
        Timer timerTicker = new Timer() { Interval = 2000, Enabled = false };
        Timer timerUpdateCurrencies = new Timer() { Interval = 30000, Enabled = false };
        static bool needUpdateUI = false;
        static bool marketQueryRunning = false;
        decimal BTCValue = 0.00m;
        public ACT()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Get a list of all coins (and whether we have any)
            Exchange.GetBalances();
            var usdt = Coin.AllCoins.Where(c => c.Currency == "USDT").FirstOrDefault();
            var btc = Coin.AllCoins.Where(c => c.Currency == "BTC").FirstOrDefault();
            //Get a list of all the stuff we ahve bought and sold (to establish profit and baseline)
            Exchange.GetOrderHistory();
            foreach (var coin in Coin.AllCoins.Where(c => c.Balance > 0 && c.Currency != "BTC" && c.Currency != "USDT"))
            {
                var mostrecentorder = Order.AllOrders.Where(o => o.Exchange == "BTC-" + coin.Currency).OrderByDescending(o => o.TimeStamp).FirstOrDefault();
                if (mostrecentorder != null) coin.LastPurchasePrice = mostrecentorder.PricePerUnit;
                Exchange.UpdateTickers(coin);
                flowLayoutPanel1.Controls.Add(CreateCurrencyGroup(coin));
            }
            FillOrders();
            FillAlerts();
            timerTicker.Tick += UpdateCurrencyTickers;
            timerTicker.Start();
            timerUpdateCurrencies.Tick += TimerUpdateCurrencies_Tick;
            timerUpdateCurrencies.Start();
            //This handles new currencies (trading on the exchange and not through this interface)
            BTCValue = Coin.AllCoins.Where(c=>c.Currency == "BTC").First().ValueInUSD;
            this.Text = "CryptoDayTrader BTC:" + BTCValue;
            lblTotalPortfolioValueUSD.Text = "Total Value in USD: $" + Math.Round(Coin.AllCoins.Sum(c => (c.LastPurchasePrice * c.Balance) * BTCValue), 4).ToString();
            lblTotalPortfolioValueBTC.Text = "Total Value in BTC: " + Coin.AllCoins.Sum(c => (c.LastPurchasePrice * c.Balance)).ToString();
            Task.Factory.StartNew(()=> { 
                //This adds ADDITIONAL information to currencies - so we dont need it right away.
                Exchange.GetCurrencies();
            });

        }

        private void TimerUpdateCurrencies_Tick(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Exchange.GetOrderHistory();
                Exchange.GetBalances();
            });
        }
        private Chart GenerateChart(Coin coin)
        {
            if (coin.Currency == "BTC" || coin.Currency == "USDT") return null;
            Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "Series_" + coin.Currency,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.Line,
                Color = Color.Red
            };

            Legend legend1 = new Legend();
            legend1.ForeColor = Color.LightGreen;
            legend1.BackColor = Color.Red;
            legend1.TitleForeColor = Color.White;

            ChartArea chartArea1 = new ChartArea();
            chartArea1.Name = "ChartArea_" + coin.Currency;
            if (coin.LastPurchasePrice == 0) coin.LastPurchasePrice = coin.ValueInBTC;
            chartArea1.AxisY.Maximum = (double)(coin.LastPurchasePrice + (coin.LastPurchasePrice * 0.10m));
            chartArea1.AxisY.Minimum = (double)(coin.LastPurchasePrice - (coin.LastPurchasePrice * (decimal)0.10));
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

            legend1.Name = "Legend_" + coin.Currency;
            chart1.Legends.Add(legend1);
            chart1.Location = new System.Drawing.Point(0, 0);
            chart1.Name = "Chart_" + coin.Currency;
            chart1.TabIndex = 0;
            chart1.Text = coin.Currency;
            chart1.Series.Add(series1);
            //chart1.Series.Add(series2);

            //chart1.DataBind();

            List<Candle> StockCandles = new List<Candle>();
            var StartDate = DateTime.Now.AddDays(-100);
            var EndDate = coin.TickerData.OrderBy(d => d.TimeStamp).Last().TimeStamp;
            var subsectionDate = StartDate;
            Candle lastCandle = null;
            var count = 1;
            while (subsectionDate < EndDate)
            {
                var relevantPrices = coin.TickerData.Where(s => s.TimeStamp >= subsectionDate && s.TimeStamp <= subsectionDate.AddMinutes(5));
                if (relevantPrices.Count() == 0)
                {
                    subsectionDate = subsectionDate.AddMinutes(5);
                    count++;
                    continue;
                }
                Candle candle = new Candle();
                candle.Open = Math.Round(relevantPrices.OrderBy(d => d.TimeStamp).FirstOrDefault().Last * coin.ValueInUSD);
                candle.Close = Math.Round(relevantPrices.OrderBy(d => d.TimeStamp).Last().Last * coin.ValueInUSD);
                candle.High = Math.Round(relevantPrices.Max(d => d.Last) * coin.ValueInUSD);
                candle.Low = Math.Round(relevantPrices.Min(d => d.Last) * coin.ValueInUSD);
                candle.TimeStamp = relevantPrices.OrderBy(d => d.TimeStamp).Last().TimeStamp;
                StockCandles.Add(candle);
                lastCandle = candle;
                subsectionDate = subsectionDate.AddMinutes(5);
            }
            chart1.DataSource = StockCandles;
            return chart1;
        }
        private GroupBox CreateCurrencyGroup(Coin coin)
        {
            if ((coin.ValueInUSD * coin.Balance) < 1) return null;
            GroupBox gbCurrency = new System.Windows.Forms.GroupBox();
            PictureBox pictureBox1 = new System.Windows.Forms.PictureBox()
            {
                Location = new System.Drawing.Point(6, 19),
                Name = "pb" + coin.Currency,
                Size = new System.Drawing.Size(56, 50),
                Image = coin.Icon,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            pictureBox1.Click += (o, i) =>
            {
                System.Diagnostics.Process.Start("https://bittrex.com/Market/Index?MarketName=" + coin.MarketName);
            };

            Label lblBuyPrice = new System.Windows.Forms.Label()
            {
                AutoSize = true,
                Location = new System.Drawing.Point(130, 20),
                Name = "lblBuyPrice",
                Size = new System.Drawing.Size(35, 13),
                Text = coin.LastPurchasePrice.ToString()
            };
            lblBuyPrice.Click += (o,ev)=> {
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
            Label label1 = new System.Windows.Forms.Label() {
                Name = "lblBuy",
                AutoSize = true,
                Location = new System.Drawing.Point(65, 20),
                Size = new System.Drawing.Size(55, 13),
                Text = "Buy Price:" };
            Label label2 = new System.Windows.Forms.Label() {
                AutoSize = true,
                Location = new System.Drawing.Point(65, 38), Name = "label2",
                Size = new System.Drawing.Size(57, 13), TabIndex = 3,
                Text = "USD Value:" };
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

            Label label12 = new System.Windows.Forms.Label {
                AutoSize = true,
                Location = new System.Drawing.Point(100, 116),
                Name = "lblLast",
                Size = new System.Drawing.Size(35, 13),
                TabIndex = 4,
                Text = coin.TickerData.Count > 0 ? coin.TickerData.Last().Last.ToString() : "0"

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
            gbCurrency.Name = "gb" + coin.Currency;
            gbCurrency.Size = new System.Drawing.Size(201, 256);
            gbCurrency.TabIndex = 1;
            gbCurrency.TabStop = false;
            gbCurrency.Text = $"{coin.CurrencyLong} Tikr({coin.Currency}) Amt({coin.Balance})";

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
                Text = coin.TickerData.Count > 0 ? coin.TickerData.Last().Bid.ToString() : 0.ToString()
            };
            Label btnSell = new Label { Text = "Sell", Location = new System.Drawing.Point(5, 75), Size = new System.Drawing.Size(56, 20), FlatStyle = FlatStyle.Flat, BorderStyle = BorderStyle.FixedSingle };
            btnSell.Click += (o, ev) =>
            {
                Exchange.SellAll(coin);
            };

            Label lblAsk = new System.Windows.Forms.Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(100, 99),
                Name = "lblAsk",
                Size = new System.Drawing.Size(35, 13),
                TabIndex = 4,
                Text = coin.TickerData.Count > 0 ?  coin.TickerData.Last().Ask.ToString() : "0",

            };
            Label btnBuy = new Label { Text = "Buy", Location = new System.Drawing.Point(5, 95), Size = new System.Drawing.Size(56, 20), FlatStyle = FlatStyle.Flat, BorderStyle = BorderStyle.FixedSingle };
            btnBuy.Click += (o, ev) =>
            {
                Exchange.SubmitBuyOrder(coin);
            };

            Label btnSell10percent = new Label { Text = "Sell@10%", Location = new System.Drawing.Point(5, 115), Size = new System.Drawing.Size(56, 20), FlatStyle = FlatStyle.Flat, BorderStyle = BorderStyle.FixedSingle };
            btnSell10percent.Click += (o, ev) =>
            {
                //Add 10% to the purchase cost and enter the sell order.
                Exchange.SubmitSellOrder(coin, coin.Balance, coin.LastPurchasePrice + (coin.LastPurchasePrice * 0.1m));
            };


            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(3, 132);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(195, 121);
            panel1.TabIndex = 6;

            panel1.Controls.Add(GenerateChart(coin));
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
        private void FillOrders()
        {
            foreach (var order in Order.AllOrders.OrderByDescending(o=>o.TimeStamp))
            {
                flpOrders.Controls.Add(new Label {
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))) ,
                    Name = "lbl"+order.OrderUuid, Text=$"{order.Exchange} {order.OrderType} {order.Limit} {order.PricePerUnit}",
                    Width = flpOrders.Width-10,
                    ForeColor = order.Closed == null ?  Color.Green : Color.LightGray
                });
            }
        }
        private void FillAlerts()
        {
            foreach (var coin in Coin.AllCoins)
            {
                var message = string.Empty;
                var order = Order.AllOrders.Where(c => c.Exchange == coin.MarketName).FirstOrDefault();

                if (order != null)
                {
                    if (coin.ValueInBTC < order.PricePerUnit && order.OrderType == "LIMIT_SELL") message = $"{coin.Currency} Cost is Lower than last sell price";
                    if (coin.ValueInBTC > order.PricePerUnit && order.OrderType == "LIMIT_BUY") message = $"{coin.Currency} Cost is Lower than last BUY price";
                    var lblAlert = new Label
                    {
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                        Name = "lbl" + order.OrderUuid,
                        Text = message,
                        Width = flpOrders.Width - 10,
                        ForeColor = Color.Green
                    };
                    lblAlert.Click += (o,ev)=> {
                        System.Diagnostics.Process.Start("https://bittrex.com/Market/Index?MarketName=" + coin.MarketName);
                    };
                    if (message!= string.Empty) flpAlerts.Controls.Add(lblAlert);
                }
            }
        }

        public void UpdateUI()
        {
            foreach (var coin in Coin.AllCoins.Where(C => C.Balance == 0) )
            {
                GroupBox gb = (GroupBox)flowLayoutPanel1.Controls["gb" + coin.Currency];
                if (gb != null)
                {
                    flowLayoutPanel1.Controls.Remove(gb);
                }
            }
            foreach (var coin in Coin.AllCoins.Where(c => c.Balance > 0 && c.Currency != "BTC" && c.Currency != "USDT" && (c.Balance * c.ValueInUSD) > 1))
            {
                GroupBox gb = (GroupBox)flowLayoutPanel1.Controls["gb" + coin.Currency];
                if (gb != null && coin.TickerData.Count > 0)
                {
                    if (gb.Text != $"{coin.CurrencyLong}-{coin.Currency} (Amt({Math.Round(coin.Balance,4)}))") gb.Text = $"{coin.CurrencyLong}-{coin.Currency} (Amt({Math.Round(coin.Balance, 4)}))";
                    try
                    {
                        var mostrecentorder = Order.AllOrders.Where(o => o.Exchange == "BTC-" + coin.Currency).OrderByDescending(o => o.TimeStamp).FirstOrDefault();
                        if (mostrecentorder != null) coin.LastPurchasePrice = mostrecentorder.PricePerUnit;
                    }
                    catch (Exception) { }
                    ((Control)gb.Controls.Find("lblBuyPrice", false).FirstOrDefault()).Text = coin.ShowinUSD ? "$" + Math.Round(coin.LastPurchasePrice * BTCValue, 4).ToString() : Math.Round(coin.LastPurchasePrice,10).ToString();
                    ((Control)gb.Controls.Find("lblBuyPrice", false).FirstOrDefault()).ForeColor = coin.ShowinUSD ? Color.Green : Color.Blue;

                    ((Control)gb.Controls.Find("lblBid", false).FirstOrDefault()).Text = coin.TickerData.Last().Bid.ToString();
                    ((Control)gb.Controls.Find("lblAsk", false).FirstOrDefault()).Text = coin.TickerData.Last().Ask.ToString();
                    ((Control)gb.Controls.Find("lblLast", false).FirstOrDefault()).Text = coin.TickerData.Last().Last.ToString();
                    ((Control)gb.Controls.Find("lblProfit", false).FirstOrDefault()).Text = coin.ShowinUSD ? "$" + Math.Round(coin.Profit * BTCValue, 4).ToString() : Math.Round(coin.Profit,10).ToString();
                    ((Control)gb.Controls.Find("lblProfit", false).FirstOrDefault()).ForeColor = coin.Profit >= 0 ? Color.Green : Color.Red;
                    //((Control)gb.Controls.Find("btnCurrencyDisplay", false).FirstOrDefault()).ForeColor = coin.ShowinUSD ? Color.Green : Color.Blue;
                    ((Control)gb.Controls.Find("lblUSDValue", false).FirstOrDefault()).Text = "$" +Math.Round(coin.ValueInUSD * coin.Balance,4).ToString();
                    lblTotalPortfolioValueUSD.Text = "Total Value in USD: $" + Math.Round(Coin.AllCoins.Sum(c => (c.LastPurchasePrice * c.Balance) * BTCValue), 4).ToString();
                    lblTotalPortfolioValueBTC.Text = "Total Value in BTC: " + Coin.AllCoins.Sum(c => (c.LastPurchasePrice * c.Balance)).ToString();

                    Chart chart = (Chart)gb.Controls.Find("Chart_" + coin.Currency, true).FirstOrDefault();
                    if (chart != null)
                    {
                        var series_bid = chart.Series[0];
                        series_bid.Points.AddY((decimal)coin.TickerData.Last().Last);

                    }
                }
                else
                {
                    Exchange.UpdateTickers(coin);
                    Exchange.GetOrderHistory();
                    flowLayoutPanel1.Controls.Add(CreateCurrencyGroup(coin));
                }
            }
            needUpdateUI = false;
        }
        public void UpdateCurrencyTickers(object sender, EventArgs e)
        {
            if (!marketQueryRunning)
            {
                marketQueryRunning = true;

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                    foreach (var coin in Coin.AllCoins.Where(c => c.Balance > 0 && c.Balance * c.ValueInUSD > 1 && c.Currency != "BTC"))
                        {
                            var mostrecentorder = Order.AllOrders.Where(o => o.Exchange == "BTC-" + coin.Currency).OrderByDescending(o => o.TimeStamp).FirstOrDefault();
                            coin.LastPurchasePrice = mostrecentorder.PricePerUnit;
                            Exchange.UpdateTickers(coin);
                        }
                        needUpdateUI = true;
                        marketQueryRunning = false;
                    }
                    catch (Exception err) {
                        Console.WriteLine(err.Message);
                        marketQueryRunning = false;
                    }
                });

            }
            if (needUpdateUI) UpdateUI();
        }
    }
}
