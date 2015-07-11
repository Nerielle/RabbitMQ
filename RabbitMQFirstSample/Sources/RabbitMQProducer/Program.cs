using System;
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
                    channel.ExchangeDeclare("logs", "fanout");

                    string message = GetMessage(args);
                    byte[] body = Encoding.UTF8.GetBytes(message);
                    
                    channel.BasicPublish("logs", "", null, body);
                    Console.WriteLine(" [x] Sent {0}", message);
                    Console.ReadLine();
                }
            }
        }

        private static string GetMessage(string[] args)
        {
            return (args.Length > 0) ? string.Join(" ", args) : "Hello world";
        }
    }
}