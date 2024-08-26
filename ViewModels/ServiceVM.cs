using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RMS.ViewModels
{
    public class ServiceVM
    {
        public int ServiceId { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public bool IsSelected { get; set; }
    }
}
