using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dot.Mail.SMTP
{
    internal class EmailMessage
    {
        private string _senderAddress;
        private string _recipientAddress;
        private string _message;

        public string ClientHostname { get; set; }
        public string SenderAddress
        {
            get { return _senderAddress; }
            set
            {
                if (String.IsNullOrEmpty(ClientHostname))
                    throw new Exception("Client host name is missing");
                _senderAddress = value;
            }
        }
        public string RecipientAddress
        {
            get { return _recipientAddress; }
            set
            {
                if (String.IsNullOrEmpty(SenderAddress))
                    throw new Exception("Sender address is missing");
                _recipientAddress = value;
            }
        }
        public string Message
        {
            get { return _message; }
            set
            {
                if (String.IsNullOrEmpty(RecipientAddress))
                    throw new Exception("Recipient address is missing");
                _message = value;
            }
        }
    }
}
