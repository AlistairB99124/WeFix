using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeFix.Domain.Entities
{
    public class Country
    {
        [Key]
        [Index(IsClustered = false)]
        [Required]
        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }
        [Display(Name = "Country Access Code")]
        [Required]
        public string CountryAccessCode { get; set; }
        [Display(Name = "Country Name")]
        [Required]
        public string CountryName { get; set; }
        [Display(Name = "Country Region")]
        [Required]
        public string CountryRegion { get; set; }
        [Display(Name = "Country Continent")]
        [Required]
        public string CountryContinent { get; set; }
        
    }
}
