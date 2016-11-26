using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WeFix.Domain.Entities;

namespace WeFix.WebUI.Models
{
    public class DashboardViewModel
    {
        #region Department
        [Display(Name = "Department ID")]
        [Key]
        [Index(IsClustered = false)]
        public int DepartmentId { get; set; }
        
        [Display(Name = "Name")]
        public string Name { get; set; }
        
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        
        [Display(Name = "City")]
        public string City { get; set; }
        
        [Display(Name = "Country")]
        public string Country { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        public int CategoryId { get; set; }
        public int OrganisationId { get; set; }
        public decimal Jurisdiction { get; set; }
        #endregion

        #region DepartmentManager

        [Key]
        [Index(IsClustered = false)]
        [Display(Name = "Department Manager ID")]
        [HiddenInput(DisplayValue = false)]
        public int DepartmentManagerId { get; set; }
        
        [Display(Name = "User ID")]
        [MaxLength(128), ForeignKey("User")]
        public string UserId { get; set; }
        
        [ForeignKey("Department")]
        public int DepartId { get; set; }
        public string Position { get; set; }
        public virtual User User { get; set; }
        public virtual Department Department { get; set; }

        #endregion

        #region User
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Cell Number")]
        public string Cell { get; set; }
        [Display(Name = "Manager")]
        public bool IsManager { get; set; }
        [Display(Name = "Organisation Name")]
        public string OrganisationName { get; set; }
        [Display(Name = "Approved?")]
        public bool Approved { get; set; }
        [Display(Name = "Photo [Optional]")]
        public string UserPhoto { get; set; }
        #endregion


    }
}
