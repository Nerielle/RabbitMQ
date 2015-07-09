using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQConsumer
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

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("hello", true, consumer);

                    Console.WriteLine(" [*] Waiting for messages." +
                                      "To exit press CTRL+C");
                    while (true)
                    {
                        BasicDeliverEventArgs ea = consumer.Queue.Dequeue();

                        byte[] body = ea.Body;
                        string message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                    }
                }
            }
        }
    }
}