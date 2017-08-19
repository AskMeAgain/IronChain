using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace IronChain {
    public static class Utility {

        public static T loadFile<T>(string fileName) {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }

            T objectOut = default(T);

            try {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);

                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString)) {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read)) {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            } catch (Exception ex) {
                //Log exception here
            }

            return objectOut;
        }

        public static void storeFile<T>(T serializableObject, string fileName) {
            if (serializableObject == null) { return; }

            try {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream()) {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileName);
                    stream.Close();
                    Console.WriteLine("storing successfull");
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public static string ComputeHash(string filename) {
            using (var md5 = MD5.Create()) {
                using (var stream = File.OpenRead(filename + ".blk")) {
                    return Convert.ToBase64String(md5.ComputeHash(stream));
                }
            }
        }

        public static string getHashSha256(string text) {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash) {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }

        public static bool verifyHashDifficulty(string hash, int difficulty) {

            char[] charArr = hash.ToCharArray();
            for (int i = 0; i < difficulty; i++) {

                if (!charArr[i].Equals('0')) {
                    return false;
                }

            }

            return true;
        }

        public static string generateKeyFiles() {

            // Variables
            RSACryptoServiceProvider rsaProvider = null;
            string privateKey = "";

            rsaProvider = new RSACryptoServiceProvider(512);


            // Export keys
            XmlDocument xmlDoc = new XmlDocument();

            privateKey = rsaProvider.ToXmlString(true);
            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(privateKey);

            //Console.WriteLine(privateKey);

            //XmlNodeList test2 = xmlDoc.GetElementsByTagName("Modulus");

            //string[] s = new string[] { test2[0].InnerText , "lul" };

            string s = privateKey;

            Form1.instance.label1.Text = "done";
            Form1.instance.label6.Text = "";

            return s;
        }

        public static string decryptString(Account account , string s) {

            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(512);

            provider.FromXmlString(account.privateKey);

            var bytesCypherText = Convert.FromBase64String(s);         

            //decrypt and strip pkcs#1.5 padding
            var bytesPlainTextData = provider.Decrypt(bytesCypherText, false);

            //get our original plainText back...
            string plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);

            return plainTextData;
        }

        public static string encryptString(string publicKey, string s) {

            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(512);

            provider.FromXmlString(publicKey);

            var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(s);

            //apply pkcs#1.5 padding and encrypt our data 
            var bytesCypherText = provider.Encrypt(bytesPlainTextData, false);

            //we might want a string representation of our cypher text... base64 will do
            var cypherText = Convert.ToBase64String(bytesCypherText);

            return cypherText;
        }


    }
}
