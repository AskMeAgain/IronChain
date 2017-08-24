using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace IronChain {

    public class Networking {

        private Socket socket;
        private byte[] buffer = new byte[1024];


        public Networking() {

            socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

        }

        public void bind(int port) {
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        public void listen(int backlog) {
            socket.Listen(backlog);
        }

        public void accept() {

            socket.BeginAccept(acceptedCallback, null);

        }

        private void acceptedCallback(IAsyncResult result) {

            Socket clientsocket = socket.EndAccept(result);
            buffer = new byte[1024];
            clientsocket.BeginReceive(buffer,0,buffer.Length,SocketFlags.None, receiveCallback, clientsocket);
            accept();

        }

        private void receiveCallback(IAsyncResult result) {

            Socket clientSocket = result.AsyncState as Socket;
            int bufferSize = clientSocket.EndReceive(result);
            byte[] packet = new byte[bufferSize];
            Array.Copy(buffer, packet, packet.Length);

            //handle packet
            buffer = new byte[1024];
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, receiveCallback, clientSocket);
        }
       





}
}
