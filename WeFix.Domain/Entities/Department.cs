using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeFix.Domain.Entities
{
    public class Department
    {
        [Required]
        [Display(Name = "Department ID")]
        [Key]
        [Index(IsClustered = false)]
        public int DepartmentId { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        [Required]
        [Display(Name = "City")]
        public string City { get; set; }
        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        [Display(Name="Jurisdiction [KM]")]
        public decimal Jurisdiction { get; set; }
        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ForeignKey("Organisation")]
        public int OrganisationId { get; set; }       
        public virtual Organisation Organisation { get; set; }
        public virtual Category Category { get; set; }
    }
}
