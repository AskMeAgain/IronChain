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

        Stream inOut;

        public void StartServer() {
            Thread a = new Thread(startListen);
            a.Start();
        }

        public void StartClient() {

            Thread a = new Thread(connectToServer);
            a.Start();

        }

        public void startListen() {

            TcpListener listener = new TcpListener(IPAddress.IPv6Any, 4711);
            // Listener starten
            listener.Start();

            // Warten bis ein Client die Verbindung wünscht
            Console.WriteLine("Waiting for Client to connect");
            TcpClient c = listener.AcceptTcpClient();
            Console.WriteLine("Connected to client");


            inOut = c.GetStream();

            while (true) {
                //receiving message from client
                byte[] buffer = new byte[5];
                inOut.Read(buffer, 0, buffer.Length);

                commandServer(buffer);
            }
        }

        public void connectToServer() {
            byte[] buffer = new byte[1024];
            TcpClient c = new TcpClient("localhost", 4711);
            inOut = c.GetStream();
        }

        private void commandServer(byte[] command) {

            //1 means request
            //0 means push file

            if (command[0] == 1) {
                Console.WriteLine("Requesting File:");
                int height = BitConverter.ToInt32(command, 1);
                Console.WriteLine("Received FileHeader with height " + height);

                sendFile(height + ".blk");
                sendFile("P" + height + ".blk");
                sendFile("L" + height + ".blk");

                Console.WriteLine("Stopped sending files!");


            } else {
                Console.WriteLine("Pushing file!");
            }

        }

        private void sendFile(string name) {

            //Console.WriteLine("Sending files from " + height + " to XXX");

            //TODO SENDING BIG FILES

            byte[] buffer = new byte[1024];

            FileStream File = new FileStream(@"C:\IronChain\" + name, FileMode.Open, FileAccess.Read);
            byte[] fileArray = System.IO.File.ReadAllBytes(@"C:\IronChain\" + name);
            File.Close();

            int startIndex = 0;
            int fileLength = fileArray.Length;
            int bytesMissing;

            do {

                bytesMissing = fileLength - startIndex;
                int endPointer = buffer.Length;

                if (bytesMissing < buffer.Length) {
                    endPointer = bytesMissing;
                }

                inOut.Write(fileArray, startIndex, endPointer);
                startIndex += buffer.Length;
            } while (startIndex < fileLength);
            Console.WriteLine("Finished sending file!");

        }

        public void requestFile(int height) {

            //writing the message byte, 1 means request, 0 means push
            byte[] buffer = Enumerable.Repeat((byte)0x01, 5).ToArray();
            byte[] num = BitConverter.GetBytes(height);
            Array.Copy(num, 0, buffer, 1, num.Length);

            //sending message
            inOut.Write(buffer, 0, buffer.Length);
            Console.WriteLine("Requesting files from block height " + height);

            // RECEIVE
            Console.WriteLine("Writing into file now!");
            receiveBlocksAndStore(height + ".blk");
            receiveBlocksAndStore("P" + height + ".blk");
            receiveBlocksAndStore("L" + height + ".blk");

            Form1.instance.analyseChain();
        }

        private void receiveBlocksAndStore(string filename) {
            byte[] receiveBuffer = new byte[1024];

            string file = "C:\\IronChain\\COPY_" + filename;
            if (File.Exists(file)) {
                File.Delete(file);
            }

            FileStream fileStream = new FileStream(file, FileMode.Append);

            while (true) {

                int receivedBytes = inOut.Read(receiveBuffer, 0, receiveBuffer.Length);
                fileStream.Write(receiveBuffer, 0, receivedBytes);
                Console.WriteLine("?" + receivedBytes);

                if (receivedBytes < receiveBuffer.Length)
                    break;
            }

            fileStream.Close();
            Console.WriteLine("Received ");
        }


    }
}
