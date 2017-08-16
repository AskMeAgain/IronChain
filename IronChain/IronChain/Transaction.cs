using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronChain {
    [Serializable]
    public class Transaction {

        public int id;

        public string coinName;
        public string owner;
        public string receiver;
        public string proofOfOwnership;
        public int amount;

        public Transaction(Random r, string name) {

            id = r.Next(0,100000);
            coinName = ""+r.Next(3, 100);
            owner = name;
            receiver = "" + r.Next(3, 100);
            proofOfOwnership = "" + r.Next(3, 100);
            amount = 3;
        }

        public Transaction() {

        }

        public string toString() {
            return "TransactionID:" + id + ", send " + coinName + " from " + owner + " to " + receiver;
        }

    }
}
