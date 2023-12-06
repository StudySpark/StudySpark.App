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


        public void createUser(string username, string password)
        {            
            byte[] key = Encoding.UTF8.GetBytes("1jlSTUDYSPARKbzJPAuhjXAQluf/e5e4");
            byte[] iv = Encoding.UTF8.GetBytes("420694206942069F");

            string encryptedString = EncryptString(password, key, iv);

            SqliteCommand sqlite_cmd;
            string createsql = "INSERT INTO Users (Username, Password) VALUES(@Username, @Password)";
            SqliteCommand insertSQL = new SqliteCommand(createsql, DBRepository.Conn);
            insertSQL.Parameters.Add(new SqliteParameter("@Username", username));
            insertSQL.Parameters.Add(new SqliteParameter("@Password", encryptedString));
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
