using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedger.Models
{
    public class Buchung
    {
        public DateTime BuchungsDatum { get; set; }
        public string Verwendungszweck { get; set; }
        public decimal Betrag { get; set; }


        public List<Mitglied> ZugeordneteMitglieder { get; set; } = new List<Mitglied>();
    }
}
