using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Identity;
using AdonisAPI.Models.Order;

namespace AdonisAPI.Services
{
    public static class EmailSender
    {
        public static void SendOrderConfirmation(
            Order order,
            string userName,
            string userEmail
        )
        {
            try
            {
                // =======================
                // BUILD EMAIL BODY
                // =======================
                var body = new StringBuilder();

                body.AppendLine($"Hello {userName},");
                body.AppendLine();
                body.AppendLine("Your order has been placed successfully ");
                body.AppendLine();
                body.AppendLine($"Order ID: {order.Id}");
                body.AppendLine($"Order Date: {DateTime.UtcNow:dd MMM yyyy HH:mm}");
                body.AppendLine();
                body.AppendLine("Order Items:");
                body.AppendLine("------------------------------------");

                foreach (var item in order.Items)
                {
                    body.AppendLine(
                        $"{item.ProductName}  x{item.Quantity}  @ ₦{item.BasePrice:N2}  = ₦{item.LineTotal:N2}"
                    );
                }

                body.AppendLine("------------------------------------");
                body.AppendLine($"Subtotal: ₦{order.Subtotal:N2}");
                body.AppendLine($"Tax (10%): ₦{order.Tax:N2}");
                body.AppendLine($"Total: ₦{order.Total:N2}");
                body.AppendLine();
                body.AppendLine("Thank you for choosing Adonis 🍽️");
                body.AppendLine();
                body.AppendLine("Regards,");
                body.AppendLine("Adonis Team");

                // =======================
                // SMTP CONFIG
                // =======================
                var credentials = new NetworkCredential(
                    SD.AE,
                    SD.AP
                );

                var mail = new MailMessage
                {
                    From = new MailAddress(SD.AE),
                    Subject = "Order Confirmation - Adonis",
                    Body = body.ToString(),
                    IsBodyHtml = false
                };

                mail.To.Add(new MailAddress(userEmail));

                var client = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = credentials,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                // =======================
                // SEND
                // =======================
                client.Send(mail);

                Console.WriteLine($"Order email sent successfully to {userEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending order email: " + ex.Message);
            }
        }
    }
}
