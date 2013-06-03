using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Rsft.Net.Dns.Entities;
using dot.Mail.Interfaces;
using Rsft.Net.Dns;

namespace dot.Mail.SMTP
{
    public class ConnectionHandler : IConnectionHandler
    {
        public async Task HandleConnectionAsync(TcpClient tcpClient)
        {
            var clientInfo = tcpClient.Client.RemoteEndPoint.ToString();
            var emailMessage = new EmailMessage();
            Console.WriteLine("Got connection request from {0}", clientInfo);
            try
            {
                using (var networkStream = tcpClient.GetStream())
                using (var reader = new StreamReader(networkStream))
                using (var writer = new StreamWriter(networkStream))
                {
                    writer.AutoFlush = true;

                    await writer.WriteLineAsync("220 Helo there");

                    while (true)
                    {
                        reader.DiscardBufferedData();
                        var dataFromServer = await reader.ReadLineAsync();
                        if (dataFromServer.StartsWith("HELO"))
                        {
                            //OLD protocol
                            emailMessage.ClientHostname = dataFromServer.Substring(5);
                            await writer.WriteLineAsync("250 Ok"); 
                        }
                        if (dataFromServer.StartsWith("EHLO"))
                        {
                            //NEW protocol
                            emailMessage.ClientHostname = dataFromServer.Substring(5);
                            await writer.WriteLineAsync("250 Ok");
                        }
                        if (dataFromServer.StartsWith("MAIL FROM:"))
                        {
                            emailMessage.SenderAddress = dataFromServer.Substring(10);
                            await writer.WriteLineAsync("250 Ok");
                        }
                        if (dataFromServer.StartsWith("RCPT TO:"))
                        {
                            emailMessage.RecipientAddress = dataFromServer.Substring(8);
                            await writer.WriteLineAsync("250 Ok");
                        }

                        if (dataFromServer.StartsWith("DATA"))
                        {
                            await writer.WriteLineAsync("354 Start mail input; end with <crlf>.<crlf>");
                            var message = new StringBuilder();
                            while (true)
                            {
                                var messageLine = await reader.ReadLineAsync();
                                
                                if (messageLine == ".")
                                    break;
                                
                                message.Append(messageLine);
                            }
                            emailMessage.Message = message.ToString();
                            await TransmitMessage(emailMessage);
                            await writer.WriteLineAsync("250 Ok");

                        }

                        if (dataFromServer.StartsWith("QUIT"))
                        {
                            break;
                        }
                        Console.WriteLine(dataFromServer);
                    }

                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.ToString());
            }
            finally
            {
                Console.WriteLine("Closing the client connection - {0}", clientInfo);
                //tcpClient.Close();
            }
        }

        internal async Task TransmitMessage(EmailMessage emailMessage)
        {
            /*var address = new MailAddress(emailMessage.RecipientAddress);
            var mxRecords = await Dns.QueryAsync(address.Host, QType.MX);

            var server = "";

            // Just grap the first one and hope for the best
            foreach (var mxRecord in mxRecords.RecordsMX)
            {
                server = mxRecord.Exchange;
                break;
            }*/

            //var tcpClient = new TcpClient(server, 25);
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("127.0.0.1", 25);
            var clientInfo = tcpClient.Client.RemoteEndPoint.ToString();

            try
            {
                using (var networkStream = tcpClient.GetStream())
                using (var reader = new StreamReader(networkStream))
                using (var writer = new StreamWriter(networkStream))
                {
                    writer.AutoFlush = true;

                    var state = "";

                    while (true)
                    {
                        var dataFromServer = await reader.ReadLineAsync();
                        Console.WriteLine(dataFromServer);

                        if (dataFromServer.StartsWith("220"))
                        {
                            //await writer.WriteLineAsync(String.Format("HELO {0}", GetLocalhostFqdn()));
                            await writer.WriteLineAsync(String.Format("EHLO {0}", GetLocalhostFqdn()));
                            state = "HELO";
                        }
                        if (dataFromServer.StartsWith("250"))
                        {
                            // Maximum laziness
                            if (state == "HELO")
                            {
                                await writer.WriteLineAsync(String.Format("MAIL FROM:{0}", emailMessage.SenderAddress));
                                state = "MAIL FROM";
                            }
                            if (state == "MAIL FROM")
                            {
                                await writer.WriteLineAsync(String.Format("RCPT TO:{0}", emailMessage.RecipientAddress));
                                state = "RCPT TO";
                            }
                            if (state == "RCPT TO")
                            {
                                await writer.WriteLineAsync("DATA");
                                state = "DATA";
                            }
                            if (state == "DATA")
                            {
                                await writer.WriteAsync(emailMessage.Message);
                                state = "";
                            }
                            if (state == "")
                            {
                                await writer.WriteLineAsync("QUIT");
                            }
                        }
                        if (dataFromServer.StartsWith("354"))
                        {
                            const string eom = "\r\n.\r\n";
                            await writer.WriteAsync(emailMessage.Message + eom);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.ToString());
            }
            finally
            {
                Console.WriteLine("Closing the client connection - {0}", clientInfo);
                //tcpClient.Close();
            }
        }

        internal static string GetLocalhostFqdn()
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            return string.Format("{0}.{1}", ipProperties.HostName, ipProperties.DomainName);
        }
    }
}
