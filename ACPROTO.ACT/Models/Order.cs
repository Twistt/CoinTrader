using System;
using System.Collections.Generic;

namespace ACPROTO.ACT
{
    public class Order
    {
        public string Uuid { get; set; }
        public string OrderUuid { get; set; }
        public string Exchange { get; set; }
        public string OrderType { get; set; }
        public bool Buy { get; set; }
        public decimal Price { get; set; }
        public decimal PricePerUnit { get; set; }
        public DateTime? Opened { get; set; }
        public DateTime? Closed {get;set;}
        public bool? CancelInitiated { get; set; }
        public bool ImmediateOrCancel { get; set; }
        public bool IsConditional { get; set; }


        public decimal Limit { get; set; }
        public decimal Cost { get; set; }
        public string Type { get; set; }
        public DateTime TimeStamp { get; internal set; }
        public string Market { get; internal set; }
        public decimal Quantity { get; internal set; }
        public decimal QuantityRemaining { get; internal set; }

        public static List<Order> AllOrders = new List<Order>();
        public decimal Commission { get; set; }
    }
}