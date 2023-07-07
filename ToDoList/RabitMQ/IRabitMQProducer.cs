namespace ToDoList.RabitMQ
{
    public interface IRabitMQProducer
    {
        public void SendItemMessage<T>(T message);
    }
}
