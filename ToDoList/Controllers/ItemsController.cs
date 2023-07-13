using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Interfaces;
using ToDoList.Model;
using ToDoList.RabitMQ;
using ToDoListProducer.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToDoList.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        //private readonly IRabitMQProducer _rabitMQProducer;
        private readonly ICosmosDbService _cosmosDbService;
        private readonly IValidator<Item> _validator;
        public ItemsController(ICosmosDbService cosmosDbService
            //, IRabitMQProducer rabitMQProducer
            , IValidator<Item> validator)
        {
            _cosmosDbService = cosmosDbService ?? throw new ArgumentNullException(nameof(cosmosDbService));
           // _rabitMQProducer = rabitMQProducer;
            _validator = validator;
        }
       
       
        // POST api/items
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Item item)
        {
            var validation = await _validator.ValidateAsync(item);

            if (!validation.IsValid)
            {
                return BadRequest(validation.Errors?.Select(e => new ValidationResult()
                {
                    Code = e.ErrorCode,
                    PropertyName = e.PropertyName,
                    Message = e.ErrorMessage
                }));
            }

            item.Id = DateTime.Now.Ticks.ToString();
            await _cosmosDbService.AddAsync(item);
            var data = _cosmosDbService.GetAsync(item.Id).GetAwaiter().GetResult();
           // _rabitMQProducer.SendItemMessage(item);

            return Ok(data);
        }

        // PUT api/items/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromBody] Item item)
        {
            var validation = await _validator.ValidateAsync(item);

            if (!validation.IsValid)
            {
                return BadRequest(validation.Errors?.Select(e => new ValidationResult()
                {
                    Code = e.ErrorCode,
                    PropertyName = e.PropertyName,
                    Message = e.ErrorMessage
                }));
            }

            await _cosmosDbService.UpdateAsync(item.Id, item);
          //  _rabitMQProducer.SendItemMessage(item);
            return NoContent();
        }
        // DELETE api/items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _cosmosDbService.DeleteAsync(id);
            //_rabitMQProducer.SendItemMessage(id);
            return NoContent();

        }
    }
}
