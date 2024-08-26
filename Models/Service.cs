using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Policy;

namespace RMS.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }

        public ICollection<ProductCategories>? Products { get; set; }

    }
}
