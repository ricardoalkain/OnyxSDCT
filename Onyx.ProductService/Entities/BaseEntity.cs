namespace Onyx.ProductService.Entities;

public abstract record BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }
}
