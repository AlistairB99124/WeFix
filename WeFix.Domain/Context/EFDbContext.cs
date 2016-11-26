using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeFix.Domain.Entities;

namespace WeFix.Domain.Context
{
    public class EFDbContext : IdentityDbContext<User>
    {
        public EFDbContext()
            : base("EFDbContext", throwIfV1Schema: false)
        {
        }

        public static EFDbContext Create()
        {
            return new EFDbContext();
        }

        public DbSet<DepartmentManager> DepartmentManagers { get; set; }
        public DbSet<OrganisationManager> OrganisationManagers { get; set; }
        public DbSet<PublicUser> PublicUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Severity> Severities { get; set; }
        public DbSet<Fault> Faults { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<ReportSuburb> ReportSuburbs { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<FaultComment> Comments { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Blog_Comments> Blog_Comments { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
    }    
}
