
using System.ComponentModel.DataAnnotations;

namespace RMS.ViewModels
{
    public class AccountDeleteVM
    {
        [Required(ErrorMessage = "You must agree to the terms and conditions.")]
        public bool ConfirmDelete { get; set; }
    }

}
