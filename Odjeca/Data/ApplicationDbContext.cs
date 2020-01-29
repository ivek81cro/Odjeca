using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Odjeca.Models;

namespace Odjeca.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; }
        public DbSet<StoreItem> StoreItem { get; set; }
        public DbSet<Discount> Discount { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}
