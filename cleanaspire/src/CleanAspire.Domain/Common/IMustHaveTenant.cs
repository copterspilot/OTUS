namespace CleanAspire.Domain.Common;

public interface IMustHaveTenant
{
    string TenantId { get; set; }
}

public interface IMayHaveTenant
{
    string? TenantId { get; set; }
}