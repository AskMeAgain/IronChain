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
                xmlDocument.Load(fileName+".blk");
                
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
                    xmlDocument.Save(fileName+ ".blk");
                    stream.Close();
                    Form1.instance.addToLog("Storing \"" + fileName +  "\" succesful");
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                Form1.instance.addToLog("Storing \"" + fileName + "\" NOT succesful");

            }
        }

        public static  string ComputeHash(string filename) {
            using (var md5 = MD5.Create()) {
                using (var stream = File.OpenRead(filename+".blk")) {
                    return Convert.ToBase64String(md5.ComputeHash(stream));
                }
            }
        }
    }
}
