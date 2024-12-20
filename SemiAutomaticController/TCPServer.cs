using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SemiAutomaticController
{
    public class TCPServer
    {
        private TcpListener tcpListener;
        private TcpClient client;
        private NetworkStream networkStream;
        private bool isConnected;
        private float[] currentPositions = new float[6];

        public TCPServer(string serverIp, int serverPort)
        {
            // Initialize the TcpListener
            tcpListener = new TcpListener(IPAddress.Parse(serverIp), serverPort);
        }

        public void StartListening()
        {
            Console.WriteLine("Server listening for connections...");
            tcpListener.Start();

            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        client = tcpListener.AcceptTcpClient();
                        isConnected = true;

                        var clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                        if (clientEndPoint != null)
                        {
                            Console.WriteLine($"Client connected from: {clientEndPoint.Address}:{clientEndPoint.Port}");
                        }

                        // Get the network stream for communication
                        networkStream = client.GetStream();

                        // Start a new thread to handle the client
                        var clientThread = new Thread(HandleClient);
                        clientThread.Start();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error accepting client: {e.Message}");
                    }
                }
            }).Start();
        }

        private void HandleClient()
        {
            byte[] buffer = new byte[1024];

            while (isConnected)
            {
                try
                {
                    int bytesRead = networkStream.Read(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        /*
                        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        //Console.WriteLine($"Received data: {receivedData}");

                        // Parse the received JSON
                        try
                        {
                            var receivedJson = JObject.Parse(receivedData);

                            if (receivedJson.ContainsKey("currentPositions"))
                            {
                                int index = 0;
                                foreach (var pos in receivedJson["currentPositions"])
                                {
                                    currentPositions[index] = pos.Value<float>();
                                    index++;
                                }
                            }
                            else
                            {
                                Console.WriteLine("JSON does not contain 'currentPositions'");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Failed to parse JSON: {e.Message}");
                        }
                        */
                    }
                    else
                    {
                        Console.WriteLine("Client disconnected.");
                        CloseConnection();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Receive failed: {e.Message}");
                    CloseConnection();
                }
            }
        }

        public void SendData(string data)
        {
            if (isConnected && networkStream != null)
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                networkStream.Write(dataBytes, 0, dataBytes.Length);
                Console.WriteLine($"Sent data: {data}");
            }
            else
            {
                Console.WriteLine("Cannot send data; no client connected.");
            }
        }

        public float[] GetCurrentPositions()
        {
            return currentPositions;
        }

        public void CloseConnection()
        {
            if (networkStream != null)
            {
                networkStream.Close();
                networkStream = null;
            }

            if (client != null)
            {
                client.Close();
                client = null;
            }

            isConnected = false;
            Console.WriteLine("Disconnected from client.");
        }
    }
}