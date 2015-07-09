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
                    channel.QueueDeclare("hello", false, false, false, null);

                    string message = "Hello World!";
                    byte[] body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("", "hello", null, body);
                    Console.WriteLine(" [x] Sent {0}", message);
                    Console.ReadLine();
                }
            }
        }
    }
}