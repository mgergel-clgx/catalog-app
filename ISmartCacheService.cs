namespace CatalogApp;

public interface ISmartCacheService
{
    Task<Model?> GetAsync(Guid id);

    Task<IList<Model>?> GetAllAsync();

    Task AddAsync(Model product);

    Task<bool> UpdateAsync(Model updatedProduct);

    Task<bool> DeleteAsync(Guid id);
}
