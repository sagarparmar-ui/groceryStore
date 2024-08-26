using System.ComponentModel.DataAnnotations;

namespace RMS.ViewModels
{
    public class ProductDetailsVM
    {
        public Guid Id { get; set; }

        [Display(Name = "Product Name")]
        [Required(ErrorMessage = "Product Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Seller Name")]
        [Required(ErrorMessage = "Seller Name is required.")]
        public string Seller { get; set; } = string.Empty;

        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price of the Product is required."),
         Range(1, int.MaxValue, ErrorMessage = "Price must be a positive integer.")]
        public int Price { get; set; }

        [Display(Name = "Price Type"), Required(ErrorMessage = "Type is required.")]
        public string PriceType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required."),
            StringLength(50, MinimumLength = 5, ErrorMessage = "Location should be between 5 and 50 characters")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(\d{3}-\d{3}-\d{4})$", ErrorMessage = "Phone Number must be in the format XXX-XXX-XXXX.")]
        public string Phoneno { get; set; } = string.Empty;

        [StringLength(200, MinimumLength = 10, ErrorMessage = "Description should be between 10 and 200 characters")]
        public string? Description { get; set; }

        [Display(Name = "Product Image")]
        [Required(ErrorMessage = "Please select your product image.")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "The maximum file size allowed is 5MB")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" }, ErrorMessage = "Only image files are allowed")]
        public IFormFile? ProductImage { get; set; }

        public IList<ServiceVM>? Services { get; set; }

        public Guid SellerId { get; set; }
    }

}
