using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace IronChain {
    class PeerNetworking {

        Dictionary<string, int> executerList;

        public PeerNetworking() {
            executerList = new Dictionary<string, int>();
        }

        public void ConnectToListener(string ip, int port) {

            try {
                //TcpClient client = new TcpClient(ip, port);
                Console.WriteLine("added executer! (ip and port)");
                executerList.Add(ip, port);
            } catch (SocketException e) {
                if (e.ErrorCode.Equals(10048)) {
                    Console.WriteLine("Key exists already!");
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        public void sendCommandToServers(byte commandIndex) {

            //0 request file || 0, 8 block# = 9 bytes
            //1 new file mined || 1, 8 block# = 9 bytes

            foreach (string s in executerList.Keys) {

                TcpClient c = new TcpClient(s, executerList[s]);
                Stream inOut = c.GetStream();

                if (!inOut.CanWrite) {
                    Console.WriteLine("this should not appear :(");
                }

                byte[] message = new byte[9];

                message = createHeaderMessage(Form1.instance.latestBlock, commandIndex);

                inOut.Write(message, 0, message.Length);

                if (commandIndex == 0x00) {
                    Console.WriteLine("Receiving Block now!");
                    //now getting the files
                    receiveBlock(inOut);
                } else {
                    Console.WriteLine("Sending to server, nothing here right now");
                    //requesting files here
                }



                inOut.Close();
            }

        }

        private void receiveBlock(Stream inOut) {

            //receive acknowledgement:
            byte[] ack = new byte[32];
            inOut.Read(ack, 0, 32);

            Console.WriteLine(BitConverter.ToInt64(ack,8) + " << received ACK");

            //receiving 3 files
            TODO
            Console.WriteLine("Reading 3 files now!");


        }

        public void ListenForConnections(int port) {

            Form1.instance.button1.Enabled = false;
            Form1.instance.button1.Text = "Listening on Port " + port;

            Thread thread = new Thread(() => {

                TcpListener listener = new TcpListener(IPAddress.IPv6Any, port);

                listener.Start();

                while (true) {

                    TcpClient c = listener.AcceptTcpClient();
                    Console.WriteLine("accepted connection!");
                    Stream serverStream = c.GetStream();

                    //receiving message from client
                    byte[] buffer = new byte[9];
                    serverStream.Read(buffer, 0, buffer.Length);

                    //sending acknowledgement back
                    commandReceived(buffer, serverStream);

                    serverStream.Close();
                    c.Close();
                }

            });

            thread.IsBackground = true;
            thread.Start();
        }

        private void commandReceived(byte[] command, Stream serverStream) {

            //0 means request
            //1 means a new file is up

            Console.WriteLine("Command received, height " + BitConverter.ToInt64(command, 1));

            if (command[0] == 0x00) {

                Console.WriteLine("Requesting File:");
                int height = BitConverter.ToInt32(command, 1);

                Console.WriteLine("Received FileHeader with height " + height);

                //SEND ACKNOWLEDGEMENT
                byte[] ack = new byte[32];
                if (Form1.instance.latestBlock >= height) {

                    Console.WriteLine("Send ack then all files");
                    ack = createMessage(true, height);
                    serverStream.Write(ack, 0, ack.Length);
                    
                    TODO
                    /*
                    sendFile(height + ".blk", serverStream);
                    sendFile("P" + height + ".blk", serverStream);
                    sendFile("L" + height + ".blk", serverStream);
                    */
                } else {
                    Console.WriteLine("here?");
                    ack[0] = 0x01;
                    serverStream.Write(ack, 0, ack.Length);
                }

                Console.WriteLine("Stopped sending files!");
            } else {

                //TODO REQUEST FILE FROM CLIENT!!

            }
        }

        private void sendFile(string name, Stream serverStream) {

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

                serverStream.Write(fileArray, startIndex, endPointer);
                startIndex += buffer.Length;
            } while (startIndex < fileLength);

            Console.WriteLine("Finished sending file!" + name + " " + startIndex);

        }

        private byte[] createMessage(bool fileExist, long blockheight) {

            byte[] message = Enumerable.Repeat((byte)0x00, 32).ToArray();

            if (!fileExist) {
                message[0] = 0x01;
            }

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
