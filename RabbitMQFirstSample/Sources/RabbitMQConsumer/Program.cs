using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQConsumer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("logs","fanout");
                    var queueName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queueName, "logs", "");
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(queueName, true, consumer);

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