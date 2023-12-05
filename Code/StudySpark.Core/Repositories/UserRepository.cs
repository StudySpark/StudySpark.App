using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace StudySpark.Core.Repositories
{
    public class UserRepository
    {
        private SqliteConnection conn;
        public UserRepository() 
        {
            try
            {
                // Create a new database connection:
                SqliteConnection sqlite_conn = new SqliteConnection("Data Source = ..\\..\\..\\..\\StudySpark.Core\\bin\\Debug\\net6.0\\database.db");
                // Open the connection:
                sqlite_conn.Open();
                this.conn = sqlite_conn;
                CreateTable();
            }
            catch (Exception ex) { }
        }
        private void CreateTable()
        {
            if (this.conn == null)
            {
                return;
            }
            SqliteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS User (username VARCHAR(256), password VARCHAR(64))";
            
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        public void createUser(string username, string password)
        {
            string originalString = "Hello, world!";
            
            byte[] key = Encoding.UTF8.GetBytes("StudySpark");
            byte[] iv = Encoding.UTF8.GetBytes("42069");

            string encryptedString = EncryptString(originalString, key, iv);

            SqliteCommand sqlite_cmd;
            string createsql = "INSERT INTO Users (Username, Password) VALUES(?,?)";
            SqliteCommand insertSQL = new SqliteCommand(createsql, conn);
            insertSQL.Parameters.Add(username);
            insertSQL.Parameters.Add(encryptedString);
            try
            {
                insertSQL.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        static string EncryptString(string plainText, byte[] key, byte[] iv)
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        static string DecryptString(string cipherText, byte[] key, byte[] iv)
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
