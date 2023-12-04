using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Static
{
    public static class ProductPrices
    {

        public static Products prices;

        static ProductPrices()
        {
            prices = new Products();
            prices.AddQuantity("wood", 78);
            prices.AddQuantity("stone", 87);
            prices.AddQuantity("iron", 116);
            prices.AddQuantity("goldore", 468);
            prices.AddQuantity("fish", 81);
            prices.AddQuantity("wheat", 73);
            prices.AddQuantity("bread", 136);
            prices.AddQuantity("fruit", 193);
            prices.AddQuantity("flour", 116);
            prices.AddQuantity("herb", 180);
            prices.AddQuantity("wine", 395);
            prices.AddQuantity("cotton", 234);
            prices.AddQuantity("spice", 346);
            prices.AddQuantity("jewelry", 1017);
            prices.AddQuantity("meat", 261);
            prices.AddQuantity("vegetable", 170);
            prices.AddQuantity("clothes", 352);
        }
    }
}
