// Summary:
// This file defines a data transfer object (DTO) for products and an enumeration for product categories.
// The ProductDto class encapsulates product details for data transfer between application layers.
// The ProductCategoryDto enum provides predefined categories for products.

namespace CleanAspire.Application.Features.Products.DTOs;

// A DTO representing a product, used to transfer data between application layers.
// By default, field names match the corresponding entity fields. For enums or referenced entities, a Dto suffix is used.
public class ProductDto
{
    public string Id { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ProductCategoryDto? Category { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Currency { get; set; }
    public string? UOM { get; set; }
}

// An enumeration representing possible product categories.
public enum ProductCategoryDto
{
    Electronics,
    Furniture,
    Clothing,
    Food,
    Beverages,
    HealthCare,
    Sports
}
