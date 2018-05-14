using Crm.Models;
using Crm.Models.Report;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Crm.Services
{
    public class CrmContext : DbContext
    {
        public CrmContext()
            : base("DbConnection")
        { }
        public DbSet<Project> Projects { get; set; }
        public DbSet<UserData> Users { get; set; }
        public DbSet<Party> Parties { get; set; }
        public DbSet<PartyMapping> PartyMappings { get; set; }
        public DbSet<SqlData> SqlData { get; set; }
        /// <summary>
        /// Список заметок
        /// </summary>
        public DbSet<Note> Notes { get; set; }

        public DbSet<File> Files { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatView> ChatViews { get; set; }
        public DbSet<ChatParty> ChatParties { get; set; }
        public DbSet<ObjectLastVisit> ObjectLastVisits { get; set; }

        public DbSet<ObjectHierarchy> ObjectHierarchies { get; set; }

        public DbSet<Pin> Pins { get; set; }


        public List<ReportParty> ReportParties { get; set; }

        public List<ReportSettings> ReportSettings { get; set; }
        public List<ReportVisibleField> ReportVisibleFields { get; set; }
        public List<ReportUserSettings> ReportUserSettings { get; set; }
        public List<ReportViewDimension> ReportViewDimensions { get; set; }

        public List<ReportDimensionInterval> ReportDimensionIntervals { get; set; }
        //public DbSet<>  { get; set; }
        //public DbSet<>  { get; set; }
        //public DbSet<>  { get; set; }
        //public DbSet<>  { get; set; }
        //public DbSet<>  { get; set; }


        public DbSet<CardHolderGroup> CardHolderGroups { get; set; }
        public DbSet<DataProperty> Properties { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // PostgreSQL uses the public schema by default - not dbo.
            modelBuilder.HasDefaultSchema("crm");
            base.OnModelCreating(modelBuilder);
        }
    }


}