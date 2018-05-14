using CitystarCms.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CitystarCms.Services
{
    public class CmsContext : DbContext
    {
        public CmsContext()
            : base("DbConnection")
        { }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Page> Pages { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<UserData> Users { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // PostgreSQL uses the public schema by default - not dbo.
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);
        }
    }
}