# IronChain

A blockchain made from ground up to provide a feature for using unused (mined) blocks and a sharding solution for Users.

The idea is simple: a mined block doesnt point to the block before, but points to multiple blocks which store the transaction. These blocks (called Particles) point to the block before, resulting in a "sharded" chain. 

## Particles
We group the transactions by their first digit into these particles. Since users are only interested in specific addresses (account balance etc), they can just delete the other Particles. 

If we group the transactions into 100 Particles, we can reduce the blocksize for a normal user by 100, because the rest can be deleted. We could go further and further, but the overhead for having multiple files could be to much. We will see how it goes when the IronChain is finished. 


## Assimilation of Blocks
###### Note: I still need to find a sufficient algorithm for this. Maybe its just not possible who knows
Rejecting a block is a waste of hashpower, so why not use it? When a node is verifying a block and it finds a collision with another block mined by a different user (think of two miners found the solution at the same time), it gets posted in the collision pool. When a miner finds the solution to the next block, he can point his block to the blocks from the collision pool. One block gets selected as the main block (where only the particles of that transaction are pointing too) and the other one will be the side block. A side block is directly pointed to from the next block. 

#### Fraud collision with assimilation
Scenario: User A sends a transaction to B in the mainblock, but user A also sends a transaction to C in the sideblock. Checking every combination is tedious for the node, so we use a little trick: Since we group the transactions by sender name, we just accept the mainblock as the main transactions and when a group collides with the sideblock, the sideblock's particles get deleted. The best thing for miners would be to select random name ranges to make sure the collision stays low. Miners who mined a sideblock also receive coins, but they are deducted for every collision on the mainblock. This makes sure that every miner spreads out his range.  
Note: this system could be shit and not worth it, but its worth to try it out
