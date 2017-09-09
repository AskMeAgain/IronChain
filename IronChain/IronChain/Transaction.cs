using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IronChain {
    [Serializable]
    public class Transaction {

        public long id;
        public string owner;
        public string receiver;
        public string proofOfOwnership;
        public int amount;

        public Transaction() {

        }

        public Transaction(string key, string receiv, int am) {
            owner = key;
            receiver = receiv;
            amount = am;
        }

        public void giveID(int height) {
            long num = owner.GetHashCode() - receiver.GetHashCode();
            id = Convert.ToInt64(string.Format("{0}{1}{2}", height, amount, num));
        }

        public string toString() {
            return id + ": Sending " + amount + " from " + owner + " to " + receiver; 
        }

    }
}
