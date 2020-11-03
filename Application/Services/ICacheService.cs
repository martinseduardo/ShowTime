using Application.Controllers.DataContracts;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ICacheService
    {
        Task<ItemResponse> GetItemFromCache(string key);

        Task SetItemOnCache(ItemRequest itemRequest);

        Task DeleteFromCache(string key);
    }
}