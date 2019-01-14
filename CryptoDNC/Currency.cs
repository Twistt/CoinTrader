using System;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using System.Linq;
using System.Collections.Generic;
using Binance.API.Csharp.Client.Models.WebSocket;
using Binance.API.Csharp.Client.Models.Market;
using Trady.Analysis;

public class Currency
{
    public string Symbol {get;set;}
    public string Market {get;set;}
    public decimal Balance {get;set;}
    public ConsolePosition SymbolPos {get;set;}
    public ConsolePosition BalancePos {get;set;}
    public decimal Last {get;set;}
    public decimal PreviousLast {get;set;}
    public List<Candlestick> Candles = new List<Candlestick>();
    public decimal ValueInUSD {get;set;}
    public decimal ValueInBTC {get;set;}
    public decimal? sma { get; internal set; }
    public decimal? sd { get; internal set; }
    public decimal? smaRDiff { get; internal set; }
    public decimal? smaDiff { get; internal set; }
}
public class ConsolePosition {

    public int top {get;set;}
    public int left {get;set;}
}