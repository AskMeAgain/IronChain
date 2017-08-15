# IronChain

A blockchain made from ground up to see if i can get a working blockchain done.





##### How does it work?

Each transaction is stored inside a transaction pool. Once a miner finds the next block, we collect all transactions and store them inside a file (i called them particles). This file also stores the hash to the block before. The new mined block will store the hash of the particle. This construct makes it impossible to change the data inbetween. 

Right now you can still change the latest block, this will be fixed in a later version.

##### Features

Lightmode  
Send your transactions  
Mine blocks  
Receive coins  


