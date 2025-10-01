namespace CatalogApp;

public static class Repository
{
    private static readonly List<Model> Products = new List<Model>();

    public static IList<Model> GetProducts() => Products;
    public static Model? GetProduct(Guid id) => Products.Find(p => p.ID == id);
    public static void Add(Model product) => Products.Add(product);
    public static bool Update(Model product)
    {
        var productFound = Products.Find(p => p.ID == product.ID);
        if (productFound == null) return false;

        productFound.Name = product.Name;
        productFound.Description = product.Description;
        productFound.Price = product.Price;
        productFound.Category = product.Category;
        return true;
    }
    public static bool Delete(Guid id) => Products.RemoveAll(p => p.ID == id) > 0;
}
