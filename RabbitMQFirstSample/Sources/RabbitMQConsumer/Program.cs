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
                    channel.ExchangeDeclare("topic_logs","topic");
                    var queueName = channel.QueueDeclare().QueueName;
                    if (args.Length < 1)
                    {
                        Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
                                           Environment.GetCommandLineArgs()[0]);
                        Environment.ExitCode = 1;
                        return;
                    }
                    foreach (var bindingKey in args)
                    {
                        channel.QueueBind(queueName, "topic_logs", bindingKey);
                        
                    }
                    
                    Console.WriteLine(" [*] Waiting for messages." +
                                      "To exit press CTRL+C");
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(queueName, true, consumer);

                   
                    while (true)
                    {
                        BasicDeliverEventArgs ea = consumer.Queue.Dequeue();

                        byte[] body = ea.Body;
                        string message = Encoding.UTF8.GetString(body);
                        var routinKey = ea.RoutingKey;
                        Console.WriteLine(" [x] Received {0}: {1}",routinKey, message);
                    }
                }
            }
        }
    }
}