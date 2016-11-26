using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeFix.Domain.Entities
{
    public class FaultComment
    {
        [Key]
        [Index(IsClustered = false)]
        [Display(Name = "Comment ID")]
        public int CommentId { get; set; }
        [Required]
        [Display(Name ="Fault")]
        [ForeignKey("Fault")]
        public int FaultId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
        public virtual Fault Fault { get; set; }
    }
}
