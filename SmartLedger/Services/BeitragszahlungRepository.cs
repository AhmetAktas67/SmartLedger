using Microsoft.EntityFrameworkCore;
using SmartLedger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedger.Services
{
    public class BeitragszahlungRepository
    {
        public List<Beitragszahlung> GetFuerJahr(int jahr)
        {
            using var db = new SmartLedgerDbContext();
            return db.Beitragszahlungen
                      .Include(b => b.Mitglied)
                      .Where(b => b.Jahr == jahr)
                      .ToList();
        }

        public void Speichern(Beitragszahlung zahlung)
        {
            using var db = new SmartLedgerDbContext();

            if (zahlung.Id == 0)
            {
                db.Beitragszahlungen.Add(zahlung);
            }
            else
            {
                db.Beitragszahlungen.Update(zahlung);
            }

            db.SaveChanges();
        }

        public void ErstelleJahrFuerMitglied(int mitgliedId, int jahr)
        {
            using var db = new SmartLedgerDbContext();

            bool existiertSchon = db.Beitragszahlungen
                .Any(b => b.MitgliedId == mitgliedId && b.Jahr == jahr);

            if (existiertSchon)
                return;

            for (int monat = 1; monat <= 12; monat++)
            {
                db.Beitragszahlungen.Add(new Beitragszahlung
                {
                    MitgliedId = mitgliedId,
                    Jahr = jahr,
                    Monat = monat,
                    Status = ZahlungsStatus.Offen
                });
            }

            db.SaveChanges();
        }
    }
}