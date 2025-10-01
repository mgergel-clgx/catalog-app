using System.Text.Json;
using StackExchange.Redis;

namespace CatalogApp;

public class SmartCacheService : ISmartCacheService
{
    private const int CacheExpiryInMinutes = 5;
    private const string CacheKeyAllProducts = "products";
    private readonly IDatabase _cache;

    public SmartCacheService(IConnectionMultiplexer redis)
    {
        _cache = redis.GetDatabase();
    }

    public async Task<Model?> GetAsync(Guid id) => await Get<Model>($"product:{id}", id);

    public async Task<IList<Model>?> GetAllAsync() => await Get<IList<Model>>(CacheKeyAllProducts);

    public async Task AddAsync(Model product)
    {
        Repository.Add(product);

        await RemoveAllAsync();
        await SetAsync($"product:{product.ID}", JsonSerializer.Serialize(product));
    }

    public async Task<bool> UpdateAsync(Model updatedProduct)
    {
        if (!Repository.Update(updatedProduct)) return false;

        await RemoveAllAsync();
        await RemoveAsync(updatedProduct.ID);

        await SetAsync($"product:{updatedProduct.ID}", JsonSerializer.Serialize(updatedProduct));
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!Repository.Delete(id)) return false;

        await RemoveAllAsync();
        await RemoveAsync(id);
        return true;
    }

    private async Task RemoveAsync(Guid id) => await _cache.KeyDeleteAsync($"product:{id}");

    private async Task RemoveAllAsync() => await _cache.KeyDeleteAsync(CacheKeyAllProducts);

    private async Task<T?> Get<T>(string key, Guid? id = null) where T : class
    {
        var cachedProduct = await _cache.StringGetAsync(key);
        if (!string.IsNullOrEmpty(cachedProduct))
        {
            return JsonSerializer.Deserialize<T>(cachedProduct);
        }

        var product = id == null ? Repository.GetProducts() as T : Repository.GetProduct(id.Value) as T;
        if (product == null) return null;

        await SetAsync(key, JsonSerializer.Serialize(product));
        return product;
    }

    private async Task SetAsync(string key, string value) => await _cache.StringSetAsync(key, value, TimeSpan.FromMinutes(CacheExpiryInMinutes));
}
