using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using dot.Mail.Interfaces;

namespace dot.Mail.Server
{
    internal class Listener
    {
        private TcpListener _server;
        private IConnectionHandler _connectionHandler;

        internal Listener(IConnectionHandler connectionHandler, string ip, int port)
        {
            _connectionHandler = connectionHandler;

            var localAddr = IPAddress.Parse(ip);
            _server = new TcpListener(localAddr, port);
            Debug.WriteLine("Server is running on {0}:{1}", ip, port);
        }

        /// <summary>
        /// Start TcpListerner on specified IP address and port
        /// </summary>
        /// <param name="ip">Local IP address to listen on</param>
        /// <param name="port">Local TCP port to listen on</param>
        internal async void Start()
        {
            try
            {
                // Start listening for client requests.
                _server.Start();

                while (true)
                {
                    Debug.WriteLine("Waiting for connections...");
                    try
                    {
                        var tcpClient = await _server.AcceptTcpClientAsync();
                        _connectionHandler.HandleConnectionAsync(tcpClient);
                    }
                    catch (Exception exp)
                    {
                        Debug.WriteLine(exp.ToString());
                    }
                }
            }
            catch (SocketException e)
            {
                Debug.WriteLine("SocketException: {0}", e);
            }
        }

        /// <summary>
        /// Stop the TcpListener
        /// </summary>
        internal void Stop()
        {
            _server.Stop();
        }


        /*
        // Buffer for reading data
                var bytes = new Byte[256];

                // Enter the listening loop. 
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests. 
                    // You could also user server.AcceptSocket() here.
                    var client = _server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // Get a stream object for reading and writing
                    var stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client. 
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        var data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        var msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }*/
    }
}