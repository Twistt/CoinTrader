using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACPROTO.ACT.Models
{
    public class Candle
    {

        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public DateTime TimeStamp { get; internal set; }

    }
}
