﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
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

        [HttpPost("aStuff")]
        public async Task<IActionResult> CreateAStuff(AStuffRequest aStuffRequest)
        {
            try
            {
                _logger.LogInformation($"Adding new stuff, Value:{aStuffRequest.Value}!");

                await _cache.SetStringAsync(
                    aStuffRequest.Key,
                    aStuffRequest.Value,
                    options: new DistributedCacheEntryOptions()
                    {
                        SlidingExpiration = new TimeSpan(0, minutes: 10, 0),
                    });

                _logger.LogInformation("New stuff added!");

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Try add stuff error: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet("aStuff/{key}")]
        public async Task<ActionResult<AStuffRequest>> Get(string key)
        {
            try
            {
                var value = await _cache.GetStringAsync(key);
                if (string.IsNullOrEmpty(value))
                {
                    return NotFound();
                }
                var response = new AStuffRequest() 
                {
                    Key = key,
                    Value = value
                };
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError($"");
                return new ContentResult()
                {
                    StatusCode = 500
                };
            }
        }
    }
}
