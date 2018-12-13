using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACPROTO.ACT.Models
{
    public interface IExchange
    {
        Task<Coin> GetCurrencyValue(Coin coin);

        Task<Ticker> UpdateTickers(Coin coin);
        Task<List<Order>> GetOrderHistory();
        Task<List<Coin>> GetBalances();
        Task<List<Order>> GetOpenOrders();
        Task<Order> SubmitBuyOrder(Coin coin, decimal amount);
        Task<Order> SellAll(Coin coin);

        Task<Order> SubmitSellOrder(Coin coin, decimal amount, decimal price);

        Task<List<Coin>> GetCurrencies();

    }

}
