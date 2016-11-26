using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeFix.Domain.Entities
{
    public class Blog
    {
        [Key]
        [Index(IsClustered = false)]
        [Display(Name = "Comment ID")]
        public int BlogId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Article { get; set; }
        [Required]
        public string AuthorId { get; set; }
        [Required]
        public DateTime Date_Published { get; set; }
        public string BannerImage { get; set; }
        public bool Featured { get; set; }
        public bool CommentsEnabled { get; set; }
        public bool Enabaled { get; set; }
        public int Views { get; set; }
        
    }
}
