# IronChain

A blockchain made from ground up to as a personal challenge in C#. In the end it should have the same main features as the original bitcoin blockchain described in the  [Bitcoin whitepaper](https://bitcoin.org/bitcoin.pdf).

## General Features of the IronChain

Linking Blocks via hashes  
Proof-Of-Work mining  
RSA verification  
Account system  
(pseudo) P2P system  
All-in-One Client/Miner/Wallet software  
Local test suite  
Transaction Fees  
Transaction History  

PS: Note that this software only has a pseudo P2P system. Doing a real P2P system is not possible without some trade offs, which iam not willing to do (hosting a server etc).


## General Guide

![alt text](https://puu.sh/xz0o9/64641f92e6.png "Main Window")

1. Host your Server and connect to a server (if you connect over the internet you need to open port 4712 to host a server, still WIP)
2. Wait 20 Seconds and the client should auto update the blocks (or go Window -> Test Window -> Download blocks from server)
3. Once updated, click on mine (recommended difficulty is 5 which takes 20-30 seconds).
4. If a block is found, your client will send the files to all connected clients
5. You can select which accounts balance will show up and you can decide which account will receive the mining reward if you find a block.
6. If you want to send some iron, you need to send it to their PUBLIC KEY. Never give out your private key. 

## Transactions

Sending a transaction is self explanatory:
1. enter amount
2. enter transaction fee
3. click on send  
If you are connected to some servers, the transaction will be pushed to all servers too.

## Account system

Your account contains your private key. Without private key, your iron is lost. So keep the .acc file if you want to use this program somewhere. If you want to create a new account, just click on File -> add account. You can drag and drop an .acc file too to import it.
Important to note is that when importing a file, the file is copied. Which means you will have two .acc files. One in C:\IronChain and one in the original location.

## Testing the IronChain

1. Open two instances of the program.
2. Open on each instance the test window (Window -> Test window)
3. Change the iron chain path of one instance
4. Each instance should host on a different port
5. Each instance needs to connect to the port of the other instance.
6. Disable auto download
7. Use the provided buttons to test each feature

## Current bugs and missing features

1. In theory you should be able to connect to others over the internet, in practice this is not working because of different things (router blocking connections, ISP's sometimes not using IPv6 and other stuff). I try to fix this, but testing it locally is proof enough that it works. 

2. You cant create an account from a single private key. 
