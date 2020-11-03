using Application.Controllers.DataContracts;
using Application.Services;
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
        private readonly ICacheService _cacheService;

        public CacheController(ILogger<CacheController> logger, ICacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        [HttpPost("item")]
        public async Task<IActionResult> CreateItem(ItemRequest itemRequest)
        {
            try
            {
                await _cacheService.SetItemOnCache(itemRequest);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Try add item error: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet("item/{key}")]
        public async Task<ActionResult<ItemResponse>> GetItem(string key)
        {
            try
            {
                var response = await _cacheService.GetItemFromCache(key);
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError($"Try get item error: {e}");
                return StatusCode(500);
            }
        }

        [HttpDelete("item/{key}")]
        public async Task<IActionResult> RemoveItem(string key)
        {
            try
            {
                await _cacheService.DeleteFromCache(key);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Try delete item error: {e}");
                return StatusCode(500);
            }
        }
    }
}
