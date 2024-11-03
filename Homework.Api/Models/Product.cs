namespace Homework.Api.Models
{
  public class Product
  {
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }
    public float Rating { get; set; }
    public string? Thumbnail { get; set; }
  }
}
