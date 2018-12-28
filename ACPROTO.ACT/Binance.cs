using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ACPROTO.ACT.Models;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using Binance.API.Csharp.Client.Utils;
using Newtonsoft.Json;

namespace ACPROTO.ACT
{
    public class BinanceWrapper: IExchange
    {
        //API Key:
        //z3ZYAyTdnIfPqNq0AkRviMKdTDF36GknKofFnQ5J0LrxQOdcfvuBVgNVBE4vBaCI
        //Secret Key:  For your security, your API Secret Key will only be displayed at the time it is created.If you lose this key, you will need to delete your API and set up a new one.
        //YGWPMVcjZVVHaiSDxNH3n7GaII3lrvNoTWefmNkq5T2b9wqXe2ibT3GIHTQukoV3

        const string apikey = "z3ZYAyTdnIfPqNq0AkRviMKdTDF36GknKofFnQ5J0LrxQOdcfvuBVgNVBE4vBaCI";
        const string apisecret = "YGWPMVcjZVVHaiSDxNH3n7GaII3lrvNoTWefmNkq5T2b9wqXe2ibT3GIHTQukoV3";


        BinanceClient bin = new BinanceClient(new ApiClient(apikey, apisecret, "https://api.binance.com"));
        public BinanceWrapper() {
            var apiClient = new ApiClient("z3ZYAyTdnIfPqNq0AkRviMKdTDF36GknKofFnQ5J0LrxQOdcfvuBVgNVBE4vBaCI", "YGWPMVcjZVVHaiSDxNH3n7GaII3lrvNoTWefmNkq5T2b9wqXe2ibT3GIHTQukoV3");
            var binanceClient = new BinanceClient(apiClient);
         
            var test = bin.GetOrderBook("TRXBTC");
            var test2 = bin.GetAccountInfo();

        }

        public async Task<List<Coin>> GetBalances()
        {
            var info = await bin.GetAccountInfo();

            foreach (var item in info.Balances)
            {
                Coin coin = Coin.AllCoins.Where(c => c.Currency == item.Asset).FirstOrDefault();

                if (coin == null)
                {
                    coin = new Coin();
                    coin.Balance = item.Free;
                    coin.Currency = item.Asset;
                    coin.MarketName = item.Asset;
                    Coin.AllCoins.Add(coin);
                }
                else
                {
                    coin.Balance = item.Free;
                }
            }
            return Coin.AllCoins;
        }

        public Task<List<Coin>> GetCurrencies()
        {
            return null;
        }

        public Task<Coin> GetCurrencyValue(Coin coin)
        {
            return null;
        }
        public async Task<List<Candlestick>> GetCandlesticks(string market, TimeInterval time)
        {
            var sticks = await bin.GetCandleSticks(market, time) ;
            return sticks.ToList();
        }

        public async Task<List<Order>> GetOrderHistory()
        {
            return new List<Order>();
            //this doesnt work as it needs a symbol to look up.
            var orders = await bin.GetAllOrders(null);
            foreach (var order in orders)
            {
                var o = new Order() { OrderUuid = order.ClientOrderId, Cost= order.Price, Price=order.Price, Quantity= order.OrigQty, QuantityRemaining=order.IcebergQty};
                Order.AllOrders.Add(o);
            }
            return Order.AllOrders;

        }
        public async Task<List<Order>> GetOpenOrders()
        {
            var orders = await bin.GetCurrentOpenOrders(null);
            foreach (var order in orders)
            {
                var o = new Order() { OrderUuid = order.ClientOrderId, Cost = order.Price, Price = order.Price, Quantity = order.OrigQty, QuantityRemaining = order.IcebergQty };
                Order.AllOrders.Add(o);
            }
            return Order.AllOrders;

        }

        public Task<Order> SellAll(Coin coin)
        {
            throw new NotImplementedException();
        }

        public Task<Order> SubmitBuyOrder(Coin coin, decimal amount)
        {
            throw new NotImplementedException();
        }

        public Task<Order> SubmitSellOrder(Coin coin, decimal amount, decimal price)
        {
            throw new NotImplementedException();
        }

        public decimal TotalPortfolioValue()
        {
            throw new NotImplementedException();
        }

        public async Task<TickerData> UpdateTickers(Coin coin)
        {
            var tickerPrices = await bin.GetAllPrices();
            foreach (var item in tickerPrices.ToList())
            {
                var acoin = Coin.AllCoins.Where(c => c.MarketName == coin.Currency).FirstOrDefault();
                coin.ValueInBTC = item.Price;
                acoin.TickerData.Add(ticker);
                if (coin.Currency == item.Symbol) {
                    coin.TickerData.Add(ticker);
                }

            }
            return coin.TickerData.Last();
            
            //////throw new NotImplementedException();
        }
    }
}
