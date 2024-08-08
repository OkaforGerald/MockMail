using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace MockMail.Test
{
    public class Test
    {
        public static async Task Main(string[] args)
        {
            await Task.Delay(2000); // Ensure the server is ready

            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    // Connect without using SSL/TLS
                    smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true; // Ignore certificate validation (not recommended for production)
                    await smtpClient.ConnectAsync("127.0.0.1", 25, MailKit.Security.SecureSocketOptions.None);

                    // Create and send a basic MimeMessage
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Sender Name", "sender@example.com"));
                    message.To.Add(new MailboxAddress("Receiver Name", "receiver@example.com"));
                    message.Subject = "Test Message";
                    message.Body = new TextPart("plain") { Text = "This is a test email." };

                    await smtpClient.SendAsync(message);
                    Console.WriteLine("Message sent successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    await smtpClient.DisconnectAsync(true);
                }
            }
        }

    }
}
