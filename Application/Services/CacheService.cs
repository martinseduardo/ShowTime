using Application.Controllers.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<ItemResponse> GetItemFromCache(string key)
        {
            var value = await _cache.GetStringAsync(key.ToLower());
            var response = new ItemResponse()
            {
                Key = key,
                Value = value
            };
            return response ?? throw new Exception("Item not found.");
        }

        public async Task SetItemOnCache(ItemRequest itemRequest)
        {
            await _cache.SetStringAsync(
                itemRequest.Key.ToLower(),
                itemRequest.Value,
                options: new DistributedCacheEntryOptions()
                {
                    SlidingExpiration = new TimeSpan(0, minutes: 10, 0),
                });
        }

        public async Task DeleteFromCache(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
