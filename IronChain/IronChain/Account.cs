using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            privateKey = "lul";
        }

        public void addKeys(string[] s) {

            privateKey = s[0];
            publicKey = s[1];

        }



        public override string ToString() {
            return publicKey;
        }
    }
}
