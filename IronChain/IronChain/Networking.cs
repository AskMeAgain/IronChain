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


            inOut = c.GetStream();

            while (true) {

                //receiving message from client
                byte[] buffer = new byte[5];

                inOut.Read(buffer, 0, buffer.Length);

                commandServer(buffer);
            }
        }

        private void connectToServer() {
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

                if (Form1.instance.latestBlock >= height) {

                    //write filesize
                    byte[] message = new byte[] { 0x00 };
                    long[] fileSizes = allocateFileSize(height);

                    inOut.Write(message, 0, message.Length);


                    sendFile(height + ".blk");


                    sendFile("P" + height + ".blk");


                    sendFile("L" + height + ".blk");

                } else {
                    byte[] message = new byte[] { 0x01 };
                    inOut.Write(message, 0, message.Length);
                    Console.WriteLine("Error file does not exist!");
                }

                Console.WriteLine("Stopped sending files!");

            } else {
                Console.WriteLine("Pushing file!");
            }

        }

        private long[] allocateFileSize(int height) {

            long[] size = new long[3];


            size[0] = new FileInfo(Form1.instance.globalChainPath + "P"+height+".blk").Length;
            size[1] = new FileInfo(Form1.instance.globalChainPath + "L" + height + ".blk").Length;
            size[2] = new FileInfo(Form1.instance.globalChainPath + height + ".blk").Length;

            Console.WriteLine(size[0] + " " + size[1] + " " + size[2]);

            return size;

        }

        private void sendFile(string name) {

            //TODO SENDING BIG FILES
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

                inOut.Write(fileArray, startIndex, endPointer);
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
            inOut.Write(buffer, 0, buffer.Length);
            Console.WriteLine("Requesting files from block height " + height);

            // RECEIVE ERROR CODE FIRST
            byte[] error = new byte[1];
            int i = inOut.Read(error, 0, error.Length);
            if (error[0] == 1) {
                Console.WriteLine("ERROR!!! FILES DO NOT EXIST");
                return;
            } else {
                //Files exist
                Console.WriteLine("NO ERROR!");
            }

            Console.WriteLine("Writing into file now!");
            receiveFileAndStore(height + ".blk");
            receiveFileAndStore("P" + height + ".blk");
            receiveFileAndStore("L" + height + ".blk");

            Form1.instance.analyseChain();
        }

        private void receiveFileAndStore(string filename) {

            string file = Form1.instance.globalChainPath + filename;

            if (File.Exists(file)) {
                File.Delete(file);
            }

            byte[] receiveBuffer = new byte[1024];
            FileStream fileStream = new FileStream(file, FileMode.Append);

            int counter = 0;

            while (true) {

                int receivedBytes = inOut.Read(receiveBuffer, 0, receiveBuffer.Length);
                counter += receivedBytes;

                if (receivedBytes == 0)
                    break;

                fileStream.Write(receiveBuffer, 0, receivedBytes);
                Console.WriteLine("?" + receivedBytes);

                if (receivedBytes < receiveBuffer.Length)
                    break;
            }

            fileStream.Close();
            Console.WriteLine("Received" + counter);
        }


    }
}
