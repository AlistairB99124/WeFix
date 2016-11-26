using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeFix.Domain.Entities
{
    public class Severity
    {
        [Key]
        [Index(IsClustered = false)]
        [Display(Name = "Severity ID")]
        public int SeverityId { get; set; }
        public string Level { get; set; }
    }
}
