using System.ComponentModel.DataAnnotations;

namespace RMS.ViewModels
{
    public class UpdateUserVM
    {
        [Required(ErrorMessage = "First name is required"), Display(Name = "First Name"),
            StringLength(50, MinimumLength = 3, ErrorMessage = "First name should be between 5 and 50 characters")]
        public string FName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required"), Display(Name = "Last Name"),
            StringLength(50, MinimumLength = 3, ErrorMessage = "Last name should be between 5 and 50 characters")]
        public string LName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required"), EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required"), DataType(DataType.Date), Display(Name = "Date of Birth")]
        public string DOB { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required"),
            RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Invalid gender")]
        public string Gender { get; set; } = string.Empty;

        //[0-9]{5}: Match any digit(0-9) exactly 5 times
        //-: Match a hyphen character
        //[0 - 9]{7}: Match any digit (0-9) exactly 7 times
        //-: Match a hyphen character
        //[0-9]: Match any digit (0-9) exactly 1 time
        [Required(ErrorMessage = "CNIC is required"),
            RegularExpression("^[0-9]{5}-[0-9]{7}-[0-9]$", ErrorMessage = "Invalid CNIC number")]
        public string CNIC { get; set; } = string.Empty;

        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "Phone Number must contain 11 digits.")]
        public string Phoneno { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required"),
    StringLength(50, MinimumLength = 5, ErrorMessage = "Address should be between 5 and 50 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Profile Picture is required"), Display(Name = "Profile Picture")]
        public IFormFile? ProfilePic { get; set; }

    }

}
