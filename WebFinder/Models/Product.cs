namespace WebFinder.Models
{
    public class Product
    {
        public string? Url { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public bool IsInStock { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Price}€)";
        }
    }
}
