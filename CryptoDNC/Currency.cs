using System;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using System.Linq;
using System.Collections.Generic;
using Binance.API.Csharp.Client.Models.WebSocket;
using Binance.API.Csharp.Client.Models.Market;
public class Currency
{
    public string Symbol {get;set;}
    public decimal Balance {get;set;}
    public ConsolePosition SymbolPos {get;set;}
    public ConsolePosition BalancePos {get;set;}
    public decimal Last {get;set;}
    public decimal PreviousLast {get;set;}
    public List<Candlestick> Candles = new List<Candlestick>();
}
public class ConsolePosition {

    public int top {get;set;}
    public int left {get;set;}
}