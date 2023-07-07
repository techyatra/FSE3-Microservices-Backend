﻿using Microsoft.Azure.Cosmos;
using ToDoList.Interfaces;
using ToDoList.Model;

namespace ToDoList.Services
{
    public class CosmosDbService: ICosmosDbService
    {
        private Container _container;
        public CosmosDbService(
            CosmosClient cosmosDbClient,
            string databaseName,
            string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }
        public async Task AddAsync(Item item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.Id));
        }
        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<Item>(id, new PartitionKey(id));
        }
        public async Task<Item> GetAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Item>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException) //For handling item not found and other exceptions
            {
                return null;
            }
        }
        public async Task UpdateAsync(string id, Item item)
        {
            await _container.UpsertItemAsync(item, new PartitionKey(item.Id));
        }
    }
}
