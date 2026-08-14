using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedger.Models
{
    public enum ZahlungsStatus
    {
        Offen,
        BestaetigtManuell,
        VorschlagKI
    }

    public class Beitragszahlung
    {
        public int Id { get; set; }
        public int MitgliedId { get; set; }
        public Mitglied Mitglied { get; set; }
        public int Jahr { get; set; }
        public int Monat { get; set; }
        public ZahlungsStatus Status { get; set; }
        public string? Kommentar { get; set; }
    }
}