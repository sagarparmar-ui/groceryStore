using System.ComponentModel.DataAnnotations;

namespace Grocery_Store.ViewModels
{
    public class CheckoutVM
    {
        public Guid Id { get; set; }

        [Display(Name = "Payment Type")]
        [Required(ErrorMessage = "Name is required.")]
        public string PaymentType { get; set; } = "CashOnDelivery";

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "CVV is required.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV must be a 3-digit number.")]
        public string CVV { get; set; } = string.Empty;

        [Display(Name = "Card Number")]
        [Required(ErrorMessage = "Card Number is required.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Card Number must be a 16-digit number.")]
        public string CardNumber { get; set; } = string.Empty;

        [Display(Name = "Expiry Date")]
        [Required(ErrorMessage = "Expiry Date is required.")]
        public DateTime ExpiryDate { get; set; }

    }
}
