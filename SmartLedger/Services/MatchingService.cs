using SmartLedger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedger.Services
{
    public class MatchingService
    {
        public class MatchErgebnis
        {
            public Buchung Buchung { get; set; }
            public List<Mitglied> GematchteMitglieder { get; set; } = new List<Mitglied>();
        }

        public List<MatchErgebnis> MatcheBuchungen(List<Buchung> buchungen, List<Mitglied> alleMitglieder)
        {
            var ergebnisse = new List<MatchErgebnis>();

            foreach (var buchung in buchungen)
            {
                var ergebnis = new MatchErgebnis { Buchung = buchung };
                string textLower = buchung.Verwendungszweck.ToLower();

                foreach (var mitglied in alleMitglieder)
                {
                    bool vornameGefunden = textLower.Contains(mitglied.Vorname.ToLower());
                    bool nachnameGefunden = textLower.Contains(mitglied.Nachname.ToLower());

                    if (vornameGefunden && nachnameGefunden)
                    {
                        ergebnis.GematchteMitglieder.Add(mitglied);
                    }
                }

                ergebnisse.Add(ergebnis);
            }

            return ergebnisse;
        }


        public int WendeMatchesAn(List<MatchErgebnis> matches, BeitragszahlungRepository beitragsRepository, int jahr)
        {
            int angewendet = 0;

            foreach (var match in matches)
            {
                if (match.GematchteMitglieder.Count == 0) continue;

                int monat = match.Buchung.BuchungsDatum.Month;

                foreach (var mitglied in match.GematchteMitglieder)
                {
                    var zahlungen = beitragsRepository.GetFuerJahr(jahr);
                    var zahlung = zahlungen.FirstOrDefault(z => z.MitgliedId == mitglied.Id && z.Monat == monat);

                    if (zahlung != null && zahlung.Status == ZahlungsStatus.Offen)
                    {
                        zahlung.Status = ZahlungsStatus.VorschlagKI;
                        beitragsRepository.Speichern(zahlung);
                        angewendet++;
                    }
                }
            }

            return angewendet;
        }
    }
}