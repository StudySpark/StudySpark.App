using Microsoft.Data.Sqlite;
using StudySpark.Core.FileManager;
using StudySpark.Core.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.Core.Repositories
{
    public class BeerRepository
    {
        public int getBrandId(string brand)
        {
            int brandID;
            switch (brand)
            {
                case "Hertog Jan":
                    brandID = 0;
                    break;
                case "Amstel":
                    brandID = 1;
                    break;
                case "Heineken":
                    brandID = 2;
                    break;
                case "Grolsch":
                    brandID = 3;
                    break;
                case "Jupiler":
                    brandID = 4;
                    break;
                default:
                    brandID = -1;
                    break;
            }
            return brandID;
        }

        public bool checkBookMark(string productname, string brandname)
        {
            SqliteCommand sqlite_cmd;
            sqlite_cmd = DBRepository.Conn.CreateCommand();
            SqliteDataReader reader;
            int brandID = getBrandId(brandname);

            sqlite_cmd.CommandText = "SELECT bookmarked FROM BeerProducts WHERE productname = @param1 AND brandID = @param2";
            sqlite_cmd.Parameters.Add(new SqliteParameter("@param1", productname));
            sqlite_cmd.Parameters.Add(new SqliteParameter("@param2", brandID));

            reader = sqlite_cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetInt32(0) == 1)
                {
                    return true;
                }
            }
            return false;
        }
        public void removeBookMark(int productID)
        {
            SqliteCommand sqlite_cmd;
            sqlite_cmd = DBRepository.Conn.CreateCommand();

            sqlite_cmd.CommandText = "DELETE FROM BeerSales WHERE ProductID = @param1";
            sqlite_cmd.Parameters.Add(new SqliteParameter("@param1", productID));

            sqlite_cmd.ExecuteNonQuery();
            removeProduct(productID);

        }
        public void removeProduct(int productID)
        {
            SqliteCommand sqlite_cmd;
            sqlite_cmd = DBRepository.Conn.CreateCommand();

            sqlite_cmd.CommandText = "DELETE FROM BeerProducts WHERE id = @param1";
            sqlite_cmd.Parameters.Add(new SqliteParameter("@param1", productID));

            sqlite_cmd.ExecuteNonQuery();

        }
        public void insertBeersale(List<Dictionary<GenericBeerProduct, List<GenericBeerSale>>> products)
        {
            SqliteCommand sqlite_cmd;

            foreach (Dictionary<GenericBeerProduct, List<GenericBeerSale>> dict in products) {
                foreach(var (key, value)  in dict)
                {        
                    sqlite_cmd = DBRepository.Conn.CreateCommand();
                    sqlite_cmd.CommandText = "INSERT INTO BeerProducts (brandID, productname, bookmarked, lowestprice) VALUES(@brandID, @productname, @bookmarked, @lowestprice)";
                    sqlite_cmd.Parameters.Add(new SqliteParameter("@brandID", key.brandID));
                    sqlite_cmd.Parameters.Add(new SqliteParameter("@productname", key.productname));
                    sqlite_cmd.Parameters.Add(new SqliteParameter("@bookmarked", key.bookmarked));
                    sqlite_cmd.Parameters.Add(new SqliteParameter("@lowestprice", key.lowestprice));

                    sqlite_cmd.ExecuteNonQuery();

                    foreach (GenericBeerSale sale in value)
                    {
                        GenericBeerProduct lastProd = getLastInserted();
                        insertSale(lastProd.id, sale.store, sale.storeImage, sale.oldprice, sale.newprice, sale.expirationdate);
                    }

                }
            }


        }
        public void insertBookMark(string brand, string productname, int bookmarked, string lowestprice)
        {
            int brandID = getBrandId(brand);
            SqliteCommand sqlite_cmd;
            sqlite_cmd = DBRepository.Conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO BeerProducts (brandID, productname, bookmarked, lowestprice) VALUES(@brandID, @productname, @bookmarked, @lowestprice)";
            sqlite_cmd.Parameters.Add(new SqliteParameter("@brandID", brandID));
            sqlite_cmd.Parameters.Add(new SqliteParameter("@productname", productname));
            sqlite_cmd.Parameters.Add(new SqliteParameter("@bookmarked", bookmarked));
            sqlite_cmd.Parameters.Add(new SqliteParameter("@lowestprice", lowestprice));

            sqlite_cmd.ExecuteNonQuery();

        }
        public void insertSale(int productID, string store, string storeimage, string oldprice, string newprice, string expirationdate)
        {

            SqliteCommand sqlite_cmd = DBRepository.Conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO BeerSales (productID, store, storeimage, oldprice, newprice, expirationdate) VALUES(@productID, @store, @storeimage, @oldprice, @newprice, @expirationdate)";
            sqlite_cmd.Parameters.Add(new SqliteParameter("@productID", productID));
            sqlite_cmd.Parameters.Add(new SqliteParameter("@store", store));
            sqlite_cmd.Parameters.Add(new SqliteParameter("@storeimage", storeimage));
            sqlite_cmd.Parameters.Add(new SqliteParameter("@oldprice", oldprice));
            sqlite_cmd.Parameters.Add(new SqliteParameter("@newprice", newprice));
            sqlite_cmd.Parameters.Add(new SqliteParameter("@expirationdate", expirationdate));

            sqlite_cmd.ExecuteNonQuery();
        }
        public List<GenericBeerProduct> getBookMarked()
        { 
            if (DBRepository.Conn == null)
            {
                return new List<GenericBeerProduct>();
            }

            List<GenericBeerProduct> products = new List<GenericBeerProduct>();

            SqliteDataReader reader;
            SqliteCommand sqlite_cmd;

            sqlite_cmd = DBRepository.Conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM BeerProducts WHERE bookmarked = 1";

            reader = sqlite_cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int brandID = reader.GetInt32(1);
                string productname = reader.GetString(2);
                int bookmarked = reader.GetInt32(3);
                string lowestprice = reader.GetString(4);

                GenericBeerProduct product = new GenericBeerProduct(id, brandID, productname, bookmarked, lowestprice);
                products.Add(product);
            }
            return products;
        }
        public List<GenericBeerSale> getSales(int productID)
        {

            if (DBRepository.Conn == null)
            {
                return new List<GenericBeerSale>();
            }

            List<GenericBeerSale> sales = new List<GenericBeerSale>();

            SqliteDataReader reader;
            SqliteCommand sqlite_cmd;

            sqlite_cmd = DBRepository.Conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM BeerSales WHERE productID = @param1";
            sqlite_cmd.Parameters.Add(new SqliteParameter("@param1", productID));

            reader = sqlite_cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int prodID = reader.GetInt32(1);
                string store = reader.GetString(2);
                string storeimage = reader.GetString(3);
                string oldprice = reader.GetString(4);
                string newprice = reader.GetString(5);
                string expirationdate = reader.GetString(5);

                GenericBeerSale sale = new GenericBeerSale(id, prodID, store, storeimage, oldprice, newprice, expirationdate);
                sales.Add(sale);
            }
            return sales;
        }
        public List<GenericBeerProduct> getBeerSales()
        {
            if (DBRepository.Conn == null)
            {
                return new List<GenericBeerProduct>();
            }

            List<GenericBeerProduct> products = new List<GenericBeerProduct>();

            SqliteDataReader reader;
            SqliteCommand sqlite_cmd;

            sqlite_cmd = DBRepository.Conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM BeerProducts";

            reader = sqlite_cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int brandID = reader.GetInt32(1);
                string productname = reader.GetString(2);
                int bookmarked = reader.GetInt32(3);
                string lowestprice = reader.GetString(4);

                GenericBeerProduct product = new GenericBeerProduct(id, brandID, productname, bookmarked, lowestprice);
                products.Add(product);
            }
            return products;
        }
        public List<GenericBeerProduct> getLastBookMarked()
        {

            if (DBRepository.Conn == null)
            {
                return new List<GenericBeerProduct>();
            }

            List<GenericBeerProduct> products = new List<GenericBeerProduct>();

            SqliteDataReader reader;
            SqliteCommand sqlite_cmd;

            sqlite_cmd = DBRepository.Conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM BeerProducts WHERE bookmarked = @param1 ORDER BY id DESC LIMIT 1";
            sqlite_cmd.Parameters.Add(new SqliteParameter("@param1", 1));

            reader = sqlite_cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int brandID = reader.GetInt32(1);
                string productname = reader.GetString(2);
                int bookmarked = reader.GetInt32(3);
                string lowestprice = reader.GetString(4);

                GenericBeerProduct product = new GenericBeerProduct(id, brandID, productname, bookmarked, lowestprice);
                products.Add(product);
            }
            return products;
        }
        public GenericBeerProduct getLastInserted()
        {
            GenericBeerProduct? product = null;
            if (DBRepository.Conn == null)
            {
                return product;
            }

            SqliteDataReader reader;
            SqliteCommand sqlite_cmd;

            sqlite_cmd = DBRepository.Conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM BeerProducts ORDER BY id DESC LIMIT 1";

            reader = sqlite_cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int brandID = reader.GetInt32(1);
                string productname = reader.GetString(2);
                int bookmarked = reader.GetInt32(3);
                string lowestprice = reader.GetString(4);

                product = new GenericBeerProduct(id, brandID, productname, bookmarked, lowestprice);
            }
            return product;
        }
    }
}
