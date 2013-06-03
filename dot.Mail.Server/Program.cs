using System;
using dot.Mail.SMTP;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace dot.Mail.Server
{
    static class Program
    {
        static void Main(string[] args)
        {
            Listener listener;
            
            if (args.Length == 2)
                listener = new Listener(new ConnectionHandler(), args[0], Convert.ToInt32(args[1]));
            else
                listener = new Listener(new ConnectionHandler(), "127.0.0.1", 25);
            
            listener.Start();
            Console.Read();
        }
    }
}
