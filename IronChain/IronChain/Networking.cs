using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace IronChain {

    public class Networking {

        Stream serverStream;

        public void StartServer() {
            Thread a = new Thread(startListen);
            a.Start();
        }

        public void StartClient() {

            Thread a = new Thread(connectToServer);
            a.Start();

        }

        public void RequestFile(int height) {
            Thread a = new Thread(() => rqstFile(height));
            a.Start();
        }

        private void startListen() {

            TcpListener listener = new TcpListener(IPAddress.IPv6Any, 4711);
            // Listener starten
            listener.Start();

            // Warten bis ein Client die Verbindung wünscht
            Console.WriteLine("Waiting for Client to connect");
            TcpClient c = listener.AcceptTcpClient();
            Console.WriteLine("Connected to client");


            serverStream = c.GetStream();

            while (true) {

                //receiving message from client
                byte[] buffer = new byte[5];

                serverStream.Read(buffer, 0, buffer.Length);

                commandServer(buffer);
            }
        }

        Dictionary<string, NetworkStream> ClientSockets;

        private void connectToServer() {
            byte[] buffer = new byte[1024];
            TcpClient c = new TcpClient("localhost", 4711);
            serverStream = c.GetStream();
        }

        private void tellClientsOfNewFile() {

            //byte[] message = new byte[1] { 0x01 };
            //inOut.Write(message, 0, 1);
        }

        private void commandServer(byte[] command) {

            //1 means request
            //0 means push file

            if (command[0] == 1) {

                Console.WriteLine("Requesting File:");
                int height = BitConverter.ToInt32(command, 1);

                Console.WriteLine("Received FileHeader with height " + height);


                //SEND ACKNOWLEDGEMENT
                byte[] ack = new byte[32];
                if (Form1.instance.latestBlock >= height) {
                    ack = createMessage(true, height);
                    serverStream.Write(ack, 0, ack.Length);

                    sendFile(height + ".blk");
                    sendFile("P" + height + ".blk");
                    sendFile("L" + height + ".blk");

                } else {
                    ack[0] = 0x01;
                    serverStream.Write(ack, 0, ack.Length);
                }

                Console.WriteLine("Stopped sending files!");

            }

        }

        private void sendFile(string name) {

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

        private void rqstFile(int height) {

            //writing the message byte, 1 means request, 0 means push
            byte[] buffer = Enumerable.Repeat((byte)0x01, 5).ToArray();
            byte[] num = BitConverter.GetBytes(height);
            Array.Copy(num, 0, buffer, 1, num.Length);

            //sending message
            serverStream.Write(buffer, 0, buffer.Length);
            Console.WriteLine("Requesting files from block height " + height);

            // RECEIVE ACK FIRST;
            byte[] ack = new byte[32];
            int i = serverStream.Read(ack, 0, 32);
            if (ack[0] == 1) {

                Console.WriteLine("ERROR!!! FILES DO NOT EXIST");

            } else {
                //Files exist
                Console.WriteLine("NO ERROR!");

                receiveFileAndStore(height + ".blk", BitConverter.ToInt64(ack, 8));
                receiveFileAndStore("P" + height + ".blk", BitConverter.ToInt64(ack, 16));
                receiveFileAndStore("L" + height + ".blk", BitConverter.ToInt64(ack, 24));

                Form1.instance.analyseChain();

            }
        }

        private void receiveFileAndStore(string filename, long fileSize) {

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

    }
}
