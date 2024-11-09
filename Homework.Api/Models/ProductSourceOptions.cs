using System.Collections.Generic;

namespace Homework.Api.Models
{
    public class ProductSourceOptions
    {
        public List<ProductSource> Sources { get; set; } = new();
    }

    public class ProductSource
    {
      public required string Url { get; set; }
      public required string ParsingStrategy { get; set; }
    }
}
