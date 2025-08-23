namespace CleanAspire.Application.Features.Products.EventHandlers;
/// <summary>
/// Represents an event triggered when a product is created.
/// Purpose:
/// 1. To signal the creation of a product.
/// 2. Used in the domain event notification mechanism to pass product details to subscribers.
/// </summary>
public class ProductCreatedEvent : DomainEvent
{
    /// <summary>
    /// Constructor to initialize the event and pass the created product instance.
    /// </summary>
    /// <param name="item">The created product instance.</param>
    public ProductCreatedEvent(Product item)
    {
        Item = item; // Assigns the provided product instance to the read-only property
    }

    /// <summary>
    /// Gets the product instance associated with the event.
    /// </summary>
    public Product Item { get; }
}

/*
public class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler(
        ILogger<ProductCreatedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} in {ElapsedMilliseconds} ms", notification.GetType().Name, notification, _timer.ElapsedMilliseconds);
    }
}
*/
