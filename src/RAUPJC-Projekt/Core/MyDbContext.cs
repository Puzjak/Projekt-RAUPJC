using System.Data.Entity;
using RAUPJC_Projekt.Core.ServiceLogic;
using RAUPJC_Projekt.Core.TermDateLogic;
using DbContext = System.Data.Entity.DbContext;

namespace RAUPJC_Projekt.Core
{
    public class MyDbContext : DbContext
    {
        public IDbSet<TermDate> TermDates { get; set; }
        public IDbSet<Service> Services { get; set; }

        public MyDbContext(string connectionString) : base(connectionString)
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Service>().HasKey(s => s.ServiceId);
            modelBuilder.Entity<Service>().Property(s => s.Name).IsRequired();
            modelBuilder.Entity<Service>().Property(s => s.Duration).IsRequired();

            modelBuilder.Entity<TermDate>().HasKey(t => t.TermDateId);
            modelBuilder.Entity<TermDate>().HasRequired(t => t.Service).WithMany(t => t.TermDates);
            modelBuilder.Entity<TermDate>().Property(t => t.Description).IsOptional().HasMaxLength(225);
            modelBuilder.Entity<TermDate>().Property(t => t.UserId).IsRequired();
            modelBuilder.Entity<TermDate>().Property(t => t.StartOfTerm).IsRequired();
            modelBuilder.Entity<TermDate>().Property(t => t.EndOfTerm).IsRequired();

        }
    }
}
