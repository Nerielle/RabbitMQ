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
                    channel.QueueDeclare("rpc", false, false,false,null);
                    channel.BasicQos(0,1,false);
                   
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("rpc", false, consumer);
                    Console.WriteLine(" [x] Awaiting RPC requests");
                   
                    while (true)
                    {
                        string response = null;
                        BasicDeliverEventArgs ea = consumer.Queue.Dequeue();

                        byte[] body = ea.Body;
                        var props = ea.BasicProperties;
                        var replyProps = channel.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;
                        try
                        {
                            string message = Encoding.UTF8.GetString(body);
                            int n = int.Parse(message);
                            Console.WriteLine("[.] Fib of {0}", message);
                            response = fib(n).ToString();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(" [.] " + e.Message);
                            response = string.Empty;
                        }
                        finally
                        {
                            var responseBytes = Encoding.UTF8.GetBytes(response);
                            channel.BasicPublish("", props.ReplyTo, replyProps, responseBytes);
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                    }
                }
            }
        }

        private static int fib(int n)
        {
            if (n == 0 || n == 1) return n;
            return fib(n - 1) + fib(n - 2);
        }
    }
}