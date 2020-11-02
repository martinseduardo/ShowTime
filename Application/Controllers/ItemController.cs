using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly IDistributedCache _cache;

        public ItemController(ILogger<ItemController> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateAStuff(ItemRequest itemRequest)
        {
            try
            {
                _logger.LogInformation($"Adding new item, Value:{itemRequest.Value}!");

                await _cache.SetStringAsync(
                    itemRequest.Key,
                    itemRequest.Value,
                    options: new DistributedCacheEntryOptions()
                    {
                        SlidingExpiration = new TimeSpan(0, minutes: 10, 0),
                    });

                _logger.LogInformation("New item added!");

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Try add item error: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet("{key}")]
        public async Task<ActionResult<ItemRequest>> GetItem(string key)
        {
            try
            {
                var value = await _cache.GetStringAsync(key);
                if (string.IsNullOrEmpty(value))
                {
                    return NotFound();
                }
                var response = new ItemRequest()
                {
                    Key = key,
                    Value = value
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e}");
                return new ContentResult()
                {
                    StatusCode = 500
                };
            }
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> RemoveItem(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
                _logger.LogInformation($"Item:{key} removed with sucess.");
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e}");
                return new ContentResult()
                {
                    StatusCode = 500
                };
            }
        }
    }
}
