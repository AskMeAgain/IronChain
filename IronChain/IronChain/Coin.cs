using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronChain {
    [Serializable]
    public class Coin {
        public string owner;
        public string name;

        public Coin() {
            owner = "";
            name = "";
        }

        public Coin(string nam, string own) {
            owner = own;
            name = nam;
        }

    }
}
