namespace IMS.Core;

/// <summary>
/// Manages the in-memory collection of <see cref="Product"/> objects.
/// All mutating operations return an <see cref="OperationResult"/> so callers
/// never need to catch exceptions for expected business-rule failures (following DDD practices ...).
/// </summary>
public sealed class Inventory
{
    private readonly Dictionary<string, Product> _products = [];
    
    /// <summary>Returns a snapshot of all products, ordered by name.</summary>
    public IReadOnlyList<Product> GetProducts() => [.. _products.Values.OrderBy(p => p.Name)];

    /// <summary>
    /// Looks up a product by exact name (case-sensitive).
    /// Returns <c>null</c> when the product does not exist.
    /// </summary>
    public Product? Find(string name) => _products.GetValueOrDefault(name);
    
    /// <summary>
    /// Adds a product to the inventory.
    /// Fails if a product with the same name already exists.
    /// </summary>
    public OperationResult Add(Product product)
    {
        return !_products.TryAdd(product.Name, product) ? OperationResult.Fail($"A product named \"{product.Name}\" already exists.") : OperationResult.Ok();
    }

    /// <summary>
    /// Updates one or more fields of an existing product.
    /// Pass <c>null</c> for any field you do not want to change.
    /// Fails if the product is not found, or if <paramref name="newName"/>
    /// is already taken by a different product.
    /// </summary>
    public OperationResult Edit(string productName, string? newName, decimal? newPrice, int? newQuantity)
    {
        if (!_products.TryGetValue(productName, out var product))
        {
            return OperationResult.Fail($"Product \"{productName}\" was not found.");
        }

        if (newName is not null && newName != productName && _products.ContainsKey(newName))
        {
            return OperationResult.Fail($"A product named \"{newName}\" already exists.");
        }

        if (newName is not null && newName != productName)
        {
            _products.Remove(productName);
            product.Name = newName;
            _products[product.Name] = product;
        }

        if (newPrice is not null)
        {
            product.Price = newPrice.Value;
        }
        
        if (newQuantity is not null)
        {
            product.Quantity = newQuantity.Value;
        }

        return OperationResult.Ok();
    }

    /// <summary>
    /// Removes a product by name.
    /// Fails if the product is not found.
    /// </summary>
    public OperationResult Remove(string productName)
    {
        return !_products.Remove(productName) ? OperationResult.Fail($"Product \"{productName}\" was not found.") : OperationResult.Ok();
    }
}