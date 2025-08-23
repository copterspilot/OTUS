using CleanAspire.Domain.Common;

namespace CleanAspire.Domain.Entities;

/// <summary>
/// Represents a product entity.
/// </summary>
public class Product : BaseAuditableEntity, IAuditTrial
{
    /// <summary>
    /// Gets or sets the SKU of the product.
    /// </summary>
    public string SKU { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category of the product.
    /// </summary>
    public ProductCategory Category { get; set; } = ProductCategory.Electronics;

    /// <summary>
    /// Gets or sets the description of the product.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the price of the product.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the currency of the product price.
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Gets or sets the unit of measure of the product.
    /// </summary>
    public string? UOM { get; set; }
}


/// <summary>
/// Represents the category of a product.
/// </summary>
public enum ProductCategory
{
    /// <summary>
    /// Electronics category.
    /// </summary>
    Electronics,

    /// <summary>
    /// Furniture category.
    /// </summary>
    Furniture,

    /// <summary>
    /// Clothing category.
    /// </summary>
    Clothing,

    /// <summary>
    /// Food category.
    /// </summary>
    Food,

    /// <summary>
    /// Beverages category.
    /// </summary>
    Beverages,

    /// <summary>
    /// Health care category.
    /// </summary>
    HealthCare,

    /// <summary>
    /// Sports category.
    /// </summary>
    Sports,
}
