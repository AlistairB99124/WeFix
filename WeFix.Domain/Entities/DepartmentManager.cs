using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WeFix.Domain.Entities
{
    public class DepartmentManager
    {
        [Key]
        [Index(IsClustered = false)]
        [Display(Name = "Department Manager ID")]
        [HiddenInput(DisplayValue = false)]
        public int DepartmentManagerId { get; set; }
        [Required]
        [Display(Name = "User ID")]
        [MaxLength(128), ForeignKey("User")]
        public string UserId { get; set; }
        [Required]
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public int CountPasswordChanges { get; set; }
        public string Position { get; set; }        
        public virtual User User { get; set; }
        public virtual Department Department { get; set; }
        
    }
}
