using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronChain {
    [Serializable]
    public class Particle {

        public List<Transaction> allTransactions;
        public string hashToBlock;

        public Particle(){
            allTransactions = new List<Transaction>();
        }

    }
}
