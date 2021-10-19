using DemoCryptography.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Tools.Cryptography.Asymmetric;

namespace DemoCryptography
{
    class Program
    {
        static void Main(string[] args)
        {
            //string passwd = "Test1234=";
            //byte[] passwdInBytes = Encoding.Unicode.GetBytes(passwd);

            //Console.WriteLine(passwd.Length);
            //Console.WriteLine(passwdInBytes.Length);

            #region Hashage
            //MD5 md5 = MD5.Create();
            //byte[] md5Hash = md5.ComputeHash(passwdInBytes);
            //Console.WriteLine(Convert.ToHexString(md5Hash));
            //Console.WriteLine(Convert.ToBase64String(md5Hash));
            //Console.WriteLine();

            //SHA1 sha1 = SHA1.Create();
            //byte[] sha1Hash = sha1.ComputeHash(passwdInBytes);
            //Console.WriteLine(Convert.ToHexString(sha1Hash));
            //Console.WriteLine(Convert.ToBase64String(sha1Hash));
            //Console.WriteLine();

            //SHA256 sha256 = SHA256.Create();
            //byte[] sha256Hash = sha256.ComputeHash(passwdInBytes);
            //Console.WriteLine(Convert.ToHexString(sha256Hash));
            //Console.WriteLine(Convert.ToBase64String(sha256Hash));
            //Console.WriteLine();

            //SHA512 sha512 = SHA512.Create();
            //byte[] sha512Hash = sha512.ComputeHash(passwdInBytes);
            //Console.WriteLine(Convert.ToHexString(sha512Hash));
            //Console.WriteLine(Convert.ToBase64String(sha512Hash));
            //Console.WriteLine();
            #endregion

            #region Cryptage Assymetrique
            // Génération des clés
            { 
                //RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider(2048);
                //Console.WriteLine(cryptoServiceProvider.KeySize);

                //byte[] binaryBothKeys = cryptoServiceProvider.ExportCspBlob(true);
                //byte[] binaryPublicKeys = cryptoServiceProvider.ExportCspBlob(false);

                ////Sauvegarde au format Binaire
                //File.WriteAllBytes("bothKeys.bin", binaryBothKeys);
                //File.WriteAllBytes("PublicKey.bin", binaryPublicKeys);

                //Console.WriteLine($"Both : {Convert.ToHexString(binaryBothKeys)}");
                //Console.WriteLine();
                //Console.WriteLine($"public : {Convert.ToHexString(binaryPublicKeys)}");
                //Console.WriteLine();

                //string stringBothKeys = cryptoServiceProvider.ToXmlString(true);
                //string stringPublicKeys = cryptoServiceProvider.ToXmlString(false);

                ////Sauvegarde au format XML
                //File.WriteAllText("bothKeys.xml", stringBothKeys);
                //File.WriteAllText("publicKey.xml", stringPublicKeys);

                //Console.WriteLine($"Both : {stringBothKeys}");
                //Console.WriteLine();
                //Console.WriteLine($"public : {stringPublicKeys}");
                //Console.WriteLine();
            }

            {
                //RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider();
                ////import du fichier Binaire
                //cryptoServiceProvider.ImportCspBlob(File.ReadAllBytes("bothKeys.bin"));
                ////import du fichier XML
                ////cryptoServiceProvider.FromXmlString(File.ReadAllText("bothKeys.xml"));

                //byte[] cryptedPasswd = cryptoServiceProvider.Encrypt(passwdInBytes, true);
                //Console.WriteLine(Convert.ToHexString(cryptedPasswd));
                //Console.WriteLine(Convert.ToBase64String(cryptedPasswd));

                //byte[] decryptedPasswd = cryptoServiceProvider.Decrypt(cryptedPasswd, true);
                //string newPasswd = Encoding.Unicode.GetString(decryptedPasswd);
                //Console.WriteLine(newPasswd);
            }

            //ICryptoRSA cryptoRSA = new CryptoRSA(File.ReadAllBytes("bothKeys.bin"));
            //Console.WriteLine(DateTime.Now);
            //ICryptoRSA cryptoRSA = new CryptoRSA(4096);
            //File.WriteAllBytes("rsa4096.bin", cryptoRSA.BothBinaryKeys);

            //byte[] cryptedPasswd = cryptoRSA.Encrypt(passwd);
            //string base64CryptedPasswd = Convert.ToBase64String(cryptedPasswd);
            //Console.WriteLine(Convert.ToHexString(cryptedPasswd));
            //Console.WriteLine(Convert.ToBase64String(cryptedPasswd));
            //Console.WriteLine(DateTime.Now);
            //byte[] receivedCryptedPasswd = Convert.FromBase64String(base64CryptedPasswd);
            //string newPasswd = cryptoRSA.DecryptAsString(receivedCryptedPasswd);
            //Console.WriteLine(newPasswd);
            //Console.WriteLine(DateTime.Now);

            #endregion

            #region ConsumeApi
            //Get Public Key
            User user = new User() { Nom = "Morre", Prenom = "Thierry", Email = "thierry.morre@cognitic.be", Passwd = "Test123456789+" };

            byte[] publicKey = null;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:5001/api/");

                using(HttpResponseMessage httpResponseMessage = httpClient.GetAsync("Security").Result)
                {
                    httpResponseMessage.EnsureSuccessStatusCode();

                    string json = httpResponseMessage.Content.ReadAsStringAsync().Result;
                    PublicKeyInfo publicKeyInfo = JsonSerializer.Deserialize<PublicKeyInfo>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    publicKey = Convert.FromBase64String(publicKeyInfo.PublicKey);
                }
            }

            ICryptoRSA cryptoRSA = new CryptoRSA(publicKey);
            byte[] cryptedPassword = cryptoRSA.Encrypt(user.Passwd);
            user.Passwd = Convert.ToBase64String(cryptedPassword);

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:5001/api/");
                HttpContent httpContent = new StringContent(JsonSerializer.Serialize(user));
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");                

                using (HttpResponseMessage httpResponseMessage = httpClient.PostAsync("Auth/Register", httpContent).Result)
                {
                    httpResponseMessage.EnsureSuccessStatusCode();
                }
            }
            #endregion
        }
    }
}
