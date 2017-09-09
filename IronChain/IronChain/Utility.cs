using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace IronChain {
    public static class Utility {

        static string RSAKEYPatternStart = "<RSAKeyValue><Modulus>";
        static string RSAKEYPatternEnd = "</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

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
                Console.WriteLine(ex.ToString());
            }

            return objectOut;
        }

        public static string buildRealPublicKey(string s) {
            return RSAKEYPatternStart + s + RSAKEYPatternEnd;
        }

        public static void storeFile<T>(T serializableObject, string fileName) {
            Console.WriteLine("storing file at: " + fileName);
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

        public static string decryptString(string key, string s) {

            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(512);

            var bytesCypherText = Convert.FromBase64String(s);

            var bytesPlainTextData = provider.Decrypt(bytesCypherText, false);

            string plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);

            return plainTextData;
        }

        public static string encryptString(string key, string s) {

            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(512);

            provider.FromXmlString(key);

            var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(s);

            //apply pkcs#1.5 padding and encrypt our data 
            var bytesCypherText = provider.Encrypt(bytesPlainTextData, false);

            //we might want a string representation of our cypher text... base64 will do
            var cypherText = Convert.ToBase64String(bytesCypherText);

            return cypherText;
        }

        [Serializable]
        public class Settings{
            public string mainAccount;
            public string minerAccount;
            public string globalChainPath;

            public Settings() {
                globalChainPath = "C:\\IronChain\\";
            }
        }

        public static byte[] ObjectToByteArray(object obj) {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream()) {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static Transaction ByteArrayToTransaction(byte[] arrBytes) {

            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Transaction obj = (Transaction)binForm.Deserialize(memStream);

            return obj;
        }

        public static void loadSettings() {

            Settings sett = loadFile<Settings>("C:\\IronChain\\settings.set");

            if (sett == null) {
                sett = new Settings();
                sett.globalChainPath = "C:\\IronChain\\";
                sett.minerAccount = findRandomAcc();
                sett.mainAccount = findRandomAcc();

                storeFile(sett, "C:\\IronChain\\settings.set");
                Console.WriteLine("here??");
            }

            Form1.instance.minerAccountName = sett.minerAccount;
            Form1.instance.mainAccount = sett.mainAccount;
            Form1.instance.globalChainPath = sett.globalChainPath;
        }

        private static string findRandomAcc() {
            string[] allAccountNames = Directory.GetFiles("C:\\IronChain\\", "*.acc");

            if (allAccountNames.Length == 0) {

                return "";

            }

            Account a = loadFile<Account>(allAccountNames[0]);
            return a.name;
        }

        public static string signData(string message, string privateKey) {

            byte[] plainText = ASCIIEncoding.Unicode.GetBytes(message);

            var rsaWrite = new RSACryptoServiceProvider();
            rsaWrite.FromXmlString(privateKey);

            byte[] signature = rsaWrite.SignData(plainText, new SHA1CryptoServiceProvider());

            return Convert.ToBase64String(signature);
        }

        public static bool verifyData(string orig, string publicKey, string sign) {

            byte[] signature = Convert.FromBase64String(sign);
            byte[] original = ASCIIEncoding.Unicode.GetBytes(orig);

            var rsaRead = new RSACryptoServiceProvider();
            rsaRead.FromXmlString(publicKey);

            if (rsaRead.VerifyData(original, new SHA1CryptoServiceProvider(), signature)) {
                return true;
            } else {
                return false;
            }
        }
    }
}
