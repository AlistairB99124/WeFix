using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeFix.Domain.Entities
{
    public class Organisation
    {
        [Required]
        [Key]
        [Index(IsClustered = false)]
        public int OrganisationId { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Approved { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
}
