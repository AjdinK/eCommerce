using EasyNetQ;
using eCommerce.Model.Messages;


// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var bus = RabbitHutch.CreateBus("host=localhost;username=admin;password=admin");
string subscriptionId = "product-updates"; // DateTime.Now.Ticks.ToString();

bus.PubSub.Subscribe<ProductUpdated>(subscriptionId, message =>
{
    Console.WriteLine($"Received product update for product with id {message.Id} and name {message.Data.Name}");
});


Console.WriteLine("Press any key to exit...");
Console.ReadKey();