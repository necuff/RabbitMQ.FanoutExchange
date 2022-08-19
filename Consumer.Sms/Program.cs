using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer.Sms
{
    class Program
    {        
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "sms-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    //channel.ExchangeDeclare(exchange: "notifier", ExchangeType.Fanout);

                    //var queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: "sms-queue", exchange: "notifier", routingKey: String.Empty);

                    //Console.WriteLine("Waiting for payments...");

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (sender, e) =>
                    {
                        var body = e.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());

                        var payment = GetPayment(message);                        

                        Console.WriteLine($"Payment received for the amount of {payment}");                        
                    };

                    channel.BasicConsume(queue: "sms-queue", autoAck: true, consumer: consumer);

                    Console.WriteLine($"Subscribed the queue 'sms-queue'");                    

                    Console.ReadLine();
                }
            }
        }

        private static double GetPayment(string message)
        {
            var messageWords = message.Split(' ');

            return double.Parse(messageWords[^1]);
        }
    }
}
