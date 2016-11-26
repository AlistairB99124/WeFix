using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeFix.Domain.Entities
{
    public class Category
    {
        [Key]
        [Index(IsClustered = false)]
        [Display(Name = "Category ID")]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Count { get; set; }
        public virtual ICollection<SubCategory> SubCategories { get; set; }
    }
}
