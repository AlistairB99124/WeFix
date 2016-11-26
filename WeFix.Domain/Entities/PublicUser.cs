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
    public class PublicUser
    {
        [Key]
        [Index(IsClustered = false)]
        [Display(Name = "ID")]
        [HiddenInput(DisplayValue = false)]
        public int PublicUserId { get; set; }
        [Required]
        [Display(Name = "User ID")]
        [MaxLength(128), ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
