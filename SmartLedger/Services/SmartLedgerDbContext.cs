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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=smartledger.db");
        }
    }
}
