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

                ping(ip, port);

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

            foreach (string s in executerList.Keys) {

                TcpClient c = new TcpClient(s, executerList[s]);
                Stream inOut = c.GetStream();

                byte[] message = new byte[9];

                message = createHeaderMessage(Form1.instance.latestBlock, commandIndex);
                inOut.Write(message, 0, message.Length);

                //receive acknowledgement:
                byte[] ack = new byte[32];
                inOut.Read(ack, 0, 32);

                Console.WriteLine(BitConverter.ToInt64(ack, 8) + " << received ACK");

                if (commandIndex == 0x00) {
                    if (ack[0] == 0x00) {
                        Console.WriteLine("Receiving Block now!");
                        //now getting the files
                        receiveBlock(inOut, Form1.instance.latestBlock + 1, ack);
                    } else {
                        Console.WriteLine("THERE ARE NO FILES HERE");
                    }
                } else {
                    Console.WriteLine("REQUESTING FILES WORKED!");
                    //request files now from everyone
                }

                inOut.Close();
            }

        }

        private void receiveBlock(Stream inOut, int height, byte[] ack) {

            Console.WriteLine("receiving blockheight" + height);

            //receiving 3 files
            receiveFileAndStore(inOut, height + ".blk", BitConverter.ToInt64(ack, 8));
            receiveFileAndStore(inOut, "P" + height + ".blk", BitConverter.ToInt64(ack, 16));
            receiveFileAndStore(inOut, "L" + height + ".blk", BitConverter.ToInt64(ack, 24));

            Console.WriteLine("Reading 3 files now!");

            Form1.instance.analyseChain();

        }

        private void receiveFileAndStore(Stream serverStream, string filename, long fileSize) {

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

                int receivedBytes = serverStream.Read(receiveBuffer, 0, endpointer);
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

            Form1.instance.button1.Enabled = false;
            Form1.instance.button1.Text = "Listening on Port " + port;

            Thread thread = new Thread(() => {

                TcpListener listener = new TcpListener(IPAddress.IPv6Any, port);

                listener.Start();

                while (true) {

                    TcpClient c = listener.AcceptTcpClient();
                    Console.WriteLine("accepted connection!");
                    Stream serverStream = c.GetStream();

                    //storing them as executer
                    string ip = ((IPEndPoint)c.Client.RemoteEndPoint).Address.ToString();
                    int s = ((IPEndPoint)c.Client.RemoteEndPoint).Port;


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

                    sendFile(height + ".blk", serverStream);
                    sendFile("P" + height + ".blk", serverStream);
                    sendFile("L" + height + ".blk", serverStream);

                }

                Console.WriteLine("Stopped sending files!");
            } else {

                //TODO REQUEST FILE FROM CLIENT!!
                Console.WriteLine("YOU HAVE A NEW FILE? ILL CHECK THAT OUT!");
                requestFile();

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
