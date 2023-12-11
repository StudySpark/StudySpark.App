﻿using System;
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
        public string oldprice;
        public string newprice;

        public GenericBeerSale(int id, int productID, string store, string oldprice, string newprice)
        {
            this.id = id;
            this.productID = productID;
            this.store = store;
            this.oldprice = oldprice;
            this.newprice = newprice;
        }
    }
}
