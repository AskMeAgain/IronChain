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

        public Transaction(string name, int amou, string receiv) {

            Random r = new Random();

            id = r.Next(0,100000);

            coinName = ""+r.Next(3, 100);
            owner = name;
            receiver = receiv;
            proofOfOwnership = "" + r.Next(3, 100);
            amount = amou;
        }

        public Transaction() {

        }

        public string toString() {
            return "Transaction" + id + ": Sending " + amount + " from " + owner + " to " + receiver; 
        }

    }
}
