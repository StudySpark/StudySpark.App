using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.Core.Generic
{
    public class GenericBeerProduct
    {
        public int id;
        public int brandID;
        public string productname;
        public bool bookmarked;
        public string lowestprice;

        public GenericBeerProduct(int id, int brandID, string productName, int bookmarked, string lowestPrice)
        {
            this.id = id;
            this.brandID = brandID;
            this.productname = productName;
            if (bookmarked == 1)
            {
                this.bookmarked = true;
            } else
            {
                this.bookmarked = false;
            }
            this.lowestprice = lowestPrice;
        }
        public GenericBeerProduct(string productname, string lowestprice)
        {
            this.productname = productname;
            this.lowestprice = lowestprice;
        }
    }
}
