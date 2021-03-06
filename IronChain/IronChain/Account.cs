﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace IronChain {
    [Serializable]
    public class Account {

        public string name;
        public string publicKey;
        public int analysedBlock;
        public int coinCounter;
        public string privateKey;

        public Account() {

        }

        public Account(string nam, int num) {
            name = nam;
            publicKey = nam;
            analysedBlock = num;

            addKeys();
        }

        public void addKeys() {

            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(512);

            privateKey = provider.ToXmlString(true);

            publicKey = provider.ToXmlString(false);

            var product = XElement.Parse(publicKey);
            publicKey = (string)product.Element("Modulus");

        }

        public override string ToString() {
            return name;
        }
    }
}
