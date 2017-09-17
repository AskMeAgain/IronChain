using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Reflection;

namespace IronChain {
    public class PeerNetworking {

        public static bool isServer = false;

        public static Dictionary<IPAddress, int> executerList;

        public PeerNetworking() {
            executerList = new Dictionary<IPAddress, int>();
        }

        public void ConnectToListener(IPAddress ip, int port) {

            try {
                executerList.Add(ip, port);
                Console.WriteLine("worked?");
            } catch (SocketException e) {
                if (e.ErrorCode.Equals(10048)) {
                    Console.WriteLine("Key exists already!");
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        public void requestFileInfo() {
            sendCommandToServers(0x00);
        }

        public void pushFile() {
            sendCommandToServers(0x01);
        }

        public void pushTransactionToServers() {
            sendCommandToServers(0x02);
        }

        Transaction selectedTransforTransmitting;

        long highestHeight;
        IPAddress ipOfHighest;
        int portOfHighest;
        bool flagForFileInfo;


        private void sendCommandToServers(byte commandIndex) {

            Thread a = new Thread(() => {

                //0 request file info || 0, 8 block# = 9 bytes
                //1 new file mined || 1, 8 block# = 9 bytes
                //2 push transaction || 2, transaction hash = 9 bytes
                //3 download whole chain

                if (commandIndex == 0x02 && Form1.instance.TransactionPool.Count == 0) {
                    Console.WriteLine("meme pool is empty!!");
                    return;
                }

                highestHeight = 0;
                flagForFileInfo = false;

                Console.WriteLine("executing command to {0} users", executerList.Count);

                foreach (IPAddress ip in executerList.Keys) {
                    try {

                        Socket inOut = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                        inOut.Connect(new IPEndPoint(ip, executerList[ip]));

                        //send command to server!
                        byte[] command = new byte[32];
                        command = createCommandMessage(Form1.instance.latestBlock, commandIndex);
                        inOut.Send(command, 0, command.Length, SocketFlags.None);

                        //receive answer
                        byte[] answer = new byte[32];
                        Console.WriteLine("waiting answer!");

                        inOut.Receive(answer, 32, SocketFlags.None);

                        Console.WriteLine("received answer!");

                        //do stuff with specific answer
                        if (commandIndex == 0x00) {
                            //store information here for further progressing
                            Console.WriteLine("storing information for each server");

                            if (answer[0] == 0x00) {
                                Console.WriteLine("You have height {0} and the hash existed before!", BitConverter.ToInt64(answer, 1));

                                if (highestHeight < BitConverter.ToInt64(answer, 1)) {
                                    highestHeight = BitConverter.ToInt64(answer, 1);
                                    ipOfHighest = ip;
                                    portOfHighest = executerList[ip];
                                    flagForFileInfo = true;
                                }

                            } else if (answer[0] == 0x01) {
                                Console.WriteLine("You have height XX, but the hash doesnt exist");
                            } else if (answer[0] == 0x02) {
                                Console.WriteLine("Your height is smaller!");
                            }

                        } else if (commandIndex == 0x01) {

                            if (answer[0] == 0x00) 
                                Console.WriteLine("everything worked");
                            


                        } else if (commandIndex == 0x02) {
                            if (answer[0] == 0x00)
                                sendTransaction(inOut);

                        }

                        inOut.Close();
                    } catch (Exception e) {
                        Console.WriteLine(e.ToString());
                    }
                }

                if (flagForFileInfo && highestHeight > Form1.instance.latestBlock) {

                    Console.WriteLine("init process to download from highest node!");
                    flagForFileInfo = false;

                    downloadChain(ipOfHighest, portOfHighest);

                }

            });

            a.Start();

        }

        public void downloadChain(IPAddress ip, int port) {

            Console.WriteLine("DOWNLOADING CHAIN!!");
            Socket inOut = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            inOut.Connect(new IPEndPoint(ip, port));

            Console.WriteLine("requesting height " + (Form1.instance.latestBlock + 1));

            //send command 0x03!
            byte[] message = new byte[32];
            message[0] = 0x03;
            byte[] num = BitConverter.GetBytes((long)Form1.instance.latestBlock + 1);
            Array.Copy(num, 0, message, 8, num.Length);
            inOut.Send(message, 0, message.Length, SocketFlags.None);


            //receive filesizes
            byte[] filesizes = new byte[32];
            inOut.Receive(filesizes, 0, filesizes.Length, SocketFlags.None);

            if (filesizes[0] == 0x01) {
                Console.WriteLine("file doesnt exist!");
                inOut.Close();
                return;
            }

            //receive block
            receiveBlock(inOut, Form1.instance.latestBlock + 1, filesizes);

            downloadChain(ip, port);

        }

        private void sendTransaction(Socket inOut) {
            Console.WriteLine("Sending transaction now because everything went fine!!!!");



            byte[] transObj = Utility.ObjectToByteArray(selectedTransforTransmitting);
            byte[] msg = new byte[4];
            msg = BitConverter.GetBytes(transObj.Length);

            Console.WriteLine("filesize" + transObj.Length);
            inOut.Send(msg, 0, msg.Length, SocketFlags.None);

            inOut.Send(transObj, 0, transObj.Length, SocketFlags.None);
        }

        private void receiveBlock(Socket inOut, int height, byte[] ack) {

            Console.WriteLine("receiving blockheight" + height);

            //receiving 3 files
            receiveFileAndStore(inOut, height + ".blk", BitConverter.ToInt64(ack, 8));
            receiveFileAndStore(inOut, "P" + height + ".blk", BitConverter.ToInt64(ack, 16));
            receiveFileAndStore(inOut, "E" + height + ".blk", BitConverter.ToInt64(ack, 24));

            foreach (Transaction trans in Form1.instance.usedTransactions) {
                Form1.instance.TransactionPool.Add(trans);
            }

            Form1.instance.analyseChain();

        }

        private void receiveFileAndStore(Socket serverStream, string filename, long fileSize) {

            string file = Form1.instance.globalChainPath + filename;
            FileStream fileStream;

            if (File.Exists(file)) {
                fileStream = new FileStream(file, FileMode.Create);
            } else {
                fileStream = new FileStream(file, FileMode.Append);
            }

            byte[] receiveBuffer = new byte[1024];
           

            int counter = 0;
            Console.WriteLine("Storing file with " + fileSize);

            while (true) {

                int endpointer = receiveBuffer.Length;

                if (fileSize <= receiveBuffer.Length) {
                    endpointer = (int)fileSize;
                }

                int receivedBytes = serverStream.Receive(receiveBuffer, endpointer, SocketFlags.None);
                fileSize -= receivedBytes;

                fileStream.Write(receiveBuffer, 0, receivedBytes);
                Console.WriteLine("?" + receivedBytes);

                if (fileSize == 0 || receivedBytes == 0)
                    break;
            }

            fileStream.Close();
            Console.WriteLine("Received" + counter);
        }

        public void ListenForConnections(int port) {

            Thread thread = new Thread(() => {
                try {
                    Socket listener = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

                    listener.Bind(new IPEndPoint(IPAddress.IPv6Any, port));
                    Console.WriteLine("Binding Socket");
                    listener.Listen(500);

                    isServer = true;

                    while (true) {

                        Socket socket = listener.Accept();
                        Console.WriteLine("accepted connection!");

                        //receiving message from client
                        byte[] buffer = new byte[32];
                        socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                        Console.WriteLine("received command");
                        //sending acknowledgement back
                        commandReceived(buffer, socket);

                        socket.Close();

                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            });

            thread.IsBackground = true;
            thread.Start();
        }

        private void commandReceived(byte[] command, Socket socket) {

            //0 means broadcast file height info
            //1 means a new file is up
            //2 means receive a new transaction
            //3 means client wants to download files from X to latest block

            if (command[0] == 0x00) {

                sendFileInfo(command, socket);

            } else if (command[0] == 0x01) {
                //socket.Close();

                byte[] msg = new byte[32];
                msg[0] = 0x00;

                socket.Send(msg, 0, msg.Length, SocketFlags.None);
                socket.Close();

                requestFileInfo();

            } else if (command[0] == 0x02) {

                receiveTransaction(socket);

            } else if (command[0] == 0x03) {

                Console.WriteLine("download request received!");
                Console.WriteLine("requested block {0}", BitConverter.ToInt64(command, 8));

                long height = BitConverter.ToInt64(command, 8);

                byte[] filesizes = createFileSizeMessage(height);
                socket.Send(filesizes, 0, filesizes.Length, SocketFlags.None);

                if (height <= Form1.instance.latestBlock) {

                    sendFile(height + ".blk", socket);
                    sendFile("P" + height + ".blk", socket);
                    sendFile("E" + height + ".blk", socket);

                }

                socket.Close();
            }

        }

        private void sendFileInfo(byte[] command, Socket socket) {

            int requestedHeight = BitConverter.ToInt32(command, 1);
            string hash = Convert.ToBase64String(command, 10, 16);
            byte[] ack = new byte[32];

            Console.WriteLine("sending file info");

            if (requestedHeight > Form1.instance.latestBlock) {
                Console.WriteLine("smaller!");
                ack[0] = 0x02;
            } else if (requestedHeight <= Form1.instance.latestBlock) {
                Console.WriteLine(requestedHeight + " " + Form1.instance.latestBlock);
                if (Utility.ComputeHash(Form1.instance.globalChainPath + (requestedHeight - 1)).Equals(hash)) {
                    ack[0] = 0x00;
                } else {
                    ack[0] = 0x01;
                }
            } else {
                //block doesnt exist, but i have height XXX
                Console.WriteLine("block doesnt exist!");
                ack[0] = 0x01;
            }

            byte[] num = BitConverter.GetBytes((long)Form1.instance.latestBlock);
            Array.Copy(num, 0, ack, 1, num.Length);

            socket.Send(ack, ack.Length, SocketFlags.None);

        }

        private void receiveTransaction(Socket socket) {
            Console.WriteLine("you have a transaction? ok sending confirmation ");
            byte[] msg = new byte[32];
            msg[0] = 0x00;
            socket.Send(msg, 0, msg.Length, SocketFlags.None);

            byte[] filesize = new byte[4];
            socket.Receive(filesize, 0, filesize.Length, SocketFlags.None);
            Console.WriteLine("received answer!" + BitConverter.ToInt32(filesize, 0));

            int bytesNeeded = BitConverter.ToInt32(filesize, 0);
            byte[] transaction = new byte[bytesNeeded];
            socket.Receive(transaction, 0, bytesNeeded, SocketFlags.None);
            Transaction t = Utility.ByteArrayToTransaction(transaction);

            Form1.instance.TransactionPool.Add(t);

            socket.Close();
        }

        private void sendFile(string name, Socket serverStream) {

            byte[] buffer = new byte[1024];

            FileStream File = new FileStream(Form1.instance.globalChainPath + name, FileMode.Open, FileAccess.Read);
            byte[] fileArray = System.IO.File.ReadAllBytes(Form1.instance.globalChainPath + name);
            File.Close();

            int fileLength = fileArray.Length;

            int startIndex = 0;
            int bytesMissing;

            do {

                bytesMissing = fileLength - startIndex;
                int endPointer = buffer.Length;

                if (bytesMissing < buffer.Length) {
                    endPointer = bytesMissing;
                }

                serverStream.Send(fileArray, startIndex, endPointer, SocketFlags.None);
                startIndex += buffer.Length;
            } while (startIndex < fileLength);

            Console.WriteLine("Finished sending file!" + name + " " + startIndex);

        }

        private byte[] createFileSizeMessage(long blockheight) {

            bool fileExist = false;

            Console.WriteLine(Form1.instance.globalChainPath + blockheight + ".blk");

            if (File.Exists(Form1.instance.globalChainPath + blockheight + ".blk")) {
                fileExist = true;
                Console.WriteLine("requested height EXISTS!");
            } else {
                Console.WriteLine("request height DOESNT EXIST");
            }

            byte[] message = Enumerable.Repeat((byte)0x00, 32).ToArray();

            if (!fileExist) {
                message[0] = 0x01;
                Console.WriteLine("message 0x01");
            } else {

                byte[] fileA = File.ReadAllBytes(Form1.instance.globalChainPath + blockheight + ".blk");
                byte[] fileB = File.ReadAllBytes(Form1.instance.globalChainPath + "P" + blockheight + ".blk");
                byte[] fileC = File.ReadAllBytes(Form1.instance.globalChainPath + "E" + blockheight + ".blk");

                byte[] sizeA = BitConverter.GetBytes(fileA.Length);
                byte[] sizeB = BitConverter.GetBytes(fileB.Length);
                byte[] sizeC = BitConverter.GetBytes(fileC.Length);

                Array.Copy(sizeA, 0, message, 8, sizeA.Length);
                Array.Copy(sizeB, 0, message, 16, sizeB.Length);
                Array.Copy(sizeC, 0, message, 24, sizeC.Length);

            }

            return message;

        }

        private byte[] createCommandMessage(long num, byte by) {

            byte[] message = new byte[32];
            message[0] = by;

            Console.WriteLine("creating command");

            if (by == 0x00) {
                //request file info
                string hash = Utility.ComputeHash(Form1.instance.globalChainPath + num);
                byte[] s = Convert.FromBase64String(hash);
                Array.Copy(s, 0, message, 10, s.Length);
                num++;
            }

            if (by == 0x02) {
                Transaction t = Form1.instance.TransactionPool[0];
                selectedTransforTransmitting = t;
                Form1.instance.TransactionPool.RemoveAt(0);
                num = t.GetHashCode();
                Console.WriteLine(num + " << hashcode");
            }


            byte[] height = BitConverter.GetBytes(num);

            Array.Copy(height, 0, message, 1, height.Length);

            return message;

        }



    }
}
