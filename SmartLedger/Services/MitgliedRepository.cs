using SmartLedger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartLedger.Models;

namespace SmartLedger.Services
{
    internal class MitgliedRepository
    {
        public List<Mitglied> GetAlle()
        {
            using var db = new SmartLedgerDbContext();
            return db.Mitglieder.ToList();
        }

        public void Speichern(Mitglied mitglied)
        {
            using var db = new SmartLedgerDbContext();
            db.Mitglieder.Add(mitglied);
            db.SaveChanges();
        }

        public void Loeschen(int mitgliedId)
        {
            using var db = new SmartLedgerDbContext();
            var mitglied = db.Mitglieder.Find(mitgliedId);
            if (mitglied != null)
            {
                db.Mitglieder.Remove(mitglied);
                db.SaveChanges();
            }
        }
    }
}

