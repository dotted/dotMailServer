using System;
using FluentAssertions;
using Xunit;
using dot.Mail.SMTP;

namespace dot.Mail.SMTPTests
{
    public class EmailMessageTests
    {
        [Fact]
        public void MessageMustOnlyBeAddedAfterRecipientAddressIsKnown()
        {
            var emailMessage = new EmailMessage();
            emailMessage.Invoking(y => y.Message = "Test message")
                .ShouldThrow<Exception>()
                .WithMessage("Recipient address is missing");
        }

        [Fact]
        public void MessageMustOnlyBeAddedAfterSenderAddressIsKnown()
        {
            var emailMessage = new EmailMessage();
            emailMessage.Invoking(y => y.Message = "Test message")
                .ShouldThrow<Exception>()
                .WithMessage("Sender address is missing");
        }
    }
}
