using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQProducer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory {HostName = "localhost"};
            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("direct_logs", "direct");
                    var severity = (args.Length > 0) ? args[0] : "info";
                    string message = GetMessage(args);
                    byte[] body = Encoding.UTF8.GetBytes(message);
                    
                    channel.BasicPublish("direct_logs", severity, null, body);
                    Console.WriteLine(" [x] Sent {0}: {1}", severity, message);
                    Console.ReadLine();
                }
            }
        }

        private static string GetMessage(string[] args)
        {
            return (args.Length > 1)
                                ? string.Join(" ", args.Skip(1).ToArray())
                                : "Hello World!";
        }
    }
}