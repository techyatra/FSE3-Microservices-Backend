using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoListQuery.Interface;

namespace ToDoListQuery.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ItemsConsumerController : ControllerBase
    {
        private readonly ICosmosDBConsumer _cosmosDbService;
        public ItemsConsumerController(ICosmosDBConsumer cosmosDbService)
        {
            _cosmosDbService = cosmosDbService ?? throw new ArgumentNullException(nameof(cosmosDbService));
        }

        // GET api/items
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var data = await _cosmosDbService.GetMultipleAsync("SELECT * FROM c");
            data = data.OrderBy(x => x.TotalEffortRequired).ToList();
            return Ok(data);
        }
        // GET api/items/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _cosmosDbService.GetAsync(id));
        }
    }
}
