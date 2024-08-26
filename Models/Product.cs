using System.ComponentModel.DataAnnotations;


namespace RMS.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string Seller { get; set; } = string.Empty;

        public int Price { get; set; }

        public string PriceType { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phoneno { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string ProductImage { get; set; } = string.Empty;

        public bool AllowShoppingCarts { get; set; } = true;

        public Guid SellerId { get; set; }
        public IList<ProductCategories>? Services { get; set; }
        public ICollection<ShoppingCart>? ShoppingCarts { get; set; }
    }

}
