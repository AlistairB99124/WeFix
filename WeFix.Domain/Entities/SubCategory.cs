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
    public class SubCategory
    {
        [Key]
        [Index(IsClustered = false)]
        [Display(Name = "ID")]
        [HiddenInput(DisplayValue = false)]
        public int SubCategoryId { get; set; }
        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
