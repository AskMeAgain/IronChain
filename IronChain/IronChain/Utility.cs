using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                xmlDocument.Load("C:\\IronChain\\" + fileName);

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
                    xmlDocument.Save("C:\\IronChain\\" + fileName);
                    stream.Close();
                    Console.WriteLine("storing successfull");
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public static string ComputeHash(string filename) {
            using (var md5 = MD5.Create()) {
                using (var stream = File.OpenRead("C:\\IronChain\\" + filename + ".blk")) {
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

        public static void loadSettings() {

            Settings.SettingsObject settings = loadFile<Settings.SettingsObject>("settings.set");

            //load values from settingsobject to everything else;
            Form1.instance.minerAccountName = settings.defaultMiningAccountName;
            Form1.instance.miningDifficulty = settings.defaultMiningDifficulty;

            if (Form1.instance.accountList.ContainsKey(settings.defaultMainAccount))
                Form1.instance.comboBox1.SelectedItem = Form1.instance.accountList[settings.defaultMainAccount];

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
