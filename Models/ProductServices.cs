using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Models
{
    public class ProductCategories
    {
        [Key]
        public Guid ServiceAssignmentID { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service? Service { get; set; }
    }

}
