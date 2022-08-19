using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Consumer.BankCharger
{
    class Program
    {
        private static int _totalHold = 0;

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //channel.QueueDeclare(queue: "dev-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.ExchangeDeclare(exchange: "notifier", ExchangeType.Fanout);

                    var queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queueName, exchange: "notifier", routingKey: String.Empty);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (sender, e) =>
                    {
                        var body = e.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());

                        var payment = GetPayment(message);
                        _totalHold += payment;

                        Console.WriteLine($"Payment received for the amount of {payment}");
                        Console.WriteLine($"{_totalHold} hold from this person");
                    };

                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                    Console.WriteLine($"Subscribed the queue {queueName}");
                    Console.WriteLine("Listening....");

                    Console.ReadLine();
                }
            }
        }

        private static int GetPayment(string message)
        {
            var messageWords = message.Split(' ');

            return int.Parse(messageWords[^1]);
        }
    }
}
