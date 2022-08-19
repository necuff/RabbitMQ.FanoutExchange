using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace Producer
{
    class Program
    {
        public static readonly Random random = new Random();

        static void Main(string[] args)
        {            
            do
            {
                int timeToSleep = random.Next(1000, 3000);
                Thread.Sleep(timeToSleep);

                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        //channel.QueueDeclare(queue: "dev_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                        channel.ExchangeDeclare(exchange: "notifier", ExchangeType.Fanout);

                        var moneyCount = random.Next(1000, 10000);


                        string message = $"Payment received for the amount of {moneyCount}";

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "notifier", routingKey: "", basicProperties: null, body: body);

                        Console.WriteLine($"Payment received for the amount of {moneyCount}\nNotifying by 'notifier Exchange'");
                    }
                }

            }
            while (true);
        }
    }
}
