using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeFix.Domain.Entities
{
    public class Blog_Comments
    {
        [Key]
        [Index(IsClustered = false)]
        [Display(Name = "Comment ID")]
        public int Blog_CommentId { get; set; }
        [Required]
        [ForeignKey("Blog")]
        public int BlogId { get; set; }
        public string InReplyTo{get;set;}
        public string Comment { get; set; }
        [Required]
        [ForeignKey("User")]
        public string Commenter { get; set; }
        public bool MarkAsRead { get; set; }
        public DateTime TimeStamp { get; set; }

        public virtual Blog Blog { get; set; }
        public virtual User User { get; set; }
    }
}
