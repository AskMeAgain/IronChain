﻿using System;
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
    class PeerNetworking {

        public static Dictionary<IPAddress, int> executerList;

        public PeerNetworking() {
            executerList = new Dictionary<IPAddress, int>();

        }

        public void ConnectToListener(string ip, int port) {

            try {

                //get local for test
                IPAddress[] addr = Dns.GetHostAddresses(Dns.GetHostName());

                //Socket socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                //socket.Connect(new IPEndPoint(addr[0], port));

                Console.WriteLine("added executer! (Socket)");
                executerList.Add(addr[0], port);

                //socket.Close();

            } catch (SocketException e) {
                if (e.ErrorCode.Equals(10048)) {
                    Console.WriteLine("Key exists already!");
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        public void requestFile() {
            sendCommandToServers(0x00);
        }

        public void pushFile() {
            sendCommandToServers(0x01);
        }

        private void sendCommandToServers(byte commandIndex) {

            //0 request file || 0, 8 block# = 9 bytes
            //1 new file mined || 1, 8 block# = 9 bytes
            Console.WriteLine("Sending?" + executerList.Count);

            foreach (IPAddress ip in executerList.Keys) {

                Socket inOut = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                inOut.Connect(new IPEndPoint(ip, executerList[ip]));

                if (inOut.Connected) {
                    Console.WriteLine("Connecting to this socket worked");
                } else {
                    Console.WriteLine("Connecting DID NOT WORK");
                    executerList.Remove(ip);
                }

                byte[] message = new byte[9];

                message = createHeaderMessage(Form1.instance.latestBlock, commandIndex);
                inOut.Send(message, 0, message.Length, SocketFlags.None);
                Console.WriteLine("Sending header");

                //receive acknowledgement:
                byte[] ack = new byte[32];
                inOut.Receive(ack, 32, SocketFlags.None);

                Console.WriteLine(BitConverter.ToInt64(ack, 8) + " << received ACK");

                if (commandIndex == 0x00) {
                    if (ack[0] == 0x00) {
                        Console.WriteLine("Receiving Block now!");
                        //now getting the files
                        receiveBlock(inOut, Form1.instance.latestBlock + 1, ack);
                        break;
                    } else {
                        Console.WriteLine("SERVER DOESNT HAVE THE FILES");
                        break;
                    }
                } else {

                    Console.WriteLine("SENDING NEW BLOCK HEIGHT WORKED!");

                }

                inOut.Close();
            }

        }

        private void receiveBlock(Socket inOut, int height, byte[] ack) {

            Console.WriteLine("receiving blockheight" + height);

            //receiving 3 files
            receiveFileAndStore(inOut, height + ".blk", BitConverter.ToInt64(ack, 8));
            receiveFileAndStore(inOut, "P" + height + ".blk", BitConverter.ToInt64(ack, 16));
            receiveFileAndStore(inOut, "L" + height + ".blk", BitConverter.ToInt64(ack, 24));

            Console.WriteLine("Reading 3 files now!");

            Form1.instance.analyseChain();

        }

        private void receiveFileAndStore(Socket serverStream, string filename, long fileSize) {

            string file = Form1.instance.globalChainPath + filename;

            if (File.Exists(file)) {
                File.Delete(file);
            }

            byte[] receiveBuffer = new byte[1024];
            FileStream fileStream = new FileStream(file, FileMode.Append);

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

                if (fileSize == 0)
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
                    listener.Listen(500);
                    while (true) {

                        Socket socket = listener.Accept();
                        Console.WriteLine("accepted connection!");

                        //receiving message from client
                        byte[] buffer = new byte[9];
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

            //0 means request
            //1 means a new file is up

            if (command[0] == 0x00) {

                int height = BitConverter.ToInt32(command, 1);

                Console.WriteLine("Received request of block" + height);

                //SEND ACKNOWLEDGEMENT
                byte[] ack = new byte[32];
                if (Form1.instance.latestBlock >= height) {

                    Console.WriteLine("Send ack then all files");
                    ack = createMessage(true, height);
                    socket.Send(ack, ack.Length, SocketFlags.None);

                    sendFile(height + ".blk", socket);
                    sendFile("P" + height + ".blk", socket);
                    sendFile("L" + height + ".blk", socket);

                } else {

                    Console.WriteLine("Stopped sending files!");
                    ack = createMessage(false, height);
                    socket.Send(ack, 0, ack.Length, SocketFlags.None);
                    Console.WriteLine("Done Sending error ack");
                }

            } else {

                //TODO REQUEST FILE FROM CLIENT!!
                Console.WriteLine("YOU HAVE A NEW FILE? ILL CHECK THAT OUT!");
                Console.WriteLine(executerList.Count + " << xount");
                requestFile();

            }
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

        private byte[] createMessage(bool fileExist, long blockheight) {

            byte[] message = Enumerable.Repeat((byte)0x00, 32).ToArray();

            if (!fileExist) {
                message[0] = 0x01;
            } else {

                byte[] fileA = File.ReadAllBytes(Form1.instance.globalChainPath + blockheight + ".blk");
                byte[] fileB = File.ReadAllBytes(Form1.instance.globalChainPath + "P" + blockheight + ".blk");
                byte[] fileC = File.ReadAllBytes(Form1.instance.globalChainPath + "L" + blockheight + ".blk");

                byte[] sizeA = BitConverter.GetBytes(fileA.Length);
                byte[] sizeB = BitConverter.GetBytes(fileB.Length);
                byte[] sizeC = BitConverter.GetBytes(fileC.Length);

                Array.Copy(sizeA, 0, message, 8, sizeA.Length);
                Array.Copy(sizeB, 0, message, 16, sizeB.Length);
                Array.Copy(sizeC, 0, message, 24, sizeC.Length);

                Console.WriteLine(BitConverter.ToInt64(message, 8));
                Console.WriteLine(BitConverter.ToInt64(message, 16));
                Console.WriteLine(BitConverter.ToInt64(message, 24));

            }

            return message;

        }

        private byte[] createHeaderMessage(long num, byte by) {

            if (by == 0x00)
                num++;

            byte[] message = new byte[9];

            message[0] = by;
            byte[] height = BitConverter.GetBytes(num);

            Array.Copy(height, 0, message, 1, height.Length);

            return message;

        }



    }
}