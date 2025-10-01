using Microsoft.AspNetCore.Mvc;

namespace CatalogApp;

[ApiController]
[Route("api/products")]
public class Controller : ControllerBase
{
    private const string NotFoundMessage = "The product with this ID has not been found";
    private const string InvalidIDParameter = "Invalid ID parameter";

    private readonly ISmartCacheService _cacheService;

    public Controller(ISmartCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var productList = await _cacheService.GetAllAsync();
        return Ok(productList);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(string id)
    {
        if (!Guid.TryParse(id, out var validID)) return BadRequest(InvalidIDParameter);

        var product = await _cacheService.GetAsync(validID);
        return product == null ? NotFound(NotFoundMessage) : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] Model product)
    {
        product.ID = Guid.NewGuid();
        await _cacheService.AddAsync(product);

        return CreatedAtAction(nameof(AddProduct), new { id = product.ID }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(string id, [FromBody] Model updatedProduct)
    {
        if (!Guid.TryParse(id, out var validID)) return BadRequest(InvalidIDParameter);

        updatedProduct.ID = validID;
        return await _cacheService.UpdateAsync(updatedProduct) ? Ok() : NotFound(NotFoundMessage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        if (!Guid.TryParse(id, out var validID)) return BadRequest(InvalidIDParameter);

        return await _cacheService.DeleteAsync(validID) ? Ok() : NotFound(NotFoundMessage);
    }
}
