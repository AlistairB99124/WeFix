using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;

namespace WeFix.Domain.Entities
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }

        #region User Fields
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
        // Here we add a byte to Save the user Profile Picture
        [Display(Name ="Photo [Optional]")]
        public string UserPhotoUrl { get; set; }

        //Add connection ID for CHathub
        public string ConnectionId { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastOnline { get; set; }
        #endregion

    }
}