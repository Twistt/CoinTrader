using System;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using System.Linq;
using System.Collections.Generic;
using Binance.API.Csharp.Client.Models.WebSocket;
using Binance.API.Csharp.Client.Models.Market;
using System.Drawing;
using System.IO;

public class Currency
{
    public string Symbol {get;set;}
    public decimal Balance {get;set;}
    public ConsolePosition SymbolPos {get;set;}
    public ConsolePosition BalancePos {get;set;}
    public decimal Last {get;set;}
    public decimal PreviousLast {get;set;}
    public Image Icon
    {
        get
        {
            if (File.Exists($"Icons\\{Symbol}.png")) return Image.FromFile($"Icons\\{Symbol}.png");
            else return Image.FromFile($"Icons\\BTC.png");

        }
    }

    public decimal LastPurchasePrice { get;  set; }
    public bool ShowinUSD { get;  set; }
    public decimal ValueInUSD { get;  set; }
    public decimal Profit { get;  set; }
    public string Market { get;  set; }
    public decimal ValueInBTC { get;  set; }

    public List<SymbolPrice> TickerData = new List<SymbolPrice>();
    public List<Candlestick> Candles = new List<Candlestick>();
}

public class ConsolePosition {

    public int top {get;set;}
    public int left {get;set;}
}