using MassTransit;
using Newtonsoft.Json;
using ToDoListQuery.Model;

namespace ToDoListQuery.Consumer
{
    public class ItemConsumer : IConsumer<Item>
    {
        public  Task Consume(ConsumeContext<Item> context)
        {
            var jsonMessage = JsonConvert.SerializeObject(context.Message);
            Console.WriteLine("Item Created - " + jsonMessage);
            return Task.CompletedTask;
        }
    }
}
