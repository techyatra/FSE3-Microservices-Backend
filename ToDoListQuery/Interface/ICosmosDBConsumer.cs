using ToDoListQuery.Model;

namespace ToDoListQuery.Interface
{
    public interface ICosmosDBConsumer
    {
        Task<IEnumerable<Item>> GetMultipleAsync(string query);
        Task<Item> GetAsync(string id);
    }
}
