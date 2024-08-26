using System.ComponentModel.DataAnnotations;

namespace RMS.Models
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CreatedOn { get; set; }
        public Guid? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set;}
        public Guid? UpdatedBy { get; set;}

        public DateTime? DeletedOn { get; set; }
        public Guid? DeletedBy { get; set; }

        public bool isActive { get; set; } = true;
        public bool isDeleted { get; set; } = false;
      
    }

}
