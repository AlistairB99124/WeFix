using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeFix.Domain.Entities
{
    public class Fault
    {
        [Key]
        [Index(IsClustered = false)]
        [Display(Name = "Fault ID")]
        public int FaultId { get; set; }
        [Required]
        [ForeignKey("PublicUser")]
        public int PublicUserId { get; set; }
        [Display(Name = "Date Created")]
        [Required]
        public DateTime DateCreated { get; set; }
        [Display(Name ="Description")]
        public string Description { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Display(Name = "Image URL")]
        public string ImageURL { get; set; }
        [Display(Name ="Image Thumbnail")]
        public string ImageThumbnail { get; set; }
        [Display(Name = "Category ID")]
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        [Display(Name = "Sub-Category ID")]
        [ForeignKey("SubCategory")]
        public int? SubCategoryId { get; set; }
        [Display(Name = "Severity ID")]
        [ForeignKey("Severity")]
        public int? SeverityId { get; set; }
        public int? ManagerId { get; set; }
        public bool NotStarted { get; set; }
        public bool InProgress { get; set; }
        public bool Resolved { get; set; }
        
        public virtual PublicUser PublicUser { get; set; }        
        public virtual Category Category { get; set; }     
        public virtual SubCategory SubCategory { get; set; }
        public virtual Severity Severity { get; set; }
    }
}
