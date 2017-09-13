# IronChain

A blockchain made from ground up to as a personal challenge in C#. In the end it should have the same main features as the original bitcoin blockchain as described in the [Bitcoin whitepaper](https://bitcoin.org/bitcoin.pdf).

## General Features

Linking Blocks via hashes  
Proof-Of-Work mining  
RSA verification  
Account system  
Peer to Peer system  
All-in-One Client/Miner/Wallet software  
Local test suite  
Transaction Fees  
Transaction History  

PS: Note that this software only has a pseudo P2P system. Discovery is not possible without hosting a server or doing some trade offs.


## General Guide

![alt text](https://puu.sh/xz0o9/64641f92e6.png "Main Window")

1. Host your Server and connect to a server (if you connect over the internet you need to open port 4712 to host a server)
2. Wait 20 Seconds and the client should auto update the blocks (or go Window -> Test Window -> Download blocks from server)
3. Once updated, click on mine (recommended difficulty is 5 which takes 20-30 seconds).
4. If a block is found, your client will send the files to all connected clients.

## Transactions

Sending a transaction is self explanatory:
1. enter amount
2. enter transaction fee
3. click on send
4. If you are connected to some servers, the transaction will be pushed to all servers.

## Testing the IronChain

1. Open two instances of the program.
2. Open on each instance the test window (Window -> Test window)
3. Change the iron chain path of 
