using System;
using System.Text;
using System.Threading;
using NLog;
using RabbitMQ.Client;


namespace RabbitMQProducer
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            Logger.Info("info");
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("hello", true, false, false, null);

                    while (true)
                        try
                        {
                            SendMessage(args, channel);
                            Thread.Sleep(4000);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                }
            }
        }

        private static void SendMessage(string[] args, IModel model)
        {
            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);
            var properties = model.CreateBasicProperties();
            properties.DeliveryMode = 2;
            model.BasicPublish("", "hello", properties, body);
            Console.WriteLine(" [x] {1} Sent {0}", message, DateTime.Now);
        }

        private static string GetMessage(string[] args)
        {
            return args.Length > 0 ? string.Join(" ", args) : "Hello world";
        }
    }
}