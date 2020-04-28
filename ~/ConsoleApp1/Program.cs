using System;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace ServerTest {
    class Program {
        const string END_CONNECTION = "EC";
        //max players 10
        private static string[] userData = new string[10]{"","","","","","","","","",""};
        private static int connections = 0;
        
        static void Main(string[] args) {
            Program main = new Program();
            main.server_start(); //starting the server
            Console.ReadLine();
        }

        TcpListener server = new TcpListener(IPAddress.Parse("192.168.0.110"), 4242);

        private void server_start() {
            server.Start();
            accept_connection(); //accepts incoming connections
        }

        private void accept_connection() {
            server.BeginAcceptTcpClient(handle_connection,
                server); //this is called asynchronously and will run in a different thread
        }

        private void handle_connection(IAsyncResult result) {
            accept_connection(); //once again, checking for any other incoming connections
            TcpClient client = server.EndAcceptTcpClient(result); //creates the TcpClient

            NetworkStream stream = client.GetStream();
            
            int connectionId = -1;
            // check if there are any disconnected user slots
            for (var i = 0; i < connections; i++) {
                if (userData[i] == END_CONNECTION) {
                    // take their old slot 
                    connectionId = i;
                    break;
                }
            }
            // if no disconnected user slot 
            if (connectionId == -1) {
                connectionId = connections;
                connections++;
            }

            byte[] hello = new byte[200];
            hello = Encoding.Default.GetBytes(" ");

            stream.Write(hello, 0, hello.Length); //sending the message
            Console.WriteLine("New connection with id: " + connectionId);
            
            while (client.Connected) //while the client is connected, we look for incoming messages
            {
                if (stream.DataAvailable) {
                    byte[] msg = new byte[400];
                    stream.Read(msg, 0, msg.Length);
                    String messageFromClient = Encoding.UTF8.GetString(msg).Trim(' ');
                    // remove weird mystery character that causes packets to be broken up
                    userData[connectionId] = messageFromClient.TrimEnd(messageFromClient[^1]);
                    // Console.WriteLine(userData[connectionId]);
                    if (userData[connectionId] == END_CONNECTION) {
                        break;
                    }

                    string dataToSend = "  ";
                    bool first = true;
                    if (connectionId == 0) {
                        Console.WriteLine("Clients:");
                    }

                    for (var i = 0; i < connections; i++) {
                        if (i != connectionId) {
                            if (first) {
                                dataToSend = userData[i];
                                first = false;
                            }
                            else {
                                dataToSend += ":" + userData[i];
                            }
                            if (connectionId == 0) {
                                Console.WriteLine(userData[i]);
                            }
                        }
                    }
                    byte[] response = new byte[200];
                    response = Encoding.Default.GetBytes(dataToSend);
                    stream.Write(response, 0, response.Length);
                }
            }
            Console.WriteLine("Client " + connectionId + " disconnected.");
        }
    }
}