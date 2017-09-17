using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IronChain {
    [Serializable]
    public class Transaction {

        public string id;
        public string owner;
        public string receiver;
        public string proofOfOwnership;
        public int amount;
        public string data;
        public int transactionfee;

        public Transaction() {

        }

        public Transaction(string key, string receiv, int am, string privateKey, long latestblock) {
            owner = key;
            receiver = receiv;
            amount = am;
            giveID();
            proofOfOwnership = Utility.signData(Utility.getHashSha256(id + ""), privateKey);
        }

        public void giveID() {
            id = Utility.getHashSha256(owner + receiver + data + amount);
        }

        public string toString() {
            return id + ": Sending " + amount + " from " + owner + " to " + receiver; 
        }

    }
}
