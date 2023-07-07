using ToDoList.Model;

namespace ToDoList.Interfaces
{
    public interface ICosmosDbService
    {
        Task<Item> GetAsync(string id);
        Task AddAsync(Item item);
        Task UpdateAsync(string id, Item item);
        Task DeleteAsync(string id);
    }
}
