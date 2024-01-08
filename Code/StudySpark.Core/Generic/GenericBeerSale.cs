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
        public string expirationdate;

        public GenericBeerSale(int id, int productID, string store, string storeimage, string oldprice, string newprice, string expirationdate)
        {
            this.id = id;
            this.productID = productID;
            this.store = store;
            this.storeImage = storeimage;
            this.oldprice = oldprice;
            this.newprice = newprice;
            this.expirationdate = expirationdate;
        }
        public GenericBeerSale(string store, string storeimage, string oldprice, string newprice, string expirationdate)
        {
            this.store = store;
            this.storeImage = storeimage;
            this.oldprice = oldprice;
            this.newprice = newprice;
            this.expirationdate = expirationdate;
        }
    }
}
