using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.Core.Generic
{
    public class GenericBeerSale
    {
        public int id;
        public int productID;
        public string store;
        public string storeImage;
        public string oldprice;
        public string newprice;
        public string img;

        public GenericBeerSale(int id, int productID, string store, string storeimage, string oldprice, string newprice)
        {
            this.id = id;
            this.productID = productID;
            this.store = store;
            this.storeImage = storeimage;
            this.oldprice = oldprice;
            this.newprice = newprice;
        }
        public GenericBeerSale(string store, string storeimage, string oldprice, string newprice)
        {
            this.store = store;
            this.storeImage = storeimage;
            this.oldprice = oldprice;
            this.newprice = newprice;
        }
    }
}
