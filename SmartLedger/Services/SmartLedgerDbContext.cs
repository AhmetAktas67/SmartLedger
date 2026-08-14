using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartLedger.Models;


namespace SmartLedger.Services
{
    public class SmartLedgerDbContext : DbContext
    {
        public DbSet<Mitglied> Mitglieder { get; set; }
        public DbSet<Beitragszahlung> Beitragszahlungen { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "smartledger.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Beitragszahlung>()
                .HasOne(b => b.Mitglied)
                .WithMany()
                .HasForeignKey(b => b.MitgliedId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
