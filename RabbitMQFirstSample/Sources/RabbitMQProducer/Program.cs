using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQConsumer;

namespace RabbitMQProducer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var client = new RpcClient();

            Console.WriteLine("Request of 50...");
            var response = client.Call("50");
            Console.WriteLine("Result {0}", response);

            client.Close();
        }
        
    }
}