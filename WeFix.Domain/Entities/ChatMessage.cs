using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeFix.Domain.Entities
{
    public class ChatMessage
    {
        [Key]
        [Index(IsClustered = false)]
        [Display(Name = "Chat ID")]
        public int ChatMessageId { get; set; }
        [ForeignKey("User")]
        public string toUserId { get; set; }
        public string fromUserId { get; set; }
        public string Body { get; set; }
        public DateTime Time_Stamp { get; set; }
        public virtual User User { get; set; }
        public bool Viewed { get; set; }
    }
}
