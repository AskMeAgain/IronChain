using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronChain {
    [Serializable]
    public class Transaction {

        public int id;

        public string owner;
        public string receiver;
        public string proofOfOwnership;
        public int amount;

        public Transaction() {

        }

        public Transaction(int id) {
            this.id = id;
        }

        public string toString() {
            return "Transaction" + id + ": Sending " + amount + " from " + owner + " to " + receiver; 
        }

    }
}
