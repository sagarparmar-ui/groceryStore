using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Models
{
    public class ShoppingCart : BaseEntity
    {
        public enum StatusEnum
        {
            Pending,
            CheckedOut,
            Delivered
        }
        public string ProductName { get; set; } = string.Empty;
        public string Resident { get; set; } = string.Empty;
        public string ResidentPhone { get; set; } = string.Empty;
        public string? PaymentType { get; set; }
        public int Price { get; set; }
        public string PriceType { get; set; } = string.Empty;
        public DateTime Date { get; set; }

        public StatusEnum Status { get; set; }

        // Foreign key properties
        [ForeignKey("ProductId")]
        public Guid ProductId { get; set; }

        [ForeignKey("ResidentId")]
        public Guid ResidentId { get; set; }

        [ForeignKey("SellerId")]
        public Guid SellerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string CardNumber { get; set; } = string.Empty;

        public string CVV { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

    }

}
