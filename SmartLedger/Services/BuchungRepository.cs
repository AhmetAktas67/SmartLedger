using SmartLedger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedger.Services
{
    public class BuchungRepository
    {
        public void Speichern(Buchung buchung)
        {
            using var db = new SmartLedgerDbContext();
            db.Buchungen.Add(buchung);
            db.SaveChanges();
        }

        public List<Buchung> GetAlle()
        {
            using var db = new SmartLedgerDbContext();
            return db.Buchungen.OrderByDescending(b => b.BuchungsDatum).ToList();
        }

        public bool ExistiertBereits(Buchung buchung)
        {
            using var db = new SmartLedgerDbContext();
            return db.Buchungen.Any(b =>
                b.BuchungsDatum == buchung.BuchungsDatum &&
                b.Verwendungszweck == buchung.Verwendungszweck &&
                b.Betrag == buchung.Betrag);
        }

        /*
        public void LoescheAlle()
        {
            using var db = new SmartLedgerDbContext();
            db.Buchungen.RemoveRange(db.Buchungen);
            db.SaveChanges();
        }

        */
    }
}