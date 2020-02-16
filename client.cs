using System;
using System.Net.Sockets;
using UnityEngine;

namespace client {
    static class NetworkClient {
        private static Int32 port = 4242;
        private static TcpClient client;
        private static NetworkStream stream;
        public static void Connect(String server) {
            try {
                // Create a TcpClient.
                client = new TcpClient(server, port);
                stream = client.GetStream();
            }
            catch (ArgumentNullException e) {
                Debug.Log("ArgumentNullException: " + e);
            }
            catch (SocketException e) {
                Debug.Log("SocketException: " + e);
            }
        }

        public static void Send(String message) {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log("Sent: " + message);
        }

        public static string Receive() {
            Byte[] data = new Byte[100];
            Int32 bytes = stream.Read(data, 0, data.Length);
            for(int i = 0; i < data.Length; i++) {
                if(data[i] == 0) {
                    data[i] = 32; 
                }
            }
            String responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            responseData = responseData.Replace(" ", "");
            Debug.Log("Received: " + responseData);
            return responseData;
        }

        public static void Close() {
            stream.Close();
            client.Close();
        }
    }
}