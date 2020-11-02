using Application.Controllers.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly ILogger<CacheController> _logger;
        private readonly IDistributedCache _cache;

        public CacheController(ILogger<CacheController> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpPost("item")]
        public async Task<IActionResult> CreateItem(ItemRequest itemRequest)
        {
            try
            {
                _logger.LogInformation($"Adding new item, Value:{itemRequest.Value}!");
                
                await _cache.SetStringAsync(
                    itemRequest.Key.ToLower(),
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

        [HttpGet("item/{key}")]
        public async Task<ActionResult<ItemRequest>> GetItem(string key)
        {
            try
            {
                var value = await _cache.GetStringAsync(key.ToLower());
                if (string.IsNullOrEmpty(value))
                {
                    return NotFound();
                }
                var response = new ItemResponse()
                {
                    Key = key,
                    Value = value
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e}");
                return StatusCode(500);
            }
        }

        [HttpDelete("item/{key}")]
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
                return StatusCode(500);
            }
        }
    }
}
