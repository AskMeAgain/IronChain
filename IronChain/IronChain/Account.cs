using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronChain {
    public class Account {

        public string publicKey;
        public int analysedBlock;
        public int coinCounter;

        public Account(string key, int num) {
            publicKey = key;
            analysedBlock = num;
        }

        public override string ToString() {
            return publicKey;
        }
    }
}
