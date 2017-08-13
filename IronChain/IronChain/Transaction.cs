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

        public Transaction(Random r) {

            id = r.Next(0,100000);
            coinName = ""+r.Next(3, 100);
            owner = "" + r.Next(3, 100);
            receiver = "" + r.Next(3, 100);
            proofOfOwnership = "" + r.Next(3, 100);
        }

        public Transaction() {

        }

        public string toString() {
            return "TransactionID:" + id + ", send " + coinName + " from " + owner + " to " + receiver;
        }

    }
}
