namespace WeFix.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Blog_Comments",
                c => new
                    {
                        Blog_CommentId = c.Int(nullable: false, identity: true),
                        BlogId = c.Int(nullable: false),
                        InReplyTo = c.String(),
                        Comment = c.String(),
                        Commenter = c.String(nullable: false, maxLength: 128),
                        MarkAsRead = c.Boolean(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Blog_CommentId)
                .ForeignKey("dbo.Blogs", t => t.BlogId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.Commenter, cascadeDelete: true)
                .Index(t => t.Blog_CommentId)
                .Index(t => t.BlogId)
                .Index(t => t.Commenter);
            
            CreateTable(
                "dbo.Blogs",
                c => new
                    {
                        BlogId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Article = c.String(nullable: false),
                        AuthorId = c.String(nullable: false),
                        Date_Published = c.DateTime(nullable: false),
                        BannerImage = c.String(),
                        Featured = c.Boolean(nullable: false),
                        CommentsEnabled = c.Boolean(nullable: false),
                        Enabaled = c.Boolean(nullable: false),
                        Views = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BlogId)
                .Index(t => t.BlogId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Cell = c.String(nullable: false),
                        IsManager = c.Boolean(nullable: false),
                        UserPhotoUrl = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Image = c.String(),
                        Count = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CategoryId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.SubCategories",
                c => new
                    {
                        SubCategoryId = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.SubCategoryId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.SubCategoryId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.FaultComments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        FaultId = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                        UserId = c.String(),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Faults", t => t.FaultId, cascadeDelete: true)
                .Index(t => t.CommentId)
                .Index(t => t.FaultId);
            
            CreateTable(
                "dbo.Faults",
                c => new
                    {
                        FaultId = c.Int(nullable: false, identity: true),
                        PublicUserId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        Description = c.String(),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        ImageURL = c.String(),
                        ImageThumbnail = c.String(),
                        CategoryId = c.Int(),
                        SubCategoryId = c.Int(),
                        SeverityId = c.Int(),
                        ManagerId = c.Int(),
                        NotStarted = c.Boolean(nullable: false),
                        InProgress = c.Boolean(nullable: false),
                        Resolved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FaultId)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .ForeignKey("dbo.PublicUsers", t => t.PublicUserId, cascadeDelete: true)
                .ForeignKey("dbo.Severities", t => t.SeverityId)
                .ForeignKey("dbo.SubCategories", t => t.SubCategoryId)
                .Index(t => t.FaultId)
                .Index(t => t.PublicUserId)
                .Index(t => t.CategoryId)
                .Index(t => t.SubCategoryId)
                .Index(t => t.SeverityId);
            
            CreateTable(
                "dbo.PublicUsers",
                c => new
                    {
                        PublicUserId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.PublicUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.PublicUserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Severities",
                c => new
                    {
                        SeverityId = c.Int(nullable: false, identity: true),
                        Level = c.String(),
                    })
                .PrimaryKey(t => t.SeverityId)
                .Index(t => t.SeverityId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryCode = c.String(nullable: false, maxLength: 128),
                        CountryAccessCode = c.String(nullable: false),
                        CountryName = c.String(nullable: false),
                        CountryRegion = c.String(nullable: false),
                        CountryContinent = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CountryCode)
                .Index(t => t.CountryCode);
            
            CreateTable(
                "dbo.DepartmentManagers",
                c => new
                    {
                        DepartmentManagerId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        DepartmentId = c.Int(nullable: false),
                        CountPasswordChanges = c.Int(nullable: false),
                        Position = c.String(),
                    })
                .PrimaryKey(t => t.DepartmentManagerId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.DepartmentManagerId)
                .Index(t => t.UserId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        AddressLine1 = c.String(nullable: false),
                        AddressLine2 = c.String(),
                        City = c.String(nullable: false),
                        Country = c.String(nullable: false),
                        PostalCode = c.String(),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        Jurisdiction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CategoryId = c.Int(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.DepartmentId)
                .Index(t => t.CategoryId)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.Organisations",
                c => new
                    {
                        OrganisationId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Approved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.OrganisationId)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.OrganisationManagers",
                c => new
                    {
                        OrganisationManagerId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        OrganisationId = c.Int(nullable: false),
                        Position = c.String(),
                    })
                .PrimaryKey(t => t.OrganisationManagerId)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.OrganisationManagerId)
                .Index(t => t.UserId)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.ReportSuburbs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Suburb = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.OrganisationManagers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrganisationManagers", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.DepartmentManagers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DepartmentManagers", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Departments", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.Departments", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.FaultComments", "FaultId", "dbo.Faults");
            DropForeignKey("dbo.Faults", "SubCategoryId", "dbo.SubCategories");
            DropForeignKey("dbo.Faults", "SeverityId", "dbo.Severities");
            DropForeignKey("dbo.Faults", "PublicUserId", "dbo.PublicUsers");
            DropForeignKey("dbo.PublicUsers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Faults", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.SubCategories", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Blog_Comments", "Commenter", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Blog_Comments", "BlogId", "dbo.Blogs");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.OrganisationManagers", new[] { "OrganisationId" });
            DropIndex("dbo.OrganisationManagers", new[] { "UserId" });
            DropIndex("dbo.OrganisationManagers", new[] { "OrganisationManagerId" });
            DropIndex("dbo.Organisations", new[] { "OrganisationId" });
            DropIndex("dbo.Departments", new[] { "OrganisationId" });
            DropIndex("dbo.Departments", new[] { "CategoryId" });
            DropIndex("dbo.Departments", new[] { "DepartmentId" });
            DropIndex("dbo.DepartmentManagers", new[] { "DepartmentId" });
            DropIndex("dbo.DepartmentManagers", new[] { "UserId" });
            DropIndex("dbo.DepartmentManagers", new[] { "DepartmentManagerId" });
            DropIndex("dbo.Countries", new[] { "CountryCode" });
            DropIndex("dbo.Severities", new[] { "SeverityId" });
            DropIndex("dbo.PublicUsers", new[] { "UserId" });
            DropIndex("dbo.PublicUsers", new[] { "PublicUserId" });
            DropIndex("dbo.Faults", new[] { "SeverityId" });
            DropIndex("dbo.Faults", new[] { "SubCategoryId" });
            DropIndex("dbo.Faults", new[] { "CategoryId" });
            DropIndex("dbo.Faults", new[] { "PublicUserId" });
            DropIndex("dbo.Faults", new[] { "FaultId" });
            DropIndex("dbo.FaultComments", new[] { "FaultId" });
            DropIndex("dbo.FaultComments", new[] { "CommentId" });
            DropIndex("dbo.SubCategories", new[] { "CategoryId" });
            DropIndex("dbo.SubCategories", new[] { "SubCategoryId" });
            DropIndex("dbo.Categories", new[] { "CategoryId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Blogs", new[] { "BlogId" });
            DropIndex("dbo.Blog_Comments", new[] { "Commenter" });
            DropIndex("dbo.Blog_Comments", new[] { "BlogId" });
            DropIndex("dbo.Blog_Comments", new[] { "Blog_CommentId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ReportSuburbs");
            DropTable("dbo.OrganisationManagers");
            DropTable("dbo.Organisations");
            DropTable("dbo.Departments");
            DropTable("dbo.DepartmentManagers");
            DropTable("dbo.Countries");
            DropTable("dbo.Severities");
            DropTable("dbo.PublicUsers");
            DropTable("dbo.Faults");
            DropTable("dbo.FaultComments");
            DropTable("dbo.SubCategories");
            DropTable("dbo.Categories");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Blogs");
            DropTable("dbo.Blog_Comments");
        }
    }
}
