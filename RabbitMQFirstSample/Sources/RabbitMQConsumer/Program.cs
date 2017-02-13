using System;
using System.Text;
using System.Threading;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQConsumer
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };
            try
            {
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare("hello", true, false, false, null);
                        var consumer = new EventingBasicConsumer(channel);

                        channel.BasicConsume("hello", false, consumer);
                        consumer.Received += (model, ea) => ReceiveMessage(ea, channel);

                        Console.WriteLine(" [*] Waiting for messages." +
                                          "To exit press CTRL+C");
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Fatal: " + ex);
            }
        }

        private static void ReceiveMessage(BasicDeliverEventArgs ea, IModel channel)
        {
            try
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] {0} Received {1}", DateTime.Now, message);

                var dots = message.Split('.').Length - 1;
                Thread.Sleep(dots*1000);
                Console.WriteLine(" [x] Done");
                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}